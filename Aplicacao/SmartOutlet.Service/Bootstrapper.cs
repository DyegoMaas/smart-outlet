using System;
using System.Configuration;
using Marten;
using Nancy;
using Nancy.TinyIoc;
using SmartOutlet.Outlet;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.EventSourcing.Events;
using SmartOutlet.Outlet.EventSourcing.Reports;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Service
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IConsumptionReporter, ConsumptionReporter>();
            container.Register<ISmartPlug, SmartPlug>();

            container.Register<IDocumentStore>(NewEventSource<PlugHistory>(
                typeof(PlugActivated),
                typeof(PlugTurnedOn),
                typeof(PlugTurnedOff),
                typeof(ConsumptionReadingReceived)    
            ));

            var messaging = new Messaging();
            container.Register<IPublisher>(messaging);
            container.Register<ITopicClientele>(messaging);
        }

        private static DocumentStore NewEventSource<TAgreggator>(params Type[] eventTypes) 
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
            return ConfigurationManager.ConnectionStrings["Production"].ConnectionString;
        }
    }
}