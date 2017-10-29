using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugTurnedOff
    {
        public DateTimeOffset TimeStamp { get; set; }
    }
}