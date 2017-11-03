using System;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing
{
    public class PlugHistory
    {
        public Guid Id { get; set; } = Plugs.PlugOneId;
        public PlugState State { get; set; } = PlugState.Off;
        public string Name { get; set; } = "PlugOne";
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

        public void Apply(PlugRenamed plugRenamed)
        {
            Name = plugRenamed.NewName;
        }
    }
}