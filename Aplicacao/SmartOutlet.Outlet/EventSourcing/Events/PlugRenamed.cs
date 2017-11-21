﻿using System;

namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugRenamed : IPlugEvent
    {
        public string NewName { get; set; }
        public DateTime IssuedAt { get; set; }

        public PlugRenamed(string newName)
        {
            NewName = newName;
            IssuedAt = DateTime.Now;
        }

        protected PlugRenamed()
        {
        }

    }
}