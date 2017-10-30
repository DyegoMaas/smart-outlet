﻿using System;
using Nancy;
using SmartOutlet.Outlet;

namespace SmartOutlet.Service.Modules
{
    public class SmartPlugModule : NancyModule
    {
//        public SmartPlugModule(ISmartPlug plug)
        public SmartPlugModule()
            : base("plug")
        {
            Get("/", _ => GetState());
//            Post("/turn-on", _ => TurnPlugOn(plug));
//            Post("/turn-off", _ => TurnPlugOff(plug));
        }

        private static SmartPlugResponse GetState()
        {
            return new SmartPlugResponse
            {
                IsOn = Convert.ToBoolean(new Random().Next(0, 2)),
                Name = "Plug 1"
            };
        }

        private static PlugState TurnPlugOn(ISmartPlug plug)
        {
            ToggeResult result = plug.TurnOn();
            return result.State;
        }

        private static PlugState TurnPlugOff(ISmartPlug plug)
        {
            ToggeResult result = plug.TurnOff();
            return result.State;
        }
    }

    public class SmartPlugResponse
    {
        public bool IsOn { get; set; }
        public string Name { get; set; }
    }
}