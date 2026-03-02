using Discord;
using MinecraftConnection;
using Discord.Interactions;
using Discord.Commands;
using MinecraftConnection.RCON;

namespace Bot
{
    
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("console", "console")]
        public async Task Console([Remainder] string command)
        {
            string name = "127.0.0.1";
            ushort port = 25565;
            string pass = "minecraft";

            var rcon = new MinecraftCommands(name, port, pass);

        }
    }
}