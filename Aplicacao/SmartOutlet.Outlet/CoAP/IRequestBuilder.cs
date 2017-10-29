namespace SmartOutlet.Outlet.CoAP
{
    public interface IRequestBuilder
    {
        IDetailedRequestBuilder Get(string relativeUri);
        IDetailedRequestBuilder Observe(string relativeUri);
        IDetailedRequestBuilder Put(string relativeUri);
        IDetailedRequestBuilder Post(string relativeUri);
        IDetailedRequestBuilder Discover();
    }
}