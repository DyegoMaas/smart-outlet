using System;
using System.Text;
using Marten;
using Nancy.Hosting.Self;
using Quartz;
using Quartz.Impl;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartOutlet.Service
{
    internal class NancyService
    {
        private const string ServiceUri = "http://localhost:8001/smart-things/";
        private readonly NancyHost _nancyHost;
        private readonly IScheduler _scheduler;
        private readonly MqttClient _mqttClient;

        public NancyService()
        {
            _nancyHost = new NancyHost(new Uri(ServiceUri));
            _scheduler = new StdSchedulerFactory().GetScheduler();
            _mqttClient = new MqttClient(
                brokerHostName:"localhost",
                brokerPort: 1883,
                secure: false,
                sslProtocol: MqttSslProtocols.None,
                userCertificateSelectionCallback: null,
                userCertificateValidationCallback: null
            ); 
        }

        public void Start()
        {
            ConfigureNancy();
            ConfigureJob();
            ConfigureMqtt();
        }
        
        public void ConfigureMqtt()
        {
            var clientId = Guid.NewGuid().ToString();
            var connected = _mqttClient.Connect(clientId);
            
            _mqttClient.MqttMsgPublished += (sender, args) =>
            {
                Console.WriteLine($"Message published {args.MessageId}");
            };

            _mqttClient.MqttMsgSubscribed += (sender, args) =>
            {
                Console.WriteLine($"Subscription: {args.MessageId}");
            };

            _mqttClient.ConnectionClosed += (sender, args) =>
            {
                Console.WriteLine("Connection closed");
            };

            _mqttClient.MqttMsgUnsubscribed += (sender, args) =>
            {
                Console.WriteLine($"Unsubscription: {args.MessageId}");
            };

            _mqttClient.MqttMsgPublishReceived += (sender, args) =>
            {
                Console.WriteLine($"message received: {Encoding.UTF8.GetString(args.Message)} in topic {args.Topic}");
            }; 
            
            var subscriptionId = _mqttClient.Subscribe(new[] { "/home/temperature" }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
 
// publish a message on "/home/temperature" topic with QoS 2 
            _mqttClient.Publish("/home/temperature", Encoding.UTF8.GetBytes("25"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true); 
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
            _mqttClient.Disconnect();
        }
    }
}