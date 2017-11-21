using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class OperationScheduled : IPlugEvent
    {
        public CommandType Type { get; set; }
        public DateTime IssuedAt { get; set; }
        public TimeSpan TimeInFuture { get; set; }

        public OperationScheduled(CommandType type, DateTime issuedAt, TimeSpan timeInFuture)
        {
            Type = type;
            IssuedAt = issuedAt;
            TimeInFuture = timeInFuture;
        }

        protected OperationScheduled()
        {
        }
    }
}