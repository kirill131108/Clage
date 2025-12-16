using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;

namespace Bot
{
    
    public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "ping")]
        public async Task CreateButton()
        {
            await RespondAsync("TTTT");
        }

        [SlashCommand("create", "create a button")]
        public async Task CreateTicketHandler()
        {
            var button = new ButtonBuilder()
            {
                Label = "Tiket",
                CustomId = "tiketbutton",
                Style = ButtonStyle.Primary
            };

            var comp = new ComponentBuilder();
            comp.WithButton(button);

            await RespondAsync("", components: comp.Build());
        }
        [ComponentInteraction("tiketbutton")]
        public async Task HandleButtonInput()
        {
            Console.WriteLine("Модал работает");
            await RespondWithModalAsync<DemoModal>("tiket");
        }

        [ModalInteraction("tiket")]
        public async Task HandleModalInput(DemoModal demo)
        {
            string? message_v1 = demo.Greeting_v1;
            string? message_v2 = demo.Greeting_v2;
            string? message_v3 = demo.Greeting_v3;
            string? message_v4 = demo.Greeting_v4;

            ulong channelID = 1445785880500506758;
            var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
            if (channel == null) return;

            await channel.SendMessageAsync($"1 - {message_v1} \n 2 - {message_v2} \n 3 - {message_v3} \n 4 - {message_v4} \n 5 - {Context.User.Id}");
        }
    }
    public class DemoModal : IModal
    {
        public string Title => "tiket";
        [InputLabel("Ваш ник")]
        [ModalTextInput("ник", TextInputStyle.Short, placeholder: "steave", maxLength: 400)]
        public string? Greeting_v1 {get; set;}

        [InputLabel("Сколько вам лет")]
        [ModalTextInput("лет", TextInputStyle.Short, placeholder: "14", maxLength: 4)]
        public string? Greeting_v2 {get; set;}
        
        [InputLabel("Чем вы занимаетесь?")]
        [ModalTextInput("занятия", TextInputStyle.Short, placeholder: "редстоуном", maxLength: 400)]
        public string? Greeting_v3 {get; set;}
        
        [InputLabel("что вы собираетесь делать на сервере")]
        [ModalTextInput("сервак", TextInputStyle.Short, placeholder: "14", maxLength: 400)]
        public string? Greeting_v4 {get; set;}
        
    }
}