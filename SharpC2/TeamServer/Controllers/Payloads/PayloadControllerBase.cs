using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using static TeamServer.Listeners.ListenerBase;

namespace TeamServer.Controllers
{
    public class PayloadControllerBase
    {
        public static string RootDirectory { get; set; } = Assembly.GetExecutingAssembly().Location.Split("TeamServer")[0];
        public static string AgentDirectory { get; set; } = RootDirectory + "TeamServer" + Path.DirectorySeparatorChar + "Agents";
        public static string ReferencesDirectory { get; set; } = AgentDirectory + Path.DirectorySeparatorChar + "References";

        public static PayloadResponse GenerateAgentPayload(PayloadRequest request)
        {
            var response = new PayloadResponse();
            var listener = GetListener(request.ListenerId);

            if (listener == null)
            {
                response.CompilerStatus = CompilerStatus.Fail;
                response.ErrorMessage = "Invalid Listener";
                return response;
            }

            byte[] payload = default;

            try
            {
                if (listener.Type == ListenerType.Http)
                {
                    var controller = new HttpPayloadController(listener as ListenerHttp);
                    payload = controller.GenerateHttpPayload(request);
                }
                else if (listener.Type == ListenerType.Tcp)
                {

                }
            }
            catch (Exception e)
            {
                response.CompilerStatus = CompilerStatus.Fail;
                response.ErrorMessage = e.Message;
                return response;

            }
            response.CompilerStatus = CompilerStatus.Success;
            response.EncodedAssembly = Convert.ToBase64String(payload);

            return response;

        }

        private static ListenerBase GetListener(string listenerId)
        {
            var listeners = Program.ServerController.ListenerController.GetListeners();
            return listeners.Where(l => l.ListenderId.Equals(listenerId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        protected static string CreateTempDirectory()
        {
            var temp = Path.GetTempPath() + Helpers.GeneratePsudoRandomString(6);
            Directory.CreateDirectory(temp);
            return temp;

        }

        protected static void CloneAgentSourceCode(ListenerType listenerType, string tempPath)
        {
            var srcPath = default(string);

            switch (listenerType)
            {
                case ListenerType.Http:
                    srcPath = AgentDirectory + Path.DirectorySeparatorChar + "HttpAgent";
                    break;
                case ListenerType.Tcp:
                    srcPath = AgentDirectory + Path.DirectorySeparatorChar + "Tcp";
                    break;
                default:
                    break;
            }

            var srcFiles = Directory.GetFiles(srcPath, "*.cs", SearchOption.AllDirectories);
            foreach (var filePath in srcFiles)
            {
                if (filePath.Contains("AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase)
                    || filePath.Contains("AssemblyAttributes.cs", StringComparison.OrdinalIgnoreCase)) { continue; }
                var fileName = Path.GetFileName(filePath);
                var finalPath = tempPath + Path.DirectorySeparatorChar + fileName;
                File.Copy(filePath, finalPath, true);
            }

        }

        protected static void RemoveTempDirectory(string path)
        {
            Directory.Delete(path, true);
        }

    }
}
