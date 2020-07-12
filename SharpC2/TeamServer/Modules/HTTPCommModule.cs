using System;
using System.Collections.Generic;
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
            // var valid =  ValidateRequest(dataReceived)
            // if (valid) { SendAgentTask } else { SendSomeJunk }

            var response = new StringBuilder("HTTP/1.1 200 OK\r\n");
            response.Append("Content-Type: plain/text\r\n\r\n");
            response.Append("Hello from SharpC2");

            var dataToSend = Encoding.UTF8.GetBytes(response.ToString());
            handler.BeginSend(dataToSend, 0, dataToSend.Length, 0, new AsyncCallback(SendCallback), handler);

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
