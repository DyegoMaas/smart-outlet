using System;
using CoAP;

namespace SmartOutlet.Outlet
{
    public interface IDetailedRequestBuilder
    {
        IDetailedRequestBuilder WithTextPayload(string payload);
        IDetailedRequestBuilder WithCallback(Action<object, ResponseEventArgs> callback);
        Request Build();
    }
}