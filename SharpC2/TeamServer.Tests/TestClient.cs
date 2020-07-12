using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TeamServer.Models;

namespace TeamServer.Tests
{
    class TestClient
    {

        public static HttpClient HttpClient { get; set; }

        public TestClient()
        {
            var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient = testServer.CreateClient();
            Program.StartTeamServer("anuwat1337");
        }

        internal static async Task<ClientAuthenticationResult> ClientLogin(string nick, string pass)
        {
            var authRequest = new ClientAuthenticationRequest { Nick = nick, Password = pass };
            var response = await HttpClient.PostAsync("api/Client", Helpers.Serialise(authRequest));

            var resultAsync = await response.Content.ReadAsStringAsync();

            var result = Helpers.Deserialise<ClientAuthenticationResult>(resultAsync);

            if (result.Result == ClientAuthenticationResult.AuthResult.LoginSucess)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
            }

            return result;
        }

    }
}
