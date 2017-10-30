using System;
using System.Collections.Generic;
using Marten;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public class PlugStateReporter : IPlugStateReporter
    {
        private readonly IDocumentStore _documentStore;

        public PlugStateReporter(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<PlugHistory> GetStateReport(params Guid[] plugIds)
        {
            using (var session = _documentStore.LightweightSession())
            {
                return session.LoadMany<PlugHistory>(plugIds);
            }
        }
    }
}