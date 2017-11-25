using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Marten;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using SmartOutlet.Outlet;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.EventSourcing.AggregatingRoots;
using SmartOutlet.Outlet.EventSourcing.Events;
using SmartOutlet.Outlet.EventSourcing.Reports;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Service
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
                ctx.Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS,POST,GET");
            });
        }
                
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<ISmartPlug, SmartPlug>();
            container.Register<IPlugEventSequencer, PlugEventSequencer>();
            container.Register<IPlugStateReporter, PlugStateReporter>();
            container.Register<IConsumptionReporter, ConsumptionReporter>();

            ConfigureAggregatingRoots(container);

            var messaging = ConfigureMQTTMessagingSystem(container);
            RegisterSubscribers(messaging, container);
        }

        private static void ConfigureAggregatingRoots(TinyIoCContainer container)
        {
            container.Register<IDocumentStore>(DocumentStore.For(_ =>
            {
                _.Connection(GetConnectionString());
                
                _.Events.AddEventTypes(new []
                {
                    typeof(PlugActivated),
                    typeof(PlugTurnedOn),
                    typeof(PlugTurnedOff),
                    typeof(PlugRenamed),
                    typeof(OperationScheduled),
                    typeof(ConsumptionReadingReceived)
                });
                
                _.Events.InlineProjections.AggregateStreamsWith<Plug>();
                _.Events.InlineProjections.AggregateStreamsWith<TimeLine>();
            }));

            string GetConnectionString() => ConfigurationManager.ConnectionStrings["Production"].ConnectionString;
        }

        private static Messaging ConfigureMQTTMessagingSystem(TinyIoCContainer container)
        {
            var messaging = new Messaging();
            container.Register<IPublisher>(messaging);
            container.Register<ITopicGuest>(messaging);
            return messaging;
        }

        private void RegisterSubscribers(Messaging messaging, TinyIoCContainer container)
        {
            messaging.Subscribe("/smart-things/plug/new-state", message =>
            {
                if (!IsValidMessageWithId(message))
                    return;
                
                var payload = ExtractPayload(message);
                var newState = payload.Content;
                var plugEventEmitter = container.Resolve<IPlugEventSequencer>();
                switch (newState)
                {
                    case "on":
                        plugEventEmitter.PlugTurnedOn(payload.PlugId);
                        break;
                    case "off":
                        plugEventEmitter.PlugTurnedOff(payload.PlugId);
                        break;
                }
            });
            
            messaging.Subscribe("/smart-things/plug/consumption", message =>
            {
                if (!IsValidMessageWithId(message))
                    return;
                
                var payload = ExtractPayload(message);
                var consumptionInWatts = Convert.ToDouble(payload.Content, CultureInfo.InvariantCulture);

                var plugEventEmitter = container.Resolve<IPlugEventSequencer>();
                plugEventEmitter.NewConsumption(payload.PlugId, consumptionInWatts);
            });
        }

        private bool IsValidMessageWithId(string message)
        {
            if (!message.Contains("|"))
                return false;
            
            var parts = message.Split('|');
            Guid id = Guid.Empty;
            return Guid.TryParse(parts[0], out id);
        }

        private Payload ExtractPayload(string message)
        {
            var parts = message.Split('|');

            var originPlugId = Guid.Parse(parts[0]);
            var content = CleanString(parts[1]);
            return new Payload
            {
                PlugId = originPlugId,
                Content = content,
            };
        }

        private struct Payload
        {
            public Guid PlugId { get; set; }
            public string Content { get; set; }
        }

        private static string CleanString(string message)
        {
            return message.Trim().ToLower();
        }
    }
}