using System;
using System.Collections.Generic;

namespace SmartOutlet.Outlet
{
    public interface ISmartPlug
    {
        ToggeResult TurnOff();
        ToggeResult TurnOn();
    }

    public class SmartPlug : ISmartPlug
    {
        public ToggeResult TurnOff()
        {
            return new ToggeResult(PlugState.Off);
        }

        public ToggeResult TurnOn()
        {
            return new ToggeResult(PlugState.On);
        }
    }

    public class ToggeResult
    {
        public PlugState State { get; }

        public ToggeResult(PlugState state)
        {
            State = state;
        }
    }

    public enum PlugState
    {
        Off = 0,
        On = 1
    }
}