using Discord.Commands;

namespace Bot
{
    public class PrefixModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PONK()
        {
            await ReplyAsync("PONG");
        }
    }
}