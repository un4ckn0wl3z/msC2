using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using TeamServer.Modules;
using static TeamServer.Listeners.ListenerBase;

namespace TeamServer.Controllers.Listeners
{
    public class HttpListenerController
    {
        public List<HTTPCommModule> HTTPListeners { get; set; } = new List<HTTPCommModule>();

        public ListenerHttp StartHttpListener(NewHttpListenerRequest request)
        {
            var listener = new ListenerHttp
            {
                BindPort = request.BindPort,
                ConnectAddress = request.ConnectAddress,
                ConnectPort = request.ConnectPort,
                Type = ListenerType.Http
            };

            var module = new HTTPCommModule
            {
                Listener = listener
            };
            HTTPListeners.Add(module);
            module.Init();
            module.Start();

            return listener;

        }

        public List<ListenerHttp> GetHttpListeners()
        {
            var result = new List<ListenerHttp>();

            foreach (var module in HTTPListeners)
            {
                result.Add(module.Listener);
            }

            return result;
        }

        public bool StopHttpListener(string listenerId)
        {

            var httpModule = HTTPListeners.Where(l => l.Listener.ListenderId
            .Equals(listenerId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (httpModule != null)
            {
                httpModule.Stop();
                HTTPListeners.Remove(httpModule);
                return true;
            }
            return false;

        }


    }
}
