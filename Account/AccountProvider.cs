using Common;
using Common.Interface;
using Account.Service;

namespace Account
{
    public class AccountProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Account Service is Starting...");

            _status = ServiceStatus.Running;

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
    }
}