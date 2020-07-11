using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer.Controllers
{
    public class AuthenticationController
    {
        public static byte[] JWTSecret { get; private set; } = Encoding.UTF8.GetBytes("@@anuwat1337p-developer-CTW&73G.AS%e&N@%");
    }
}
