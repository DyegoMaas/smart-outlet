using System;

namespace SmartOutlet.Outlet.EventSourcing
{
    public class ConsumptionReadingReceived
    {
        public double ConsumoEmWatts { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public ConsumptionReadingReceived(double consumoEmWatts)
        {
            ConsumoEmWatts = consumoEmWatts;
        }

        protected ConsumptionReadingReceived()
        {
        }
    }
}