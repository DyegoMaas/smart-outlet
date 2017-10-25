using Topshelf;
using Topshelf.Nancy;

namespace SmartOutlet.Service
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var host = HostFactory.New(x =>
            {
//                x.UseNLog();
	    
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(settings => new Service());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WithNancyEndpoint(x, c =>
                    {
                        c.AddHost(port: 8001, path:"smart-things/");
                        c.ConfigureNancy(hc =>
                        {
                            hc.UrlReservations.CreateAutomatically = true;
                        });
                    });
                });
                x.StartAutomatically();
                x.SetServiceName("topshelf.nancy.sampleservice");
                x.RunAsNetworkService();
            });
	
            host.Run();
        }
    }

    internal class Service
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}