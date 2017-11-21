﻿using System;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.Mqtt;

namespace SmartOutlet.Outlet
{
    public class SmartPlug : ISmartPlug
    {
        private readonly IPublisher _publisher;
        private readonly IPlugEventSequencer _plugEventSequencer;

        public SmartPlug(IPublisher publisher, IPlugEventSequencer plugEventSequencer)
        {
            _publisher = publisher;
            _plugEventSequencer = plugEventSequencer;
        }

        public Guid CreatePlug(string name)
        {
            var plugId = Guid.NewGuid();
            _plugEventSequencer.PlugActivated(plugId, name);
            
            var payload = $"{plugId}";
            _publisher.Publish("/smart-plug/activate", payload);
            
            return plugId;
        }

        public void Rename(string newName, Guid plugId)
        {
            _plugEventSequencer.PlugRenamed(newName, plugId);
        }

        public void TryTurnOff(Guid plugId)
        {
            _publisher.Publish("/smart-plug/state", $"{plugId}|turn-off");
        }

        public void TryTurnOn(Guid plugId)
        {
            _publisher.Publish("/smart-plug/state", $"{plugId}|turn-on");
        }

        public void ScheduleTurnOn(ScheduleCommand command, Guid plugId)
        {
            _publisher.Publish("/smart-plug/schedule-on", $"{plugId}|{GetMilisecondsString(command.TimeInFuture)}");
        }

        public void ScheduleTurnOff(ScheduleCommand command, Guid plugId)
        {
            _publisher.Publish("/smart-plug/schedule-off", $"{plugId}|{GetMilisecondsString(command.TimeInFuture)}");
            _plugEventSequencer.ActionScheduled(command, plugId);
        }

        private static string GetMilisecondsString(TimeSpan timeInFuture)
        {
            return ((int)timeInFuture.TotalMilliseconds).ToString();
        }
    }
}