using System;
using System.Collections.Generic;
using System.Text;
using TeamServer.Listeners;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.ListenerTests
{
    public class Listener
    {
        public Listener()
        {
            new TestClient();
        }

        [Fact]
        public async void _06_StartHttpListener()
        {
            await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");
            var listenerRequest = new NewHttpListenerRequest
            {
                BindPort = 8080,
                ConnectAddress = "127.0.0.1",
                ConnectPort = 8080
            };

            var apiReq = await TestClient.HttpClient.PostAsync("api/Listener/http", Helpers.Serialise(listenerRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            var result = Helpers.Deserialise<ListenerHttp>(resultAsync);

            Assert.Equal(8080, result.BindPort);
            Assert.Equal("127.0.0.1", result.ConnectAddress);
            Assert.Equal(8080, result.ConnectPort);

        }

        [Fact]
        public async void _07_StartTcpListener()
        {
            await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");
            var listenerRequest = new NewTcpListenerRequest
            {
                BindAddress = "0.0.0.0",
                BindPort = 8080
            };

            var apiReq = await TestClient.HttpClient.PostAsync("api/Listener/tcp", Helpers.Serialise(listenerRequest));
            var resultAsync = await apiReq.Content.ReadAsStringAsync();

            var result = Helpers.Deserialise<ListenerTcp>(resultAsync);

            Assert.Equal(8080, result.BindPort);
            Assert.Equal("0.0.0.0", result.BindAddress);


        }
    }
}
