using Nancy;
using Nancy.TinyIoc;
using SmartOutlet.Outlet;

namespace SmartOutlet.Service
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<ISmartPlug, SmartPlug>();
        }
    }
}