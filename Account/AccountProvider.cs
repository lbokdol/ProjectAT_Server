using Common;
using Common.Interface;
using AccountSpace.Service;

namespace AccountSpace
{
    public class AccountProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;
        private AccountService _service;
        private string _address;
        private int _port;

        public async Task RunAsync(string address, int port, CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Account Service is Starting...");

            _status = ServiceStatus.Running;

            Initialize(address, port);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Account Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Account is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

        private void Initialize(string address, int port)
        {
            _address = address;
            _port = port;

            CreateService(address, port);
        }

        private void CreateService(string address, int port)
        {
            _service = new AccountService(address, port);
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