using System;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Outlet
{
    public class SmartPlug : ISmartPlug
    {
        private readonly IPublisher _publisher;
        private readonly IPlugEventEmitter _plugEventEmitter;

        public SmartPlug(IPublisher publisher, IPlugEventEmitter plugEventEmitter)
        {
            _publisher = publisher;
            _plugEventEmitter = plugEventEmitter;
        }

        public void TryTurnOff(Guid plugId)
        {
            _publisher.Publish("/smart-plug/state", "turn-off");
        }

        public void TryTurnOn(Guid plugId)
        {
            _publisher.Publish("/smart-plug/state", "turn-on");
        }

        public void ScheduleTurnOn(TimeSpan timeInFuture)
        {
            _publisher.Publish("/smart-plug/schedule-on", GetMilisecondsString(timeInFuture));
        }

        public void ScheduleTurnOff(TimeSpan timeInFuture)
        {
            _publisher.Publish("/smart-plug/schedule-off", GetMilisecondsString(timeInFuture));
        }

        public void Rename(string newName, Guid plugId)
        {
            _plugEventEmitter.PlugRenamed(newName, plugId);
        }

        public Guid CreatePlug(string name)
        {
            var plugId = Guid.NewGuid();
            _plugEventEmitter.PlugActivated(plugId, name);
            return plugId;
        }

        private static string GetMilisecondsString(TimeSpan timeInFuture)
        {
            return ((int)timeInFuture.TotalMilliseconds).ToString();
        }
    }
}