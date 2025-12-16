using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bot
{
    class Program
    {
        public static async Task Main(String[] args) => await new Program().StartAsync();

        public async Task StartAsync()
        {
            using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            services
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = Discord.GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))).Build();

            await RunAsync(host);
        }
        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;

            var _client = serviceProvider.GetRequiredService<DiscordSocketClient>();

            _client.Log += async (LogMessage msg) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;  
                Console.WriteLine(msg.ToString());
                await Task.CompletedTask;
                Console.ForegroundColor = ConsoleColor.White;
            };
            _client.Ready += async () =>
            {
                Console.WriteLine("Bot Start");  
                await Task.CompletedTask;
            }; 

            await _client.LoginAsync(TokenType.Bot, "");
            await _client.StartAsync();

            Console.ReadLine();

            await _client.StopAsync();
            await Task.Delay(5000);
        }
    }
}
