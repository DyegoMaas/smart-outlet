using System;
using Marten;
using SmartOutlet.Outlet.Consumption;

namespace SmartOutlet.Outlet.Tests
{
    public static class DocumentStoreForTests
    {
        public static DocumentStore SetupNew()
        {
            var documentStore = DocumentStore
                .For("host=localhost;database=smartthings_test;password=postgres;username=postgres");
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
                _.Connection("host=localhost;database=smartthings_test;password=postgres;username=postgres");

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
    }
}