using Discord;
using MinecraftConnection;
using Discord.Interactions;
using Discord.Commands;

namespace Bot
{
    
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("console", "console")]
        public async Task Console([Remainder] string command)
        {
            
        }
    }
}