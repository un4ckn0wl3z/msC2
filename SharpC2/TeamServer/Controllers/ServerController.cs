using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Controllers
{
    public class ServerController
    {
        public ServerStatus ServerStatus { get; private set; }
        public ListenerControllerBase ListenerControllerBase { get; private set; }

        public ServerController()
        {
            ServerStatus = ServerStatus.Starting;
            ListenerControllerBase = new ListenerControllerBase();
        }

        public void Start()
        {
            ServerStatus = ServerStatus.Running;
            while (ServerStatus == ServerStatus.Running)
            {
                // do staff
            }
        }

        public void Stop()
        {
            ServerStatus = ServerStatus.Stopped;
        }

    }

    public enum ServerStatus
    {
        Starting,
        Running,
        Stopped
    }
}
