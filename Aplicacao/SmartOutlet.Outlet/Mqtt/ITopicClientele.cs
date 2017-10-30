using System;

namespace SmartOutlet.Outlet.Mqtt
{
    public interface ITopicClientele
    {
        void Subscribe(string topic, Action<string> onMessageReceived);
        void Subscribe(string[] topics, Action<string> onMessageReceived);
    }
}