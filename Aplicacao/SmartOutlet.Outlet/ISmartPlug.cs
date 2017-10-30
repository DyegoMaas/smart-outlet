namespace SmartOutlet.Outlet
{
    public interface ISmartPlug
    {
        ToggeResult TryTurnOff();
        ToggeResult TryTurnOn();
    }
}