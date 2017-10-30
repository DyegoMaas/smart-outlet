using System;
using Nancy.Hosting.Self;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace SmartOutlet.Service
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var host = HostFactory.New(x =>
            {
//                x.UseNLog();
	    
                x.Service<NancyService>(s =>
                {
                    s.ConstructUsing(settings => new NancyService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    
//                    s.WithNancyEndpoint(x, c =>
//                    {
//                        c.AddHost(port: 8001/*, path:"smart-things/"*/);
//                        c.ConfigureNancy(hc =>
//                        {
//                            hc.UrlReservations.CreateAutomatically = true;
//                        });
//                    });
                    
//                    s.ScheduleQuartzJob(q =>
//                        q.WithJob(() =>
//                                JobBuilder.Create<MyJob>().Build())
//                            .AddTrigger(() => TriggerBuilder.Create()
//                                .WithSimpleSchedule(b => b
//                                    .WithIntervalInSeconds(10)
//                                    .RepeatForever())
//                                .Build()));
                });
                x.StartAutomatically();
                x.SetServiceName("topshelf.nancy.sampleservice");
                x.SetDisplayName("SmartThings");
                x.SetDescription("SmartThings - plugue inteligente");
                x.RunAsNetworkService();
            });
	
            host.Run();
        }
    }

    internal class MyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            int a = 0;
        }
    }

    internal class NancyService
    {
        private const string ServiceUri = "http://localhost:8001/smart-things/";
        private readonly NancyHost _nancyHost;
        private readonly IScheduler _scheduler;

        public NancyService()
        {
            _nancyHost = new NancyHost(new Uri(ServiceUri));
            _scheduler = new StdSchedulerFactory().GetScheduler();
        }

        public void Start()
        {
            ConfigureNancy();
            ConfigureJob();
        }

        private void ConfigureNancy()
        {
            _nancyHost.Start();
            Console.WriteLine($"Running on {ServiceUri}/");
        }

        private void ConfigureJob()
        {
            var job = JobBuilder.Create<MyJob>().Build();
            var trigger = TriggerBuilder
                .Create()
                .StartNow()
                .Build();
            _scheduler.ScheduleJob(job, trigger);
            _scheduler.Start();
        }

        public void Stop()
        {
            _nancyHost.Dispose();
            _scheduler.Shutdown();
        }
    }
}