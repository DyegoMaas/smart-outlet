using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoAP.Util;
using SmartOutlet.Outlet.CoAP;

namespace SmartOutlet.Outlet
{
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
}