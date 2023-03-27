using Common;
using Common.Interface;

namespace Redis
{
    public class RedisProvider : Common.Interface.IServiceProvider
    {
        private ServiceStatus _status = ServiceStatus.Stopped;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            LoggingService.Logger.Information("Redis Service is Starting...");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Redis Service is encountered...");
                _status = ServiceStatus.Error;
            }
            finally
            {
                LoggingService.Logger.Information("Redis is stopping...");
                _status = ServiceStatus.Stopping;
            }
        }
        public ServiceStatus Status => _status;
    }
}