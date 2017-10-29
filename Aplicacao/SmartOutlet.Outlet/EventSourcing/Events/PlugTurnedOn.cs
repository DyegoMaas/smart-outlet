using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugTurnedOn
    {
        public DateTimeOffset TimeStamp { get; set; }
    }
}