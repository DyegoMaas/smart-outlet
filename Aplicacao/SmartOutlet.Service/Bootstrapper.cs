using System;
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
            container.Register<IPlugEventEmitter, PlugEventEmitter>();
            container.Register<IConsumptionReporter, ConsumptionReporter>();

            container.Register<IDocumentStore>(DocumentStorageFactory.NewEventSource<PlugHistory>(
                typeof(PlugActivated),
                typeof(PlugTurnedOn),
                typeof(PlugTurnedOff),
                typeof(ConsumptionReadingReceived)    
            ));

            var messaging = new Messaging();
            container.Register<IPublisher>(messaging);
            container.Register<ITopicClientele>(messaging);

            RegisterSubscribers(messaging, container);
        }

        private void RegisterSubscribers(Messaging messaging, TinyIoCContainer container)
        {
            messaging.Subscribe("/smart-plug/new-state/on", s =>
            {
                var plugEventEmitter = container.Resolve<IPlugEventEmitter>();
                plugEventEmitter.PlugTurnedOn(Plugs.PlugOneId);
            });
            
            messaging.Subscribe("/smart-plug/new-state/off", s =>
            {
                var plugEventEmitter = container.Resolve<IPlugEventEmitter>();
                plugEventEmitter.PlugTurnedOff(Plugs.PlugOneId);
            });
            
            messaging.Subscribe("/smart-plug/consumption", value =>
            {
                var consumptionInWatts = Convert.ToDouble(value);
                var plugEventEmitter = container.Resolve<IPlugEventEmitter>();
                plugEventEmitter.NewConsumption(Plugs.PlugOneId, consumptionInWatts);
            });
        }
    }
}