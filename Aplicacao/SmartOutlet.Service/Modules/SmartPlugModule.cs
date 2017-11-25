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
        private readonly ITimelineReporter _timelineReporter;
        private readonly ISmartPlug _smartPlug;

        public SmartPlugModule(ISmartPlug plug, 
            IConsumptionReporter consumptionReporter,
            IPlugStateReporter plugStateReporter,
            ITimelineReporter timelineReporter
        ) : base("plugs")
        {
            _smartPlug = plug;
            _consumptionReporter = consumptionReporter;
            _plugStateReporter = plugStateReporter;
            _timelineReporter = timelineReporter;

            Post("/activate", _ =>
            {
                var request = this.Bind<ActivatePlugRequest>();
                var plugId = _smartPlug.CreatePlug(request.Name);
                return new
                {
                    Name = request.Name,
                    PlugId = plugId
                };
            });
            
            Post("/credentials/reset", _ =>
            {
                _smartPlug.ResetCredentials();
                return new OkResponse();
            });
            
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
                return new OkResponse();
            });
            
            Post("/{plugId:guid}/try-turn-off", _ =>
            {
                Guid plugId = _.plugId;
                _smartPlug.TryTurnOff(plugId);
                return new OkResponse();
            });
            
            Post("/{plugId:guid}/rename", _ =>
            {
                Guid plugId = _.plugId;
                string newName = _.newName;
                _smartPlug.Rename(newName, plugId);
                return new OkResponse();
            });
            
            Post("/{plugId:guid}/scheduling/turn-on", _ =>
            {
                Guid plugId = _.plugId;
                var scheduleRequest = this.Bind<ScheduleRequest>();
                
                var scheduleCommand = new ScheduleCommand(CommandType.TurnOn, TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                _smartPlug.ScheduleTurnOn(scheduleCommand, plugId);
                return GetEstimatedActionTimeResponse(scheduleRequest, scheduleCommand.EstimatedExecutionTime);
            });
            
            Post("/{plugId:guid}/scheduling/turn-off", _ =>
            {
                Guid plugId = _.plugId;
                var scheduleRequest = this.Bind<ScheduleRequest>();

                var scheduleCommand = new ScheduleCommand(CommandType.TurnOff, TimeSpan.FromSeconds(scheduleRequest.SecondsInFuture));
                _smartPlug.ScheduleTurnOff(scheduleCommand, plugId);
                return GetEstimatedActionTimeResponse(scheduleRequest, scheduleCommand.EstimatedExecutionTime);
            });

            Get("/{plugId:guid}/reports/consumption", _ =>
            {
                Guid plugId = _.plugId;
                var report = _consumptionReporter.GetConsumptionReport(plugId).ToList();
                if (!report.Any())
                    return new {data = new string[0]};

                var firstReading = report.First().TimeStamp;
                return new
                {
                    data = report.Select(x => new
                    {
                        Power = x.ConsumptionInWatts,
                        Time = (int)(x.TimeStamp - firstReading).TotalSeconds
                    })
                };
            });
            
            Get("/{plugId:guid}/reports/timeline", _ =>
            {
                Guid plugId = _.plugId;
                var timeline = _timelineReporter.LoadTimeLine(plugId);
                return timeline.EventDescriptions;
            });
        }

        private static EstimatedActionTimeResponse GetEstimatedActionTimeResponse(ScheduleRequest scheduleRequest, DateTime future)
        {
            return new EstimatedActionTimeResponse
            {
                EstimatedActionTime = future.ToString("MM/dd/yyyy HH:mm:ss")
            };
        }

        private IList<SmartPlugResponse> GetListOfPlugStates()
        {
            return GetPlugStates();
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
                    PlugId = x.Id,
                    IsOn = x.CurrentState == PlugState.On,
                    Name = x.Name,
                    LastConsumption = x.LastConsumptionInWatts
                })
                .ToList();
        }
    }
}