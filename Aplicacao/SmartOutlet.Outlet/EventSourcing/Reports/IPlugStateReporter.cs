using System;
using System.Collections.Generic;

namespace SmartOutlet.Outlet.EventSourcing.Reports
{
    public interface IPlugStateReporter
    {
        IEnumerable<PlugHistory> GetStateReport(params Guid[] plugIds);
    }
}