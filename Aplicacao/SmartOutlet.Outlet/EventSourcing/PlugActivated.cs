using System;

namespace SmartOutlet.Outlet.EventSourcing
{
    public class PlugActivated
    {
        public Guid PlugId { get; set; }
        public string PlugName { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public PlugActivated(Guid plugId, string plugName)
        {
            PlugId = plugId;
            PlugName = plugName;
        }

        protected PlugActivated()
        {
        }
    }
}