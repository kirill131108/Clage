using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot
{
    class InteractionHandler
    {
            private readonly DiscordSocketClient _client;
            private readonly InteractionService _service;
            private readonly IServiceProvider _provider;
        public InteractionHandler(DiscordSocketClient client, InteractionService service, IServiceProvider provider)
        {
            _client = client;
            _service = service;
            _provider = provider;
        }

        public async Task InitilizeAsync()
        {
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            _client.InteractionCreated += HandleInteraction;
        }
        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(_client, arg);
                await _service.ExecuteCommandAsync(ctx, _provider);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}