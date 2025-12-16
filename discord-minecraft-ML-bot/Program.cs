using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Discord.Interactions;
using Discord.Commands;

namespace Bot
{
    class Program
    {
        public static async Task Main(String[] args) => await new Program().StartAsync();

        public async Task StartAsync()
        {
            
            var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddYamlFile("config.yml")
            .Build();


            using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            services
            .AddSingleton(config)
            .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = Discord.GatewayIntents.All,
                AlwaysDownloadUsers = true
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton(x => new CommandService())
            .AddSingleton<PrefixHandler>()
            )
            .Build();

            await RunAsync(host);
        }
        public async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;

            var _client = serviceProvider.GetRequiredService<DiscordSocketClient>();

            var sCommands = serviceProvider.GetRequiredService<InteractionService>();
            await serviceProvider.GetRequiredService<InteractionHandler>().InitializeAsync();
            var config = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var pCommands = serviceProvider.GetRequiredService<PrefixHandler>();
            pCommands.AddModule<PrefixModule>();
            await pCommands.InitializeAsync();
            


            _client.Log += async (LogMessage msg) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;  
                Console.WriteLine(msg.ToString());
                await Task.CompletedTask;
                Console.ForegroundColor = ConsoleColor.White;
            };
            sCommands.Log += async (LogMessage msg) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;  
                Console.WriteLine(msg.ToString());
                await Task.CompletedTask;
                Console.ForegroundColor = ConsoleColor.White;
            };
            _client.Ready += async () =>
            {
                Console.WriteLine("Bot Start");  
                await sCommands.RegisterCommandsToGuildAsync(ulong.Parse(config["testGuild"]!));
            }; 
            _client.MessageReceived += async (SocketMessage msg) =>
            {
                Console.WriteLine($"{msg.Author.GlobalName} -> {msg.Content}");
                if (msg.Channel is ITextChannel textChannel)
                await ML_model.ExecuteAsync(msg.Content, msg.Id, textChannel);
            };

            await _client.LoginAsync(TokenType.Bot, config["tokens:discord"]);
            await _client.StartAsync();

            Console.ReadLine();

            await _client.StopAsync();
        }
    }
}
