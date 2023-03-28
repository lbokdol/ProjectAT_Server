using Common;
using Common.Interface;

namespace Player
{
    public class PlayerProvider: Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Player Service is Starting...");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Player Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Player is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;
    }
}