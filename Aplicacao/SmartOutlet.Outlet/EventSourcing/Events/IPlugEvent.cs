using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public interface IPlugEvent
    {
        DateTime IssuedAt { get; }
    }
}