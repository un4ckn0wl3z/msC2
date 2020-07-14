using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamServer.Interfaces;
using TeamServer.Listeners;

namespace TeamServer.Modules
{

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
    }

    public class HTTPCommModule : ICommModule
    {

        public ListenerHttp Listener { get; set; }

        private Socket Socket { get; set; }

        public ModuleStatus ModuleStatus { get; private set; } = ModuleStatus.Stopped;

        private static ManualResetEvent AllDone = new ManualResetEvent(false);

        public void Init()
        {
            Socket = new Socket(SocketType.Stream, ProtocolType.IP);

        }

        public void Start()
        {

            ModuleStatus = ModuleStatus.Running;

            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), Listener.BindPort));
                Socket.Listen(100);

                Task.Factory.StartNew(
                    delegate ()
                    {
                        while (ModuleStatus == ModuleStatus.Running)
                        {
                            AllDone.Reset();
                            Socket.BeginAccept(new AsyncCallback(AcceptCallback), Socket);
                            AllDone.WaitOne();
                        }
                    }
                    );
            }
            catch
            {

            }
        }


        private void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();
            var listener = ar.AsyncState as Socket;
            if (ModuleStatus == ModuleStatus.Running)
            {
                var handler = listener.EndAccept(ar);
                var state = new StateObject { workSocket = handler };
                handler.BeginReceive(state.buffer,
                    0,
                    StateObject.BufferSize,
                    0,
                    new AsyncCallback(ReadCallback), state);
            }
        }


        private void ReadCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            var handler = state.workSocket;
            var bytesRead = handler.EndReceive(ar);
            var data = state.buffer;

            if (bytesRead > 0)
            {
                SendData(handler, data);
            }



        }

        private void SendData(Socket handler, byte[] dataReceived)
        {

            var agentTask = GetAgentTask(dataReceived);
            var transformData = TransformOutputData(agentTask);

            var response = new StringBuilder("HTTP/1.1 200 OK\r\n");

            foreach (var header in Listener.TrafficProfile.ServerProfile.Headers)
            {
                response.Append(string.Format("{0}: {1}\r\n", header.Key, header.Value));
            }

            response.Append(string.Format("Content-Length: {0}\r\n", transformData.Length));
            response.Append(string.Format("Date: {0}\r\n", DateTime.UtcNow.ToString("ddd, d MMM yyyy HH:mm:ss UTC")));

            response.Append("\r\n");
            var headers = Encoding.UTF8.GetBytes(response.ToString());

            var dataToSend = new byte[transformData.Length + headers.Length];

            Buffer.BlockCopy(headers, 0, dataToSend, 0, headers.Length);
            Buffer.BlockCopy(transformData, 0, dataToSend, headers.Length, transformData.Length);

            handler.BeginSend(dataToSend, 0, dataToSend.Length, 0, new AsyncCallback(SendCallback), handler);

        }


        private byte[] GetAgentTask(byte[] data)
        {
            return Encoding.UTF8.GetBytes("Hello from SharpC2");
        }


        private byte[] TransformOutputData(byte[] data)
        {
            // Data=AAAAAA
            // 0x
            var transformedData = new byte[] { };
            var preppendData = new byte[] { };
            var appendData = new byte[] { };

            switch (Listener.TrafficProfile.ServerProfile.OutputProfile.DataTransform)
            {
                case Models.HttpTrafficProfile.DataTransform.Raw:
                    transformedData = data;
                    break;
                case Models.HttpTrafficProfile.DataTransform.Base64:
                    transformedData = Encoding.UTF8.GetBytes(Convert.ToBase64String(data));
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData))
            {
                if (Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData.Substring(0, 2).Equals("\\x"))
                {
                    // parse byte
                    preppendData = ParseBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData);
                }
                else
                {
                    preppendData = Encoding.UTF8.GetBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData);
                }
            }
            //------------
            if (!string.IsNullOrEmpty(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData))
            {
                if (Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData.Substring(0, 2).Equals("\\x"))
                {
                    // parse byte
                    appendData = ParseBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData);
                }
                else
                {
                    appendData = Encoding.UTF8.GetBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData);
                }
            }
            // ====
            var result = new byte[preppendData.Length + transformedData.Length + appendData.Length];
            Buffer.BlockCopy(preppendData, 0, result, 0, preppendData.Length);
            Buffer.BlockCopy(transformedData, 0, result, preppendData.Length, transformedData.Length);
            Buffer.BlockCopy(appendData, 0, result, (preppendData.Length + transformedData.Length), appendData.Length);

            return result;
        }


        private byte[] ParseBytes(string data)
        {
            var bytes = new List<byte>();
            var split = data.Split("\\x"); // \xff\xff\xff -> ff ff ff

            foreach (var byteString in split)
            {
                if (!string.IsNullOrEmpty(byteString))
                {
                    byte.TryParse(byteString, NumberStyles.HexNumber, null, out byte parsedByte);
                    bytes.Add(parsedByte);
                }
            }
            return bytes.ToArray();

        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handle = ar.AsyncState as Socket;
                var bytesSent = handle.EndSend(ar);
                handle.Shutdown(SocketShutdown.Both);
                handle.Close();
            }
            catch (SocketException e)
            {

                throw new Exception(e.Message);
            }
        }


        public void Stop()
        {
            ModuleStatus = ModuleStatus.Stopped;
            Socket.Close();
            // Socket.Dispose();
        }
    }
}
