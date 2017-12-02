using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
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
                    s.ConstructUsing(name => new HttpApiService(new Uri("http://localhost:6788")));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsPrompt();
                x.StartAutomatically();
                x.SetDescription("Esoft Web API Windows Print Service");
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
            _config.MapHttpAttributeRoutes();
            _config.EnableCors(); 
            _config.MaxReceivedMessageSize = 2000000;
            _config.Formatters.Clear();
            _config.Formatters.Add(new JsonMediaTypeFormatter());
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
}
