﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class HttpTrafficProfile
    {

        public ServerTrafficProfile ServerProfile { get; set; } = new ServerTrafficProfile();
        public ClientTrafficProfile ClientProfile { get; set; } = new ClientTrafficProfile();


        public class ServerTrafficProfile
        {
            public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

            public OutputTrafficProfile OutputProfile { get; set; } = new OutputTrafficProfile();

            public class OutputTrafficProfile
            {
                public DataTransform DataTransform { get; set; } = DataTransform.Raw;
                public string PrependData { get; set; } = "";
                public string AppendData { get; set; } = "";
            }
        }

        public class ClientTrafficProfile
        {

        }

        public enum DataTransform
        {
            Raw,
            Base64
        }

    }
}
