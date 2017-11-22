namespace SmartOutlet.Outlet.EventSourcing.Events
{
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