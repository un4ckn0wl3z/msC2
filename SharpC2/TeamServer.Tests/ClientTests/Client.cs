using System;
using System.Collections.Generic;
using System.Text;
using TeamServer.Listeners;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.ClientTests
{
    public class Client
    {

        public Client()
        {
            new TestClient();
        }


        [Fact]
        public async void _01_SuccessfulClientLogin()
        {
            var result = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1337");

            Assert.Equal(ClientAuthenticationResult.AuthResult.LoginSucess, result.Result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async void _02_BadPasswordClientLogin()
        {
            var result = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePsudoRandomString(6), "anuwat1995");

            Assert.Equal(ClientAuthenticationResult.AuthResult.BadPassword, result.Result);
            Assert.Null(result.Token);
        }

        [Fact]
        public async void _03_NickInUse()
        {
            var user = TeamServer.Helpers.GeneratePsudoRandomString(6);
            await TestClient.ClientLogin(user, "anuwat1337");
            var result = await TestClient.ClientLogin(user, "anuwat1337");


            Assert.Equal(ClientAuthenticationResult.AuthResult.NickInUse, result.Result);
            Assert.Null(result.Token);
        }

        [Theory]
        [InlineData("anuwat", "")]
        [InlineData("", "password")]
        public async void _04_InvalidRequest(string nick, string pass)
        {
            var result = await TestClient.ClientLogin(nick, pass);

            Assert.Equal(ClientAuthenticationResult.AuthResult.InvalidRequest, result.Result);
            Assert.Null(result.Token);
        }


        [Fact]
        public async void _05_GetConnectedClients()
        {
            var user = TeamServer.Helpers.GeneratePsudoRandomString(6);
            await TestClient.ClientLogin(user, "anuwat1337");
            await TestClient.ClientLogin("Anuwat", "anuwat1337");

            var apiReq = await TestClient.HttpClient.GetAsync("api/Client");
            var resultAsync = await apiReq.Content.ReadAsStringAsync();
            var result = Helpers.Deserialise<IEnumerable<string>>(resultAsync);

            Assert.Contains(result, n => n == user);
        }

    }
}
