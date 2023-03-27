using Grpc.Core;
using GrpcServerLauncher;

namespace ServerLauncher.gRPC
{
    public class GrpcServer
    {
        private GreeterService _greeterService;
        private Server _server;
        private Channel _greeterChannel;
        private Greeter.GreeterClient _greeterClient;
        private GrpcClient _GrpcClientService;

        private int _port;
        private string _address;

        public GrpcServer(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public void Initialize()
        {
            _greeterChannel = new Channel(_address, _port, ChannelCredentials.Insecure);
            _greeterClient = new Greeter.GreeterClient(_greeterChannel);

            // GrpcClient 인스턴스 생성 및 클라이언트 전달
            _GrpcClientService = new GrpcClient(_greeterClient);

            // 서비스 인스턴스 생성
            _greeterService = new GreeterService();
            _server = new Server
            {
                Services = { Greeter.BindService(_greeterService) },
                Ports = { new ServerPort(_address, _port, ServerCredentials.Insecure) }
            };
        }

        public void Start()
        {
            _server.Start();
        }

        public void Shutdown()
        {
            _server.ShutdownAsync().Wait();
        }
    }
}
