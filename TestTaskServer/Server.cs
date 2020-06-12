using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestTaskServer
{
    internal class Server
    {
        private readonly WordDictionary dictionary;
        private readonly int port;
        public Server(WordDictionary dictionary, int port)
        {
            this.dictionary = dictionary;
            this.port = port;
        }

        public void Run()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList
                .First(x => x.AddressFamily == AddressFamily.InterNetwork);
            var localEndPoint = new IPEndPoint(ipAddress, port);
            var listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);
            Console.Write("Started on " + ipAddress +"\n>");
            while (true)
            {
                var clientSocket = listener.Accept();
                Console.Write("Client connected\n>");
                ThreadPool.QueueUserWorkItem(HandleClient, clientSocket);
            }
        }

        private void HandleClient(object socketObject)
        {
            var socket = (Socket)socketObject;
            var bytes = new byte[64];
            try
            {
                var bytesReceived = 0;
                while ((bytesReceived = socket.Receive(bytes)) != 0)
                {
                    var input = Encoding.UTF8.GetString(bytes, 0, bytesReceived).Split(' ');
                    if (input[0] != "get" || input.Length < 2)
                    {
                        socket.SendWithTrailingZero(Encoding.UTF8.GetBytes("Unknown command"));
                        continue;
                    }
                    var rawPredictions = dictionary.GetPredictions(input[1]);
                    foreach (var prediction in rawPredictions.Select(x => x.Value))
                        socket.Send(Encoding.UTF8.GetBytes(prediction + "\n"));
                    socket.Send(new byte[1] { 0 });
                }
            }
            catch (SocketException)
            {
                Console.Write("Client disconnected\n>");
            }
            finally
            {
                socket.Close();
            }
        }
    }
}
