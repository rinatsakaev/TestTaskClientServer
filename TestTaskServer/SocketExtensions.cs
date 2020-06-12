using System.Linq;
using System.Net.Sockets;

namespace TestTaskServer
{
    public static class SocketExtensions
    {
        public static void SendWithTrailingZero(this Socket socket, byte[] bytes)
        {
            socket.Send(bytes.ToList().Append<byte>(0).ToArray());
        }
    }
}