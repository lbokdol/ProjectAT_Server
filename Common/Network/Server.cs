using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common.Network
{
    internal class Server
    {
        SocketAsyncEventArgs _acceptArgs;
        Socket _listenSocket;

        public delegate void SocketAsyncEventHandler(object sender, SocketAsyncEventArgs e);
        public SocketAsyncEventHandler callbackNewclient;

        public Server()
        {

        }

        public void Start(string host, int port)
        {
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address;
            if (host == "0.0.0.0")
                address = IPAddress.Any;
            else
                address = IPAddress.Parse(host);

            IPEndPoint endPoint = new IPEndPoint(address, port);

            try
            {
                _listenSocket.Bind(endPoint);
                _listenSocket.Listen(1);

                _acceptArgs = new SocketAsyncEventArgs();

                _listenSocket.AcceptAsync(_acceptArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
