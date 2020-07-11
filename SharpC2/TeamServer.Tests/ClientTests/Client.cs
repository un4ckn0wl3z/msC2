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
        public void SuccessfulClientLogin()
        {
            var request = new ClientAuthenticationRequest
            {
                Nick = "Anuwat",
                Password = "anuwat1337"
            };

            var result = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.LoginSucess, result.Result);
            Assert.NotNull(result.Token);
        }
    }
}
