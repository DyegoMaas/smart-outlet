using System;

namespace SmartOutlet.Outlet
{
    public interface ISmartPlug
    {
        void TryTurnOff(Guid plugId);
        void TryTurnOn(Guid plugId);
    }
}