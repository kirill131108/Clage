using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Bot
{
    class PrefixHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _command;
        private readonly IConfigurationRoot _config;

        public PrefixHandler(DiscordSocketClient client, CommandService command, IConfigurationRoot config)
        {
            _client = client;
            _command = command;
            _config = config;
        }

        public async Task InitilizeAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
        }

        public void AddModule<T>()
        {
            _command.AddModuleAsync<T>(null);
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;

            if (message == null) return;

            int ArgPos = 0;

            if (!(message.HasCharPrefix(_config["prefix"]![0], ref ArgPos)) || !message.HasMentionPrefix(_client.CurrentUser, ref ArgPos) || message.Author.IsBot) return;
            
            var context = new SocketCommandContext(_client, message);

            await _command.ExecuteAsync(context: context, argPos: ArgPos, services: null);
        }
    }
}