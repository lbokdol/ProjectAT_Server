using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Common.Packet;

namespace Common.Network
{
    public class Server
    {
        private TcpListener _listener;
        private int _port;
        private bool _running;

        public Server(int port)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Any, port);
            _running = false;
        }

        public async Task StartAsync()
        {
            if (!_running)
            {
                _listener.Start();
                _running = true;
                Console.WriteLine("Server started on port " + _port);
                while (_running)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Task.Run(() => HandleClientAsync(client));
                }
            }
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _listener.Stop();
                Console.WriteLine("Server stopped");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

                NetworkStream stream = client.GetStream();

                while (_running && client.Connected)
                {
                    // Receive the packet from the client
                    Packet receivedPacket = await Packet.ReceiveAsync(stream);

                    // Handle the received packet in parallel
                    Packet responsePacket = await Task.Run(() => HandlePacketAsync(receivedPacket));

                    // Send the response packet to the client
                    await responsePacket.SendAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected: " + client.Client.RemoteEndPoint);
            }
        }

        private async Task<Packet> HandlePacketAsync(Packet packet)
        {
            // Process the received packet here in a parallel-friendly way
            // ...

            // Create and return the response packet
            Packet responsePacket = new Packet();
            // ...
            return responsePacket;
        }
    }
}
