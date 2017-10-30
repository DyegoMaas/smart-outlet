using System;
using System.Configuration;
using Marten;
using SmartOutlet.Outlet.Consumption;

namespace SmartOutlet.Outlet.Tests.Integration
{
    public static class DocumentStoreForTests
    {
        public static DocumentStore SetupNew()
        {
            var documentStore = DocumentStore
                .For(GetConnectionString());
            using (var session = documentStore.LightweightSession())
            {
                session.DeleteWhere<ConsumptionReading>(reading => true); //TODO parametrizar
                session.SaveChanges();
            }
            return documentStore;
        }

        public static DocumentStore NewEventSource<TAgreggator>(params Type[] eventTypes) 
            where TAgreggator : class, new()
        {
            var store = DocumentStore.For(_ =>
            {
                _.Connection(GetConnectionString());

                _.Events.AddEventTypes(eventTypes);
                _.Events.InlineProjections.AggregateStreamsWith<TAgreggator>();
            });
            using (var session = store.LightweightSession())
            {
                session.DeleteWhere<TAgreggator>(reading => true);
                session.SaveChanges();
            }
            return store;
        }

        private static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Tests"].ConnectionString;
            return connectionString;
        }
    }
}