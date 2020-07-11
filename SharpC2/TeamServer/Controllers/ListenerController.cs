/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using TeamServer.Modules;

namespace TeamServer.Controllers
{
    public class ListenerController
    {
        public List<HTTPCommModule> HTTPListeners { get; set; } = new List<HTTPCommModule>();

        public List<ListenerTcp> TCPListeners { get; set; } = new List<ListenerTcp>();

        public void StartHttpListener(NewHttpListenerRequest request)
        {
            var listener = new ListenerHttp
            {
                BindPort = request.BindPort,
                ConnectAddress = request.ConnectAddress,
                ConnectPort = request.ConnectPort
            };

            var module = new HTTPCommModule
            {
                Listener = listener
            };
            HTTPListeners.Add(module);
            module.Init();
            module.Start();

        }

        public void StartTcpListener(NewTcpListenerRequest request)
        {
            var listener = new ListenerTcp
            {
                BindAddress = request.BindAddress,
                BindPort = request.BindPort
            };

            TCPListeners.Add(listener);

        }

        public IEnumerable<ListenerBase> GetListeners()
        {
            var result = new List<ListenerBase>();

            foreach (var listener in TCPListeners)
            {
                result.Add(listener);
            }

            foreach (var module in HTTPListeners)
            {
                result.Add(module.Listener);
            }

            return result;
        }

        public void StopListener(string listenerId)
        {
            var tcpListener = TCPListeners.Where(l => l.ListenderId.Equals(listenerId, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (tcpListener != null)
            {
                TCPListeners.Remove(tcpListener);
            }
            else
            {
                var httpModule = HTTPListeners.Where(l => l.Listener.ListenderId
                .Equals(listenerId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (httpModule != null)
                {
                    httpModule.Stop();
                    HTTPListeners.Remove(httpModule);
                }

            }

        }


    }
}
*/