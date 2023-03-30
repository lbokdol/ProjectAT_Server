using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Interface;
using Common.Network;
using Common.Packet;
using AccountSpace;

using MessagePack;

namespace Session
{
    public class SessionProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;
        private readonly int _maxConnections;
        private readonly SemaphoreSlim _connectionSemaphore;
        private readonly SymmetricAlgorithm _encryptionAlgorithm = Aes.Create();
        private readonly string _secretKey = "tlzmfltzl";
        private Client.AccountServiceClient _accountClient;

        private string _address;
        private int _port;

        public SessionProvider(int maxConnections)
        {
            _maxConnections = maxConnections;
            _connectionSemaphore = new SemaphoreSlim(maxConnections, maxConnections);

            // 주소와 포트 받아와야함 고민할 것
            _accountClient = new Client.AccountServiceClient("localhost:6801");
        }

        public async Task RunAsync(string address, int port, CancellationToken cancellationToken)
        {
            try
            {
                _encryptionAlgorithm.Key = Encoding.UTF8.GetBytes("abc-htd-hjb-lgf-weq-lqx-pqn-rkbl");
                _encryptionAlgorithm.IV = Encoding.UTF8.GetBytes("abc-htd-hjb-lgfh");
                _encryptionAlgorithm.Padding = PaddingMode.PKCS7;

                Initialize(address, port);

                var listener = new TcpListener(IPAddress.Parse(address), port);
                listener.Start();

                LoggingService.Logger.Information("Session Service is Starting...");

                while (!cancellationToken.IsCancellationRequested)
                {
                    await _connectionSemaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var clientSocket = await listener.AcceptSocketAsync();
                        _ = HandleClientAsync(clientSocket, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accepting TCP client: {ex.Message}");
                    }
                }

                listener.Stop();
                Console.WriteLine("Session service stopped.");
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Session Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Session is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private async Task HandleClientAsync(Socket clientSocket, CancellationToken cancellationToken)
        {
            using (clientSocket)
            {
                using var networkStream = new NetworkStream(clientSocket, true);

                while (!cancellationToken.IsCancellationRequested && clientSocket.Connected)
                {
                    Packet packet = await ReceivePacketAsync(networkStream, _encryptionAlgorithm);
                    Packet responsePacket = await ProcessPacketAsync(packet);
                    await SendPacketAsync(networkStream, responsePacket, _encryptionAlgorithm);
                }
            }
        }

        private async Task<Packet> ReceivePacketAsync(NetworkStream networkStream, SymmetricAlgorithm encryptionAlgorithm)
        {
            byte[] lengthBuffer = new byte[sizeof(int)];
            await networkStream.ReadAsync(lengthBuffer, 0, sizeof(int));
            int packetLength = BitConverter.ToInt32(lengthBuffer);

            byte[] encryptedPacketBuffer = new byte[packetLength];
            await networkStream.ReadAsync(encryptedPacketBuffer, 0, packetLength);

            byte[] decryptedPacketBuffer = Decrypt(encryptedPacketBuffer, encryptionAlgorithm);
            Packet packet = MessagePackSerializer.Deserialize<Packet>(decryptedPacketBuffer);

            if (!VerifyPacketSignature(packet))
            {
                throw new Exception("Invalid packet signature");
            }

            return packet;
        }

        private async Task SendPacketAsync(NetworkStream networkStream, Packet packet, SymmetricAlgorithm encryptionAlgorithm)
        {
            byte[] packetData = MessagePackSerializer.Serialize(packet);

            byte[] encryptedPacketData = Encrypt(packetData, encryptionAlgorithm);

            byte[] lengthBuffer = BitConverter.GetBytes(encryptedPacketData.Length);
            await networkStream.WriteAsync(lengthBuffer, 0, sizeof(int));

            await networkStream.WriteAsync(encryptedPacketData, 0, encryptedPacketData.Length);
            
        }

        private async Task<Packet> ProcessPacketAsync(Packet packet)
        {
            Packet response;

            switch (packet.Type)
            {
                case PacketType.LoginRequest:
                    var loginData = MessagePackSerializer.Deserialize<LoginRequest>(packet.Data);

                    var loginResult = await AuthenticateUserAsync(new LoginReq() { Username = loginData.Username, Password = loginData.Password });
                    var loginResponse = new LoginResponse { UserName = loginResult.Username, StatusCode = loginResult.StatusCode, Message = loginResult.Message };
                    byte[] loginResponseData = MessagePackSerializer.Serialize(loginResponse);

                    response = new Packet
                    {
                        Type = PacketType.LoginResponse,
                        UserId = packet.UserId,
                        Data = loginResponseData
                    };

                    response.Signature = GeneratePacketSignature(response);
                    break;
                case PacketType.MoveRequest:
                    var moveData = MessagePackSerializer.Deserialize<MoveData>(packet.Data);

                    bool moveResult = await ProcessPlayerMoveAsync(moveData.PlayerId, moveData.NewPosition);
                    var moveResponse = new MoveResponse { Result = moveResult };
                    byte[] moveResponseData = MessagePackSerializer.Serialize(moveResponse);

                    response = new Packet
                    {
                        Type = PacketType.MoveResponse,
                        UserId = packet.UserId,
                        Data = moveResponseData
                    };
                    response.Signature = GeneratePacketSignature(response);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown packet type '{packet.Type}'");
            }
            
            return response;
        }

        private bool VerifyPacketSignature(Packet packet)
        {
            string secretKey = _secretKey;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));

            byte[] computedSignature = hmac.ComputeHash(packet.GetSignatureData());
            string computedSignatureString = BitConverter.ToString(computedSignature).Replace("-", string.Empty);

            return string.Equals(computedSignatureString, packet.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private string GeneratePacketSignature(Packet packet)
        {
            string secretKey = _secretKey;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));

            byte[] signature = hmac.ComputeHash(packet.GetSignatureData());

            return BitConverter.ToString(signature).Replace("-", string.Empty);
        }

        private byte[] Encrypt(byte[] data, SymmetricAlgorithm encryptionAlgorithm)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptionAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);

            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();

            return memoryStream.ToArray();
        }

        private byte[] Decrypt(byte[] data, SymmetricAlgorithm encryptionAlgorithm)
        {
            using var decryptor = encryptionAlgorithm.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }

        private async Task<LoginRes> AuthenticateUserAsync(LoginReq request)
        {
            var response = await _accountClient.LoginAsync(request);
            return response;
        }

        private Task<bool> ProcessPlayerMoveAsync(Guid userId, double position)
        {
            return Task<bool>.FromResult(true);
        }

        private void Initialize(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public string GetAddress()
        {
            return _address;
        }

        public int GetPort()
        {
            return _port;
        }
    }
}
