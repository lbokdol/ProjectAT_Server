using Grpc.Core;
using GrpcServerLauncher;

namespace ServerLauncher.gRPC
{
    public class GrpcClient : Greeter.GreeterBase
    {
        private readonly Greeter.GreeterClient _greeterClient;

        public GrpcClient(Greeter.GreeterClient greeterClient)
        {
            _greeterClient = greeterClient;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            // GreeterService에게 Hello 메시지를 요청합니다.
            var response = await _greeterClient.SayHelloAsync(request);

            // 사용자에게 응답을 전달합니다.
            return response;
        }
    }
}
