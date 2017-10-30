using System;
using Marten;
using Nancy.Hosting.Self;
using Quartz;
using Quartz.Impl;

namespace SmartOutlet.Service
{
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