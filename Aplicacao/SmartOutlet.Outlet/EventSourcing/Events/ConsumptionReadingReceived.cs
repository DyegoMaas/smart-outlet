using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class ConsumptionReadingReceived
    {
        public double ConsumptionInWatts { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public ConsumptionReadingReceived(double consumptionInWatts)
        {
            ConsumptionInWatts = consumptionInWatts;
        }

        protected ConsumptionReadingReceived()
        {
        }
    }
}