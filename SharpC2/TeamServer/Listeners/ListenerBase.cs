using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Listeners
{
    public class ListenerBase
    {
        public string ListenderId { get; set; } = Helpers.GeneratePsudoRandomString(8);
        public int BindPort { get; set; }
    }
}
