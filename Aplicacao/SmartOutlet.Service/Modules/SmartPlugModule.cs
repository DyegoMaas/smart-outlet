using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using SmartOutlet.Outlet;
using SmartOutlet.Outlet.EventSourcing.Reports;

namespace SmartOutlet.Service.Modules
{
    public class SmartPlugModule : NancyModule
    {
        private readonly IConsumptionReporter _consumptionReporter;
        private readonly ISmartPlug _smartPlug;

        public SmartPlugModule(ISmartPlug plug, IConsumptionReporter consumptionReporter) 
            : base("plug")
        {
            _smartPlug = plug;
            _consumptionReporter = consumptionReporter;
            Get("/", _ => GetState());
            Post("/turn-on", _ => TurnPlugOn());
            Post("/turn-off", _ => TurnPlugOff());
            Get("/consumption-report", _ => GetComsuptionReport());
        }

        private static SmartPlugResponse GetState()
        {
            return new SmartPlugResponse
            {
                IsOn = Convert.ToBoolean(new Random().Next(0, 2)),
                Name = "Plug 1"
            };
        }

        private PlugState TurnPlugOn()
        {
            ToggeResult result = _smartPlug.TryTurnOn();
            return result.State;
        }

        private PlugState TurnPlugOff()
        {
            ToggeResult result = _smartPlug.TryTurnOff();
            return result.State;
        }

        private IList<ConsumptionInTime> GetComsuptionReport()
        {
            return _consumptionReporter
                .GetConsumptionReport(Plugs.PlugOneId)
                .ToList();
        }
    }

    public class SmartPlugResponse
    {
        public bool IsOn { get; set; }
        public string Name { get; set; }
    }
}