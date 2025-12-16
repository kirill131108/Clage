using Discord;
using Discord.Interactions;

namespace Bot
{
    
    class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("Create-Button", "Create Ticket Button")]
        public async Task CreateButton()
        {
            
        }
    }
}