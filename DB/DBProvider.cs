using Common;
using Common.Interface;
using System.Net;

namespace DB
{
    public class DBProvider : Common.Interface.IServiceProvider
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
                LoggingService.Logger.Information("DB Service is Starting...");

                await _taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "DB Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("DB is stopping...");
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