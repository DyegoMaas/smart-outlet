using Marten.Events;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public static class PlugEventExtensions
    {
        public static string GetDescription<T>(this Event<T> @event) 
            where T : IPlugEvent
        {
            return $"{@event.Timestamp:yyyy/MM/dd HH:mm:ss} - {DescribeEvent(@event, @event.Data)}";
        }

        private static string DescribeEvent<T>(Event<T> martenEvent, IPlugEvent @event)
            where T : IPlugEvent 
        {
            switch (@event)
            {
                case PlugActivated _:
                    return "Plugue ativado";
                case PlugDeactivated _:
                    return "Plugue desativado";
                case PlugRenamed _:
                    return "Plugue renomeado para " + ((PlugRenamed) @event).NewName;
                case PlugTurnedOn _:
                    return "Plugue ligado";
                case PlugTurnedOff _:
                    return "Plugue desligado";
                case OperationScheduled agendamento:
                    return agendamento.Type == CommandType.TurnOn
                        ? $"Ligar em {(int) agendamento.TimeInFuture.TotalSeconds}s ({martenEvent.Timestamp + agendamento.TimeInFuture:yyyy/MM/dd HH:mm:ss})"
                        : $"Desligar em {(int) agendamento.TimeInFuture.TotalSeconds}s ({martenEvent.Timestamp + agendamento.TimeInFuture:yyyy/MM/dd HH:mm:ss})";
            }

            return string.Empty;
        }
    }
}