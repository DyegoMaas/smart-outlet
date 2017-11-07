using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using SmartOutlet.Outlet;
using SmartOutlet.Outlet.EventSourcing.Reports;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Service.Modules
{
    public class SmartPlugModule : NancyModule
    {
        private readonly IConsumptionReporter _consumptionReporter;
        private readonly IPlugStateReporter _plugStateReporter;
        private readonly ISmartPlug _smartPlug;

        public SmartPlugModule(ISmartPlug plug, 
            IConsumptionReporter consumptionReporter,
            IPlugStateReporter plugStateReporter
        ) : base("plug")
        {
            _smartPlug = plug;
            _consumptionReporter = consumptionReporter;
            _plugStateReporter = plugStateReporter;

            Get("/", _ => GetListOfPlugStates());
            
            Get("/{plugId:guid}", _ => GetPlugState(_.plugId));
            
            Post("/try-turn-on", _ =>
            {
                _smartPlug.TryTurnOn(Plugs.PlugOneId);
                return HttpStatusCode.OK;
            });
            
            Post("/try-turn-off", _ =>
            {
                _smartPlug.TryTurnOff(Plugs.PlugOneId);
                return HttpStatusCode.OK;
            });
            
            Post("/scheduling/turn-on", _ =>
            {
                var scheduleRequest = this.Bind<ScheduleRequest>();
                _smartPlug.ScheduleTurnOn(TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                return HttpStatusCode.OK;
            });
            
            Post("/scheduling/turn-off", _ =>
            {
                var scheduleRequest = this.Bind<ScheduleRequest>();
                _smartPlug.ScheduleTurnOff(TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                return HttpStatusCode.OK;
            });
            
            Get("/consumption-report", _ => GetComsuptionReport());
        }

        private IList<SmartPlugResponse> GetListOfPlugStates()
        {
            var allPlugIds = new[] {Plugs.PlugOneId};
            return GetPlugStates(allPlugIds);
        }

        private SmartPlugResponse GetPlugState(Guid plugId)
        {
            return GetPlugStates(plugId).FirstOrDefault();
        }

        private IList<SmartPlugResponse> GetPlugStates(params Guid[] plugIds)
        {
            var stateReport = _plugStateReporter.GetStateReport(plugIds);
            return stateReport
                .Select(x => new SmartPlugResponse
                {
                    IsOn = x.State == PlugState.On,
                    Name = x.Name,
                    LastConsumption = x.LastConsumptionInWatts
                })
                .ToList();
        }

        private IList<ConsumptionInTime> GetComsuptionReport()
        {
            return _consumptionReporter
                .GetConsumptionReport(Plugs.PlugOneId)
                .ToList();
        }
    }
}