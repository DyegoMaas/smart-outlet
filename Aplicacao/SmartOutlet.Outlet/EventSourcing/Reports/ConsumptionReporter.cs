﻿using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public class ConsumptionReporter : IConsumptionReporter
    {
        private readonly IDocumentStore _documentStore;

        public ConsumptionReporter(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<ConsumptionInTime> GetConsumptionReport(Guid plugId, DateTime? startTime = null)
        {
            using (var session = _documentStore.LightweightSession())
            {
                var consumption = session.Events
                    .FetchStream(plugId, timestamp:startTime)
                    .Where(e => e.Data.GetType() == typeof(ConsumptionReadingReceived))
                    .Select(e =>
                    {
                        var reading = (ConsumptionReadingReceived) e.Data;
                        return new ConsumptionInTime(reading.ConssumptionInWatts, e.Timestamp);
                    });
                return consumption;
            }
        }
    }
}