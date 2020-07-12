using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Controllers.Listeners;
using TeamServer.Listeners;
using TeamServer.Models;
using static TeamServer.Listeners.ListenerBase;

namespace TeamServer.Controllers
{
    public class ListenerControllerBase
    {
        private HttpListenerController HttpListenerController { get; set; } = new HttpListenerController();
        private TcpListenerController TcpListenerController { get; set; } = new TcpListenerController();

        public ListenerHttp StartHttpListener(NewHttpListenerRequest request)
        {
            return HttpListenerController.StartHttpListener(request);
        }

        public ListenerTcp StartTcpListener(NewTcpListenerRequest request)
        {
            return TcpListenerController.StartTcpListener(request);
        }

        public IEnumerable<ListenerBase> GetListeners()
        {
            var result = new List<ListenerBase>();

            foreach (var tcpListener in TcpListenerController.GetTcpListeners())
            {
                result.Add(tcpListener);
            }

            foreach (var httpListener in HttpListenerController.GetHttpListeners())
            {
                result.Add(httpListener);
            }

            return result;
        }


        public bool StopListener(string listenerId, ListenerType type)
        {
            if (ListenerType.Http.Equals(type))
            {
                return HttpListenerController.StopHttpListener(listenerId);
            }
            else if (ListenerType.Tcp.Equals(type))
            {
                return TcpListenerController.StopTcpListener(listenerId);

            }

            return false;


        }



    }
}
