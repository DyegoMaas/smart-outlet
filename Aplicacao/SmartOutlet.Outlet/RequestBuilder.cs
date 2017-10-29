using System;
using CoAP;

namespace SmartOutlet.Outlet
{
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
}