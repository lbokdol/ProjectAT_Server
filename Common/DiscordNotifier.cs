using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace Common
{
    public class DiscordNotifier
    {
        private readonly ulong _channelId;
        private DiscordSocketClient _client;

        public DiscordNotifier(string token, ulong channelId)
        {
            _channelId = channelId;
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.LoginAsync(TokenType.Bot, token).Wait();
            _client.StartAsync().Wait();
            _client.Ready += ReadyAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            return Task.CompletedTask;
        }

        public async Task SendMessageAsync(string message)
        {
            var channel = _client.GetChannel(_channelId) as IMessageChannel;
            if (channel != null)
            {
                Console.WriteLine(message);
                await channel.SendMessageAsync(message);
            }
        }
    }
}
