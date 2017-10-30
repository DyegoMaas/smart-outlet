using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class ConsumptionReadingReceived
    {
        public double ConssumptionInWatts { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public ConsumptionReadingReceived(double conssumptionInWatts)
        {
            ConssumptionInWatts = conssumptionInWatts;
        }

        protected ConsumptionReadingReceived()
        {
        }
    }
}