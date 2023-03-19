using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Common.Object;

namespace Common.Network
{
    public class Server
    {
        public Server()
        {

        }

        public async Task Start()
        {
            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 11000));
            listener.Listen(100);

            while (true)
            {
                Socket client = await listener.AcceptAsync();
                Console.WriteLine($"Client connected: {client.RemoteEndPoint}");

                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        var bytesReceived = client.Receive(buffer);

                        Packet packet;
                        using (MemoryStream stream = new MemoryStream(buffer, 0, bytesReceived))
                        {
                            packet = JsonSerializer.Deserialize<Packet>(stream);
                        }

                        /*
                        byte[] message = Encoding.ASCII.GetBytes("Hello Client~");
                        int bytesSent = client.Send(message);

                        Console.WriteLine($"Data sent: {Encoding.ASCII.GetString(message)}");
                        */
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                });
            }
        }
    }
}
