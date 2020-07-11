using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace TeamServer.Controllers
{
    public class AuthenticationController
    {
        private static byte[] _serverPassword { get; set; }
        public static byte[] JWTSecret { get; private set; } = Encoding.UTF8.GetBytes("@@anuwat1337p-developer-CTW&73G.AS%e&N@%");

        public static void SetPassword(string plaintext)
        {
            _serverPassword = HashPassword(plaintext);
        }

        private static byte[] HashPassword(string plaintext)
        {
            using (var crypto = SHA512.Create())
            {
                return crypto.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            }
        }

    }
}
