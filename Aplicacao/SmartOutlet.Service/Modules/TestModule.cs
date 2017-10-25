using Nancy;

namespace SmartOutlet.Service.Modules
{
    public class TestModule : NancyModule
    {
        public TestModule() : base("test")
        {
            Get("/hello-world", _ => "hello world");
        }
    }
}