using System;
using Emitter;
using Emitter.Messages;
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
        
        private const string  SmartThingsSecretKey = "a4cccb0a-7961-4e68-a83b-d23a5b6cc3e6";
        private readonly Connection _emitter;

        public NancyService()
        {
            _nancyHost = new NancyHost(new Uri(ServiceUri));
            _scheduler = new StdSchedulerFactory().GetScheduler();
            
            _emitter = new Emitter.Connection();
        }

        public void Start()
        {
            ConfigureEmitter();
            ConfigureNancy();
            ConfigureJob();
        }

        private void ConfigureEmitter()
        {
            _emitter.Connect();
            
            const string smartPlugChannelName = "smart-plug";
//            _emitter.GenerateKey(
//                SmartThingsSecretKey, 
//                smartPlugChannelName,
//                EmitterKeyType.ReadWrite,
//                (response) => Console.WriteLine("Generated Key: " + response.Key)
//            );

            _emitter.On(SmartThingsSecretKey, smartPlugChannelName, (channel, msg) =>
            {
                Console.WriteLine("msg: " + msg);
            });
            
            _emitter.Publish(SmartThingsSecretKey, smartPlugChannelName, "on");
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
//                .WithSimpleSchedule(x => x
//                    .WithIntervalInSeconds(10)
//                    .RepeatForever())
                .StartNow()
                .Build();
            _scheduler.ScheduleJob(job, trigger);
            _scheduler.Start();
        }

        public void Stop()
        {
            _emitter.Disconnect();
            _nancyHost.Dispose();
            _scheduler.Shutdown();
        }
    }
}