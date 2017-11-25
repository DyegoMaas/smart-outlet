﻿using System;
using SmartOutlet.Outlet.EventSourcing.AggregatingRoots;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public interface ITimelineReporter
    {
        TimeLine LoadTimeLine(Guid plugId);
    }
}