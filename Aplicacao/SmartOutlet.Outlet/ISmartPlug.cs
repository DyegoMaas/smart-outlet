using System;

namespace SmartOutlet.Outlet
{
    public interface ISmartPlug
    {
        void TryTurnOff(Guid plugId);
        void TryTurnOn(Guid plugId);
        void ScheduleTurnOn(TimeSpan timeInFuture);
        void ScheduleTurnOff(TimeSpan timeInFuture);
        void Rename(string newName, Guid plugId);
        Guid CreatePlug(string name);
    }
}