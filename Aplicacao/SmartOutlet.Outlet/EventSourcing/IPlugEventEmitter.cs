using System;

namespace SmartOutlet.Outlet.EventSourcing
{
    public interface IPlugEventEmitter
    {
        void NewConsumption(Guid plugId, double consumptionInWatts);
        void PlugTurnedOn(Guid plugId);
        void PlugTurnedOff(Guid plugId);
        void PlugRenamed(string newName, Guid plugId);
    }
}