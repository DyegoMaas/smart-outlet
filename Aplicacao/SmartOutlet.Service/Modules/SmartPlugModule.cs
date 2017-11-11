using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using SmartOutlet.Outlet;
using SmartOutlet.Outlet.EventSourcing;
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
            IPlugStateReporter plugStateReporter,
            IPlugEventEmitter plugEventEmitter
        ) : base("plugs")
        {
            _smartPlug = plug;
            _consumptionReporter = consumptionReporter;
            _plugStateReporter = plugStateReporter;

            Get("/", _ => GetListOfPlugStates());
            
            Get("/{plugId:guid}", _ =>
            {
                Guid plugId = _.plugId;
                return GetPlugState(plugId);
            });
            
            Post("/{plugId:guid}/try-turn-on", _ =>
            {
                Guid plugId = _.plugId;
                _smartPlug.TryTurnOn(plugId);
                return HttpStatusCode.OK;
            });
            
            Post("/{plugId:guid}/try-turn-off", _ =>
            {
                Guid plugId = _.plugId;
                _smartPlug.TryTurnOff(plugId);
                return HttpStatusCode.OK;
            });
            
            Post("/{plugId:guid}/rename", _ =>
            {
                Guid plugId = _.plugId;
                string newName = _.newName;
                _smartPlug.Rename(newName, plugId);
                return HttpStatusCode.OK;
            });
            
            Post("/{plugId:guid}/scheduling/turn-on", _ =>
            {
                var scheduleRequest = this.Bind<ScheduleRequest>();
                _smartPlug.ScheduleTurnOn(TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                return HttpStatusCode.OK;
            });
            
            Post("/{plugId:guid}/scheduling/turn-off", _ =>
            {
                var scheduleRequest = this.Bind<ScheduleRequest>();
                _smartPlug.ScheduleTurnOff(TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                return HttpStatusCode.OK;
            });
            
            Get("/{plugId:guid}/consumption-report", _ =>
            {
                Guid plugId = _.plugId;
                return GetComsuptionReport(plugId);
            });
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

        private IList<ConsumptionInTime> GetComsuptionReport(Guid plugId)
        {
            return _consumptionReporter
                .GetConsumptionReport(plugId)
                .ToList();
        }
    }
}