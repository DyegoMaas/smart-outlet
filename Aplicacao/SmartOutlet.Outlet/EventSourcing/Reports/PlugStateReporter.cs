using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Plug> GetStateReport(params Guid[] plugIds)
        {
            using (var session = _documentStore.LightweightSession())
            {
                if (!plugIds.Any())
                {
                    // ReSharper disable once RemoveToList.1
                    plugIds = session
                        .Query<Plug>()
                        .Select(x => x.Id)
                        .ToList()
                        .ToArray();
                }
                return session.LoadMany<Plug>(plugIds);
            }
        }
    }
}