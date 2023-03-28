using Common;
using Common.Interface;

namespace Authentication
{
    public class AuthenticationProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Authentication Service is Starting...");

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
                LoggingService.Logger.Error(ex, "Authentication Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Authentication is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;

    }
}