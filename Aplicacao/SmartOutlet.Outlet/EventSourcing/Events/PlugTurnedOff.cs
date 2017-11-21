using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugTurnedOff : IPlugEvent
    {
        public DateTime IssuedAt { get; set; }

        public PlugTurnedOff(DateTime issuedAt)
        {
            IssuedAt = issuedAt;
        }

        protected PlugTurnedOff()
        {
        }
    }
}