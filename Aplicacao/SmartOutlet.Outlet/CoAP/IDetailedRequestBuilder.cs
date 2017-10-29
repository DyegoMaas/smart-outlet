using System;
using CoAP;

namespace SmartOutlet.Outlet.CoAP
{
    public interface IDetailedRequestBuilder
    {
        IDetailedRequestBuilder WithTextPayload(string payload);
        IDetailedRequestBuilder WithCallback(Action<object, ResponseEventArgs> callback);
        Request Build();
    }
}