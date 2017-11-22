using System.Collections.Generic;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.AggregationRoots
{
    public class TimeLine
    {
        private readonly List<string> _eventDescriptions = new List<string>();
        
        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            InserirDescricaoDeEvento(plugTurnedOn.GetDescription());
        }

        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            InserirDescricaoDeEvento(plugTurnedOff.GetDescription());
        }

        public void Apply(PlugRenamed plugRenamed)
        {
            InserirDescricaoDeEvento(plugRenamed.GetDescription());
        }

        public void Apply(PlugActivated activation)
        {
            InserirDescricaoDeEvento(activation.GetDescription());
        }

        public void Apply(OperationScheduled scheduling)
        {
            InserirDescricaoDeEvento(scheduling.GetDescription());
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

    public static class PlugEventExtensions
    {
        public static string GetDescription(this IPlugEvent @event)
        {
            return $"{@event.IssuedAt:yyyy/MM/dd HH:mm:ss} - {DescribeEvent(@event)}";
        }

        private static string DescribeEvent(IPlugEvent @event)
        {
            switch (@event)
            {
                case PlugActivated _:
                    return "Plugue ativado";
                case PlugRenamed _:
                    return "Plugue renomeado para " + ((PlugRenamed) @event).NewName;
                case PlugTurnedOn _:
                    return "Plugue ligado";
                case PlugTurnedOff _:
                    return "Plugue desligado";
                case OperationScheduled agendamento:
                    return agendamento.Type == CommandType.TurnOn
                        ? $"Ligar em {(int) agendamento.TimeInFuture.TotalSeconds}s ({agendamento.IssuedAt + agendamento.TimeInFuture:yyyy/MM/dd HH:mm:ss})"
                        : $"Desligar em {(int) agendamento.TimeInFuture.TotalSeconds}s ({agendamento.IssuedAt + agendamento.TimeInFuture:yyyy/MM/dd HH:mm:ss})";
            }

            return string.Empty;
        }
    }

}