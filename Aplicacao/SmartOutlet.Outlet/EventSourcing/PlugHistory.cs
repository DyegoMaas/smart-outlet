using System;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing
{
    public class PlugHistory
    {
        public Guid Id { get; set; }
        public PlugState State { get; set; }
        public string Name { get; set; }
        public double LastConsumptionInWatts { get; set; }

        public bool IsOn() => State == PlugState.On;

        public void Apply(PlugActivated activation)
        {
            Id = activation.PlugId;
            Name = activation.PlugName;
        }
        
        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            State = PlugState.On;
        }
        
        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            State = PlugState.Off;
        }

        public void Apply(ConsumptionReadingReceived consumptionReading)
        {
            LastConsumptionInWatts = consumptionReading.ConssumptionInWatts;
        }
    }
}