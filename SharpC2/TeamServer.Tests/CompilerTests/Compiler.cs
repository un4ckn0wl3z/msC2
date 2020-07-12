using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.CompilerTests
{
    public class Compiler
    {


        public Compiler()
        {
            new TestClient();
        }

        internal async Task<ListenerHttp> StartHttpListener()
        {
            await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");
            var listenerRequest = new NewHttpListenerRequest
            {
                BindPort = 8081,
                ConnectAddress = "127.0.0.1",
                ConnectPort = 8081
            };

            var apiReq = await TestClient.HttpClient.PostAsync("api/Listener/http", Helpers.Serialise(listenerRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            return Helpers.Deserialise<ListenerHttp>(resultAsync);

        }

        internal async Task<ListenerTcp> StartTcpListener()
        {
            await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");
            var listenerRequest = new NewTcpListenerRequest
            {
                BindAddress = "0.0.0.0",
                BindPort = 8080
            };

            var apiReq = await TestClient.HttpClient.PostAsync("api/Listener/tcp", Helpers.Serialise(listenerRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            return Helpers.Deserialise<ListenerTcp>(resultAsync);


        }


        [Fact]
        public async void _08_GenerateHttpAgentPayloadSuccess()
        {
            var res = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");

            var listener = await StartHttpListener();

            var payloadRequest = new PayloadRequest
            {
                ListenerId = listener.ListenderId,
                OutputType = OutputType.Exe,
                TargetFramework = TargetFramework.Net40
            };


            var apiReq = await TestClient.HttpClient.PostAsync("api/Payload", Helpers.Serialise(payloadRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            var result = Helpers.Deserialise<PayloadResponse>(resultAsync);

            Assert.Equal(CompilerStatus.Success, result.CompilerStatus);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.EncodedAssembly);

        }



        [Fact]
        public async void _09_GenerateHttpAgentPayloadFailed()
        {
            var res = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");

            var payloadRequest = new PayloadRequest
            {
                ListenerId = "bitch!",
                OutputType = OutputType.Exe,
                TargetFramework = TargetFramework.Net40
            };


            var apiReq = await TestClient.HttpClient.PostAsync("api/Payload", Helpers.Serialise(payloadRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            var result = Helpers.Deserialise<PayloadResponse>(resultAsync);

            Assert.Equal(CompilerStatus.Fail, result.CompilerStatus);
            Assert.NotNull(result.ErrorMessage);
            Assert.Null(result.EncodedAssembly);

        }

    }
}
