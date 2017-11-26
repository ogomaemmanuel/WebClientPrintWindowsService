using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Topshelf;

namespace WebApiPrintService
{
    class Program 
    {
        static void Main(string[] args)
        {

            HostFactory.Run(x =>
            {
                x.Service<HttpApiService>(s =>
                {
                    //s.("Piotr.WebApiTopShelfService");
                    s.ConstructUsing(name => new HttpApiService(new Uri("http://localhost:6784")));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription("Sample Web API Windows service");
                x.SetDisplayName("EsoftWindowsPrintService");
                x.SetServiceName("EsoftWindowsPrintService");
            });

        }

        
    }
    public class HttpApiService
    {
        private readonly HttpSelfHostServer _server;
        private readonly HttpSelfHostConfiguration _config;
        private const string EventSource = "HttpApiService";

        public HttpApiService(Uri address)
        {
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, "Application");
            }
            EventLog.WriteEntry(EventSource,
                String.Format("Creating server at {0}",
                address.ToString()));
            _config = new HttpSelfHostConfiguration(address);
            _config.MessageHandlers.Add(new CustomHeaderHandler());
            _config.MapHttpAttributeRoutes();
            _config.Routes.MapHttpRoute("DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );
            _server = new HttpSelfHostServer(_config);
        }

        public void Start()
        {
            //EventLog.WriteEntry(EventSource, "Opening HttpApiService server.");
            _server.OpenAsync();
        }

        public void Stop()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
        }
    }

    public class CustomHeaderHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith((task) =>
                {
                    HttpResponseMessage response = task.Result;
                    response.Headers.Add("Access-Control-Allow-Origin", "*");
                    return response;
                });
        }
    }
}
