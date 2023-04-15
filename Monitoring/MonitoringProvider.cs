using Common;
using Common.Interface;
using System.Net;

namespace Monitoring
{
    public class MonitoringProvider : Common.Interface.IServiceProvider
    {
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();

        private ServiceStatus _status = ServiceStatus.Stopped;
        private string _address;
        private int _port;
        private Dictionary<string, List<string>> _serviceInfos;

        public async Task RunAsync(string address, int port, Dictionary<string, List<string>> serviceInfos, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());

            Initialize(address, port, serviceInfos);

            try
            {
                LoggingService.Logger.Information("Monitoring Service is Starting...");
                _status = ServiceStatus.Running;
                await _taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Monitoring Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Monitoring is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private void Initialize(string address, int port, Dictionary<string, List<string>> serviceInfos)
        {
            _address = address;
            _port = port;
            _serviceInfos = serviceInfos;
        }

        public string GetAddress()
        {
            return _address;
        }

        public int GetPort()
        {
            return _port;
        }

        public Dictionary<string, List<string>> GetServices()
        {
            return _serviceInfos;
        }
    }
}