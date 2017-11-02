namespace SmartOutlet.Outlet.EventSourcing.Events
{
    public class PlugRenamed
    {
        public string NewName { get; set; }

        public PlugRenamed(string newName)
        {
            NewName = newName;
        }

        protected PlugRenamed()
        {
        }
    }
}