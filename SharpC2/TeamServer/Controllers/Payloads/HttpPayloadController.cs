using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Agents;
using TeamServer.Listeners;
using TeamServer.Models;
using static TeamServer.Agents.Compiler;

namespace TeamServer.Controllers
{
    public class HttpPayloadController : PayloadControllerBase
    {
        private ListenerHttp Listener { get; set; }
        public string TempPath { get; set; }

        public HttpPayloadController(ListenerHttp listenerHttp)
        {
            Listener = listenerHttp;
        }

        public byte[] GenerateHttpPayload(PayloadRequest request)
        {
            TempPath = CreateTempDirectory();
            var compilerRequest = new Compiler.CompilationRequest
            {
                AssemblyName = string.IsNullOrEmpty(request.AssemblyName) ? Helpers.GeneratePsudoRandomString(8) : request.AssemblyName,
                OutputKind = (OutputKind)request.OutputType,
                Platform = Platform.AnyCpu,
                ReferenceDirectory = request.TargetFramework == TargetFramework.Net35
                    ? ReferencesDirectory + Path.DirectorySeparatorChar + "net35"
                    : ReferencesDirectory + Path.DirectorySeparatorChar + "net40",
                TargetDotNetVersion = (DotNetVersion)request.TargetFramework,
                SourceDirectory = TempPath,
                References = new List<Compiler.Reference>
                {
                    new Compiler.Reference
                    {
                        File = "mscorlib.dll",
                        Framework = Compiler.DotNetVersion.Net40,
                        Enabled = true
                    }
                }

            };



            CloneAgentSourceCode(Listener.Type, TempPath);

            InsertConnectAddress();
            InsertConnectPort();


            var result = Compiler.Compile(compilerRequest);

            RemoveTempDirectory(TempPath);

            return result;

        }

        private void InsertConnectAddress()
        {
            var srcPath = TempPath + Path.DirectorySeparatorChar + "Program.cs";
            var src = File.ReadAllText(srcPath);
            var newSrc = src.Replace("<<ConnectAddress>>", Listener.ConnectAddress);
            File.WriteAllText(srcPath, newSrc);
        }

        private void InsertConnectPort()
        {
            var srcPath = TempPath + Path.DirectorySeparatorChar + "Program.cs";
            var src = File.ReadAllText(srcPath);
            var newSrc = src.Replace("<<ConnectPort>>", Listener.ConnectPort.ToString());
            File.WriteAllText(srcPath, newSrc);
        }
    }
}
