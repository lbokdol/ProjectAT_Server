using Serilog;

namespace Common
{
    public static class LoggingService
    {
        public static ILogger Logger { get; private set; }
        public static DiscordNotifier _discordNotifier;

        static LoggingService()
        {
            string apiKey = "";
            ulong channelId = 0;
            _discordNotifier = new DiscordNotifier(apiKey, channelId);

            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/logfile.log", rollingInterval: RollingInterval.Hour)
                .CreateLogger();
        }

        static public void LogInformation(string message)
        {
            Logger.Information(message);
        }

        static public async Task LogError(string message)
        {
            Logger.Error(message);
            await _discordNotifier.SendMessageAsync($"[ERROR] {message}");
        }
    }
}