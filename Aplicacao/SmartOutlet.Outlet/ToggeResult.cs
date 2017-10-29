namespace SmartOutlet.Outlet
{
    public class ToggeResult
    {
        public PlugState State { get; }

        public ToggeResult(PlugState state)
        {
            State = state;
        }
    }
}