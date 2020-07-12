using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using static TeamServer.Listeners.ListenerBase;

namespace TeamServer.Controllers
{
    public class TcpListenerController
    {
        public List<ListenerTcp> TCPListeners { get; set; } = new List<ListenerTcp>();

        public ListenerTcp StartTcpListener(NewTcpListenerRequest request)
        {
            var listener = new ListenerTcp
            {
                BindAddress = request.BindAddress,
                BindPort = request.BindPort,
                Type = ListenerType.Tcp
            };

            TCPListeners.Add(listener);

            return listener;

        }

        public List<ListenerTcp> GetTcpListeners()
        {
            var result = new List<ListenerTcp>();

            foreach (var listener in TCPListeners)
            {
                result.Add(listener);
            }

            return result;
        }

        public bool StopTcpListener(string listenerId)
        {
            var tcpListener = TCPListeners.Where(l => l.ListenderId.Equals(listenerId, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (tcpListener != null)
            {
                TCPListeners.Remove(tcpListener);

                return true;
            }

            return false;

        }


    }
}
