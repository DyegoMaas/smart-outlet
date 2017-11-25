﻿using System;
using System.Collections.Generic;
using Marten.Events;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.EventSourcing.ProjectionViews
{
    public class TimeLine
    {
        public List<string> EventDescriptions = new List<string>();
        public Guid Id { get; set; }

        public void Apply(Event<PlugActivated> activation)
        {
            Id = activation.Data.PlugId;
            InserirDescricaoDeEvento(activation.GetDescription());
        }

        public void Apply(Event<PlugTurnedOn> plugTurnedOn)
        {
            InserirDescricaoDeEvento(plugTurnedOn.GetDescription());
        }

        public void Apply(Event<PlugTurnedOff> plugTurnedOff)
        {
            InserirDescricaoDeEvento(plugTurnedOff.GetDescription());
        }

        public void Apply(Event<OperationScheduled> scheduling)
        {
            InserirDescricaoDeEvento(scheduling.GetDescription());
        }

        public void Apply(Event<PlugRenamed> plugRenamed)
        {
            InserirDescricaoDeEvento(plugRenamed.GetDescription());
        }
        
        public void Apply(Event<PlugDeactivated> deactivation)
        {
            InserirDescricaoDeEvento(deactivation.GetDescription());
        }

        private void InserirDescricaoDeEvento(string descricao)
        {
            EventDescriptions.Insert(0, descricao);
        }
    }
}