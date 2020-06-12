using System;
using System.Net.Sockets;
using System.Text;

namespace TestTaskClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("Params: hostname port");
            var hostName = args[0];
            var port = int.Parse(args[1]);
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(hostName, port);
                var stream = tcpClient.GetStream();
                var buffer = new byte[1024];
                Console.Write(">");
                var clientInput = Console.ReadLine();
                while (!string.IsNullOrEmpty(clientInput))
                {
                    stream.Write(Encoding.UTF8.GetBytes(clientInput));
                    var bytesReceived = 0;
                    do
                    {
                        bytesReceived = stream.Read(buffer);
                        Console.Write(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
                    } while (buffer[bytesReceived - 1] != 0);
                    Console.WriteLine();
                    Console.Write(">");
                    clientInput = Console.ReadLine();
                }
            }
        }
    }
}
