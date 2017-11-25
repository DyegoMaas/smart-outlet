using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using SmartOutlet.Outlet.EventSourcing.Events;
using SmartOutlet.Outlet.EventSourcing.ProjectionViews;

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
            using (var session = _documentStore.OpenSession())
            {
                if (!plugIds.Any())
                {
                    plugIds = session.Events.QueryRawEventDataOnly<PlugActivated>()
                        .Select(x => x.PlugId)
                        .ToArray();
                }
                return session.LoadMany<Plug>(plugIds);
            }
        }
    }
}