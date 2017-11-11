﻿using System;
using Marten;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing
{
    public class PlugEventEmitter : IPlugEventEmitter
    {
        private readonly IDocumentStore _documentStore;

        public PlugEventEmitter(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void NewConsumption(Guid plugId, double consumptionInWatts)
        {
            AppendEvent(plugId, new ConsumptionReadingReceived(consumptionInWatts));
        }

        public void PlugTurnedOn(Guid plugId)
        {
            AppendEvent(plugId, new PlugTurnedOn());
        }
        
        public void PlugTurnedOff(Guid plugId)
        {
            AppendEvent(plugId, new PlugTurnedOff());
        }

        public void PlugRenamed(string newName, Guid plugId)
        {
            AppendEvent(plugId, new PlugRenamed(newName));
        }

        private void AppendEvent<T>(Guid plugId, T @event)
        {
            using (var session = _documentStore.OpenSession())
            {
                session.Events.Append(plugId, @event);
                session.SaveChanges();
            }
        }
    }
}