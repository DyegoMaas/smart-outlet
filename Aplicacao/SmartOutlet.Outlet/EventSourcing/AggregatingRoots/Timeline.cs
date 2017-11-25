﻿using System;
using System.Collections.Generic;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.AggregatingRoots
{
    public class TimeLine
    {
        public List<string> EventDescriptions = new List<string>();
        public Guid Id { get; set; }

        public void Apply(PlugActivated activation)
        {
            Id = activation.PlugId;
            InserirDescricaoDeEvento(activation.GetDescription());
        }

        public void Apply(PlugTurnedOn plugTurnedOn)
        {
            InserirDescricaoDeEvento(plugTurnedOn.GetDescription());
        }

        public void Apply(PlugTurnedOff plugTurnedOff)
        {
            InserirDescricaoDeEvento(plugTurnedOff.GetDescription());
        }

        public void Apply(OperationScheduled scheduling)
        {
            InserirDescricaoDeEvento(scheduling.GetDescription());
        }

        public void Apply(PlugRenamed plugRenamed)
        {
            InserirDescricaoDeEvento(plugRenamed.GetDescription());
        }

        private void InserirDescricaoDeEvento(string descricao)
        {
            EventDescriptions.Insert(0, descricao);
        }
    }
}