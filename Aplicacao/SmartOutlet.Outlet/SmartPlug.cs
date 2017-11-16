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
            _publisher.Publish("/smart-plug/state", $"{plugId}|turn-off");
        }

        public void TryTurnOn(Guid plugId)
        {
            _publisher.Publish("/smart-plug/state", $"{plugId}|turn-on");
        }

        public void ScheduleTurnOn(TimeSpan timeInFuture, Guid plugId)
        {
            _publisher.Publish("/smart-plug/schedule-on", $"{plugId}|GetMilisecondsString(timeInFuture)");
        }

        public void ScheduleTurnOff(TimeSpan timeInFuture, Guid plugId)
        {
            _publisher.Publish("/smart-plug/schedule-off", $"{plugId}|GetMilisecondsString(timeInFuture)");
        }

        public void Rename(string newName, Guid plugId)
        {
            _plugEventSequencer.PlugRenamed(newName, plugId);
        }

        public Guid CreatePlug(string name)
        {
            var plugId = Guid.NewGuid();
            _plugEventSequencer.PlugActivated(plugId, name);
            _publisher.Publish("/smart-plug/activate", $"{plugId}");
            return plugId;
        }

        private static string GetMilisecondsString(TimeSpan timeInFuture)
        {
            return ((int)timeInFuture.TotalMilliseconds).ToString();
        }
    }
}