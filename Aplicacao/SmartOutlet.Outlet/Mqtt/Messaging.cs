using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Polly;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartOutlet.Outlet.Mqtt
{
    /*Public brokers:
        broker.hivemq.com (port 1883).
        broker.mqttdashboard.com (port 1883).
        iot.eclipse.org (port 1883).
        test.mosca.io (port 1883).
     */
    public class Messaging : IPublisher, ITopicClientele, IDisposable
    {
        private const byte QosLevel = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
        private static readonly Dictionary<string, List<Action<string>>> CallbacksPerTopic = new Dictionary<string, List<Action<string>>>();
        private readonly MqttClient _mqttClient;

        public Messaging()
        {
            const string brokerHostName = "iot.eclipse.org";
            const int brokerPort = 1883;
            _mqttClient = new MqttClient(
                brokerHostName:brokerHostName,
                brokerPort: brokerPort,
                secure: false,
                sslProtocol: MqttSslProtocols.None,
                userCertificateSelectionCallback: null,
                userCertificateValidationCallback: null
            ); 
            
            var clientId = Guid.NewGuid().ToString();
            Policy.Handle<Exception>()
                .WaitAndRetry(5, 
                    count => TimeSpan.FromMinutes(3), 
                    (exception, retryCount) =>
                    {
                        Console.WriteLine($"Not able to connect to MQTT broker. Retrying for the {retryCount} time");
                        Thread.Sleep(3000);        
                    })
                .Execute(() =>
                {
                    _mqttClient.Connect(clientId);
                    if (_mqttClient.IsConnected) 
                        return;
                    
                    var message = $"Not connected to MQTT broker '{brokerHostName}' on port '{brokerPort}'";
                    throw new InvalidOperationException(message);
                });
            
            _mqttClient.MqttMsgPublished += (sender, args) =>
            {
                Console.WriteLine($"Message published: {args.MessageId}");
            };

            _mqttClient.MqttMsgPublishReceived += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message);
                Console.WriteLine($"Message publish received: {message} in topic {args.Topic}");
                
                if (CallbacksPerTopic.ContainsKey(args.Topic))
                {
                    var callbacks = CallbacksPerTopic[args.Topic];
                    foreach (var callback in callbacks)
                    {
                        callback.Invoke(message);
                    }
                }
            };

            _mqttClient.ConnectionClosed += (sender, args) =>
            {
                Console.WriteLine("MQTT connection closed.");
            };

            _mqttClient.MqttMsgUnsubscribed += (sender, args) =>
            {
                Console.WriteLine($"MQTT unsubscription: {args.MessageId}");
            };
            
            _mqttClient.MqttMsgSubscribed += (sender, args) =>
            {
                Console.WriteLine($"MQTT subscription: {args.MessageId}");
            };
        }

        public void Publish(string topic, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _mqttClient.Publish(topic, bytes, QosLevel, true);
        }

        public void Subscribe(string topic, Action<string> onMessageReceived)
        {
            Subscribe(new [] {topic}, onMessageReceived);           
        }
        
        public void Subscribe(string[] topics, Action<string> onMessageReceived)
        {
            foreach (var topic in topics)
            {
                RegisterCallback(onMessageReceived, topic);
            }
            _mqttClient.Subscribe(topics, new[] { QosLevel });            
        }

        private static void RegisterCallback(Action<string> onMessageReceived, string topic)
        {
            if (!CallbacksPerTopic.ContainsKey(topic))
                CallbacksPerTopic.Add(topic, new List<Action<string>>());

            CallbacksPerTopic[topic].Add(onMessageReceived);
        }

        public void Dispose()
        {
            _mqttClient.Disconnect();
        }
    }
}