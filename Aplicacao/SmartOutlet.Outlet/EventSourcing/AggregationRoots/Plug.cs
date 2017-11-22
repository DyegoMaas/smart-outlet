using System;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.AggregationRoots
{
    public class Plug
    {
        public Guid Id { get; set; } = PlugIds.PlugOneId;
        public PlugState CurrentState { get; set; } = PlugState.Off;
        public string Name { get; set; }
        public double LastConsumptionInWatts { get; set; }

        public bool IsOn() => CurrentState == PlugState.On;

        public void Apply(PlugActivated activation)
        {
            Id = activation.PlugId;
            Name = activation.PlugName;
        }
        
        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            CurrentState = PlugState.On;
        }
        
        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            CurrentState = PlugState.Off;
        }

        public void Apply(ConsumptionReadingReceived consumptionReading)
        {
            LastConsumptionInWatts = consumptionReading.ConsumptionInWatts;
        }

        public void Apply(PlugRenamed plugRenamed)
        {
            Name = plugRenamed.NewName;
        }
    }
}