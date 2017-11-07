using System;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Outlet
{
    public class SmartPlug : ISmartPlug
    {
        private readonly IPublisher _publisher;

        public SmartPlug(IPublisher publisher)
        {
            _publisher = publisher;
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

        private static string GetMilisecondsString(TimeSpan timeInFuture)
        {
            return ((int)timeInFuture.TotalMilliseconds).ToString();
        }
    }
}