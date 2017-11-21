using System;

namespace SmartOutlet.Outlet
{
    public class ScheduleCommand
    {
        public DateTime IssuedAt { get; }
        public DateTime EstimatedExecutionTime { get; }
        public CommandType Type { get; }
        public TimeSpan TimeInFuture { get; }
        public string Description => EstimatedExecutionTime.ToString("MM/dd/yyyy HH:mm:ss");

        public ScheduleCommand(CommandType type, TimeSpan timeInFuture)
        {
            Type = type;
            IssuedAt = DateTime.Now;
            TimeInFuture = timeInFuture;
            EstimatedExecutionTime = IssuedAt + timeInFuture;
        }
    }
}