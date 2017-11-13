using System;
using System.Globalization;
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

            container.Register<ISmartPlug, SmartPlug>();
            container.Register<IPlugEventSequencer, PlugEventSequencer>();
            container.Register<IPlugStateReporter, PlugStateReporter>();
            container.Register<IConsumptionReporter, ConsumptionReporter>();

            container.Register<IDocumentStore>(DocumentStorageFactory.NewEventSource<Plug>(
                typeof(PlugActivated),
                typeof(PlugTurnedOn),
                typeof(PlugTurnedOff),
                typeof(ConsumptionReadingReceived)    
            ));

            var messaging = new Messaging();
            container.Register<IPublisher>(messaging);
            container.Register<ITopicGuest>(messaging);

            RegisterSubscribers(messaging, container);
        }

        private void RegisterSubscribers(Messaging messaging, TinyIoCContainer container)
        {
            messaging.Subscribe("/smart-plug/new-state", message =>
            {
                var plugEventEmitter = container.Resolve<IPlugEventSequencer>();
                
                var newState = CleanString(message);
                switch (newState)
                {
                    case "on":
                        plugEventEmitter.PlugTurnedOn(PlugIds.PlugOneId);
                        break;
                    case "off":
                        plugEventEmitter.PlugTurnedOff(PlugIds.PlugOneId);
                        break;
                }
            });
            
            messaging.Subscribe("/smart-plug/consumption", value =>
            {
                var consumptionInWatts = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                var plugEventEmitter = container.Resolve<IPlugEventSequencer>();
                plugEventEmitter.NewConsumption(PlugIds.PlugOneId, consumptionInWatts);
            });
        }

        private static string CleanString(string message)
        {
            return message.Trim().ToLower();
        }
    }
}