using System;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Outlet
{
    public class SmartPlug : ISmartPlug
    {
        private readonly IPublisher _publisher;
        private readonly IPlugEventSequencer _plugEventSequencer;

        public SmartPlug(IPublisher publisher, IPlugEventSequencer plugEventSequencer)
        {
            _publisher = publisher;
            _plugEventSequencer = plugEventSequencer;
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
            _plugEventSequencer.PlugRenamed(newName, plugId);
        }

        public Guid CreatePlug(string name)
        {
            var plugId = Guid.NewGuid();
            _plugEventSequencer.PlugActivated(plugId, name);
            return plugId;
        }

        private static string GetMilisecondsString(TimeSpan timeInFuture)
        {
            return ((int)timeInFuture.TotalMilliseconds).ToString();
        }
    }
}