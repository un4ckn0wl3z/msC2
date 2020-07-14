using AgentCore.Controllers;
using AgentCore.Interfaces;
using AgentCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpAgent.Modules
{
    class HttpCommModule : ICommModule
    {
        private ConfigController Config { get; set; }
        public ModuleStatus ModuleStatus { get; private set; }

        public void Init(ConfigController config)
        {
            ModuleStatus = ModuleStatus.Starting;
            Config = config;
        }

        public bool RecvData()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            ModuleStatus = ModuleStatus.Running;
            Task.Factory.StartNew(delegate() {

                while (ModuleStatus == ModuleStatus.Running)
                {
                    var sleepTime = (int)Config.GetOption(ConfigSetting.SleepTime) * 1000;
                    var jitter = (int)Config.GetOption(ConfigSetting.Jitter);

                    Thread.Sleep(sleepTime);

                    AgentCheckIn();
                }

            });
        }

        public void SendData()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        private void AgentCheckIn()
        {
            var client = new WebClient();
            client.BaseAddress = string.Format("http://{0}:{1}", Config.GetOption(ConfigSetting.ConnectHost)
                ,Config.GetOption(ConfigSetting.ConnectPort));

            client.DownloadString("/testUri");
        }
    }
}
