using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class ConsumptionReadingReceived : IPlugEvent
    {
        public double ConsumptionInWatts { get; set; }
        public DateTime IssuedAt { get; set; }

        public ConsumptionReadingReceived(double consumptionInWatts)
        {
            ConsumptionInWatts = consumptionInWatts;
            IssuedAt = DateTime.Now;
        }

        protected ConsumptionReadingReceived()
        {
        }
    }
}