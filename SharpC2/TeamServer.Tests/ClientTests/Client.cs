using System;
using System.Collections.Generic;
using System.Text;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.ClientTests
{
    public class Client
    {
        [Fact]
        public void _01_SuccessfulClientLogin()
        {
            var request = new ClientAuthenticationRequest { Nick = "Anuwat", Password = "anuwat1337" };

            var result = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.LoginSucess, result.Result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public void _02_BadPasswordClientLogin()
        {
            var request = new ClientAuthenticationRequest { Nick = "Anuwat", Password = "anuwat1336" };

            var result = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.BadPassword, result.Result);
            Assert.Null(result.Token);
        }

        [Fact]
        public void _03_NickInUse()
        {
            var request = new ClientAuthenticationRequest { Nick = "Anuwat", Password = "anuwat1337" };

            var result = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.NickInUse, result.Result);
            Assert.Null(result.Token);
        }

        [Fact]
        public void _04_InvalidRequest()
        {
            var request = new ClientAuthenticationRequest { Nick = "", Password = "" };

            var result = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.InvalidRequest, result.Result);
            Assert.Null(result.Token);
        }
    }
}
