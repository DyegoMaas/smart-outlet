using System;

namespace SmartOutlet.Outlet.Consumption
{
    public class ConsumptionReading
    {
        public Guid Id { get; set; }
        public string PlugName { get; set; }
        public PlugState PlugState { get; set; }
        public double ConsumoEmWatts { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}