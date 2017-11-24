using System.Collections.Generic;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.AggregatingRoots
{
    public class TimeLine
    {
        private readonly List<string> _eventDescriptions = new List<string>();

        public void Apply(PlugActivated activation)
        {
            InserirDescricaoDeEvento(activation.GetDescription());
        }

        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            InserirDescricaoDeEvento(plugTurnedOn.GetDescription());
        }

        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            InserirDescricaoDeEvento(plugTurnedOff.GetDescription());
        }

        public void Apply(OperationScheduled scheduling)
        {
            InserirDescricaoDeEvento(scheduling.GetDescription());
        }

        public void Apply(PlugRenamed plugRenamed)
        {
            InserirDescricaoDeEvento(plugRenamed.GetDescription());
        }

        private void InserirDescricaoDeEvento(string descricao)
        {
            _eventDescriptions.Insert(0, descricao);
        }

        public IEnumerable<string> GetEvents()
        {
            return _eventDescriptions;
        }
    }
}