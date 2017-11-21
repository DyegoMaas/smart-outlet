using System.Collections.Generic;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.Tests.Unit
{
    public class TimeLine
    {
        private readonly List<string> _eventDescriptions = new List<string>();
        
        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            _eventDescriptions.Add(plugTurnedOn.GetDescription());
        }

        public IEnumerable<string> GetEvents()
        {
            return _eventDescriptions;
        }

        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            _eventDescriptions.Add(plugTurnedOff.GetDescription());
        }

        public void Apply(PlugRenamed plugRenamed)
        {
            _eventDescriptions.Add(plugRenamed.GetDescription());
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
                case OperationScheduled _:
                    var agendamento = (OperationScheduled) @event;
                    return agendamento.Type == CommandType.TurnOn
                        ? $"Agendamento para ligar às {agendamento.IssuedAt + agendamento.TimeInFuture:yyyy/MM-dd HH:mm:ss}"
                        : $"Agendamento para desligar às {agendamento.IssuedAt + agendamento.TimeInFuture:yyyy/MM-dd HH:mm:ss}";
            }

            return string.Empty;
        }
    }

}