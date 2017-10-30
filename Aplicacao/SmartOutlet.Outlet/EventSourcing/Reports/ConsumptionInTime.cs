using System;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public class ConsumptionInTime
    {
        public DateTimeOffset TimeStamp { get;  }
        public double ConsumptionInWatts { get; }

        public ConsumptionInTime(double consumptionInWatts, DateTimeOffset timeStamp)
        {
            TimeStamp = timeStamp;
            ConsumptionInWatts = consumptionInWatts;
        }
    }
}