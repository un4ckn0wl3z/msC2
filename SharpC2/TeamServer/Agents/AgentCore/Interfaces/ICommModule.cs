using AgentCore.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentCore.Interfaces
{
    public interface ICommModule
    {
        void Init(ConfigController config);
        void Run();
        void SendData();
        bool RecvData();
        void Stop();
    }
}
