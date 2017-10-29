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
                session.DeleteWhere<ConsumptionReading>(reading => true);
                session.SaveChanges();
            }
            return documentStore;
        }
    }
}