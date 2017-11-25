using System;
using System.Collections.Generic;
using SmartOutlet.Outlet.EventSourcing.ProjectionViews;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public interface IPlugStateReporter
    {
        IEnumerable<Plug> GetStateReport(params Guid[] plugIds);
    }
}