using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoAP;
using CoAP.Util;

namespace SmartOutlet.Outlet
{
    public interface ISmartPlug
    {
        ToggeResult TurnOff();
        ToggeResult TurnOn();
    }

    public class SmartPlug : ISmartPlug
    {
        //TODO tornar configurável
        private readonly Uri _baseUri = new Uri("coap://localhost:10.0.0.5");
        
        public ToggeResult TurnOff()
        {
            return new ToggeResult(PlugState.Off);
        }

        public ToggeResult TurnOn()
        {
            var taskCompletionSource = new TaskCompletionSource<PlugState>();
                
                const string toggleOnPayload = "1";
                var post = RequestBuilder.New(_baseUri)
                    .Post("toggle")
                    .WithTextPayload(toggleOnPayload)
                    .WithCallback((sender, e) =>
                    {
                        var response = e.Response;
                        if (response == null)
                        {
                            //Timeout
//                        Console.WriteLine("Request timeout");
                            taskCompletionSource.SetResult(PlugState.Unknown);
                        }
                        else
                        {
//                        Console.WriteLine(Utils.ToString(response));
//                        Console.WriteLine("Time (ms): " + response.RTT);
                            PlugState result = ParsePlugResponse(response.PayloadString);
                            taskCompletionSource.SetResult(result);
                        }
                    })
                    .Build();              
                post.Send();
            Task.WaitAll(taskCompletionSource.Task);

            return new ToggeResult(taskCompletionSource.Task.Result);
        }

        private PlugState ParsePlugResponse(string payload)
        {
            switch (payload.Trim().ToLower())
            {
                case "on": return PlugState.On;
                case "off": return PlugState.Off;
                default: return PlugState.Unknown;
            }
        }
    }

    public class ToggeResult
    {
        public PlugState State { get; }

        public ToggeResult(PlugState state)
        {
            State = state;
        }
    }

    public enum PlugState
    {
        Off = 0,
        On = 1,
        Unknown = 2
    }

//    public class CoapResquestFactory : ICoapRequestFactory
//    {
//        private readonly Uri _baseUri = new Uri("coap://localhost:10.0.0.5");
//
//        public Request NewPost(string relativeUri)
//        {
//            var postRequest = Request.NewPost();
//            ConfigureRequest(postRequest, relativeUri);
//            return postRequest;
//        }
//
//        public Request NewPut(string relativeUri)
//        {
//            var putRequest = Request.NewPut();
//            ConfigureRequest(putRequest, relativeUri);
//            return putRequest;
//        }
//
//        public Request NewDelete(string relativeUri)
//        {
//            return Request.NewDelete();
//        }
//
//        public Request NewGet(string relativeUri)
//        {
//            return Request.NewGet();
//        }
//
//        public Request NewObserve(string relativeUri)
//        {
//            var getRequest = NewGet(relativeUri);
//            getRequest.MarkObserve();
//            
//            return getRequest;
//        }
//
//        public Request NewDiscovery()
//        {
//            var discoveryRequest = NewGet("");
//            discoveryRequest.URI = new Uri(_baseUri, "/.well-known/core");
//            
//            return discoveryRequest;
//        }
//
//        private void ConfigureRequest(Request request, string relativeUri)
//        {
//            request.URI = new Uri(_baseUri, relativeUri);;
//        }
//    }

    public class RequestBuilder : IRequestBuilder, IDetailedRequestBuilder
    {
        private Uri _baseUri;
        private string _relativeUri;
        private Request _request;
        private string _payload;
        private int _mediaType;
        private Action<object, ResponseEventArgs> _callback;

        private RequestBuilder()
        {
        }

        public static IRequestBuilder New(Uri baseUri)
        {
            var requestBuilder = new RequestBuilder {_baseUri = baseUri};
            return requestBuilder;
        }

        public IDetailedRequestBuilder WithTextPayload(string payload)
        {
            _payload = payload;
            _mediaType = MediaType.TextPlain;
            return this;
        }

        public IDetailedRequestBuilder WithCallback(Action<object, ResponseEventArgs> callback)
        {
            _callback = callback;
            return this;
        }

        public Request Build()
        {
            _request.URI = new Uri(_baseUri, _relativeUri);
            
            if (!string.IsNullOrWhiteSpace(_payload))
                _request.SetPayload(_payload, _mediaType);

            if (_callback != null)
                _request.Respond += (sender, e) => _callback(sender, e);
            
            return _request;
        }

        public IDetailedRequestBuilder Get(string relativeUri)
        {
            _request = Request.NewGet();
            _relativeUri = relativeUri;
            return this;
        }

        public IDetailedRequestBuilder Observe(string relativeUri)
        {
            _request = Request.NewGet();
            _request.MarkObserve();
            _relativeUri = relativeUri;
            return this;
        }

        public IDetailedRequestBuilder Post(string relativeUri)
        {
            _request = Request.NewPost();
            _relativeUri = relativeUri;
            return this;
        }

        public IDetailedRequestBuilder Put(string relativeUri)
        {
            _request = Request.NewPut();
            _relativeUri = relativeUri;
            return this;
        }

        public IDetailedRequestBuilder Discover()
        {
            _request = Request.NewPut();
            _relativeUri = "/.well-known/core";
            return this;
        }
    }

    public interface IRequestBuilder
    {
        IDetailedRequestBuilder Get(string relativeUri);
        IDetailedRequestBuilder Observe(string relativeUri);
        IDetailedRequestBuilder Put(string relativeUri);
        IDetailedRequestBuilder Post(string relativeUri);
        IDetailedRequestBuilder Discover();
    }

    public interface IDetailedRequestBuilder
    {
        IDetailedRequestBuilder WithTextPayload(string payload);
        IDetailedRequestBuilder WithCallback(Action<object, ResponseEventArgs> callback);
        Request Build();
    }

//    public interface ICoapRequestFactory
//    {
//        Request NewPost(string relativeUri);
//        Request NewPut(string relativeUri);
//        Request NewDelete(string relativeUri);
//
//        Request NewDiscovery();
//        
//        Request NewGet(string relativeUri);
//        Request NewObserve(string relativeUri);
//    }
}