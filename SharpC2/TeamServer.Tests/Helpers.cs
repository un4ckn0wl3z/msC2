using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer.Tests
{
    public static class Helpers
    {

        public static HttpContent Serialise(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }


        public static T Deserialise<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static T Deserialise<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                return (T)ser.ReadObject(ms);
            }
        }

        public static byte[] Prune(this byte[] bytes)
        {
            if (bytes.Length == 0) return bytes;
            var i = bytes.Length - 1;
            while (bytes[i] == 0)
            {
                i--;
            }
            byte[] copy = new byte[i + 1];
            Array.Copy(bytes, copy, i + 1);
            return copy;
        }
    }
}