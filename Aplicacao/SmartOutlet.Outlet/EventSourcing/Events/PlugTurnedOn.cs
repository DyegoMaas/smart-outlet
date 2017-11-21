using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugTurnedOn : IPlugEvent
    {
        public DateTime IssuedAt { get; set; }

        public PlugTurnedOn(DateTime issuedAt)
        {
            IssuedAt = issuedAt;
        }

        protected PlugTurnedOn()
        {
        }
    }
}