using Common;
using Common.Interface;
using System.Net;

namespace Chat
{
    public class ChatProvider : Common.Interface.IServiceProvider
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        private ServiceStatus _status = ServiceStatus.Stopped;
        private string _address;
        private int _port;

        public async Task RunAsync(string address, int port, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
            

            Initialize(address, port);

            try
            {
                LoggingService.Logger.Information("Chat Service is Starting...");

                await _taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Chat Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Chat is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

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