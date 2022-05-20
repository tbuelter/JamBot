using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JamBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // Generate LoginData, edit the config of the server and send the Login to the User via Private Message
        // 
        [Command("r")]
        public async Task GenerateLogin()
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;

            string name = Context.User.Username;
            string pw = Functions.GeneratePassword(7);

            // Only give a Login via Server Messages
            if (!Functions.IsPrivateMessage(Context.Message))
            {
                var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "test");

                if (user.Roles.Contains(role))
                {
                    _ = Task.Run(() =>
                      {
                          Server[] serverList = Functions.GetServerList();
                          _ = Functions.Connect(serverList[0], "Login " + name + " " + pw + " -1");
                      });
                    await ReplyAsync("Accepted");
                    await user.SendMessageAsync("Your Jamtaba Login: \nLogin: " + name + "\nPassword: " + pw + "\nHave fun jamming!");

                }
                else
                {
                    await ReplyAsync("Not Accepted");
                }
            }
            else
            {
                // Do Something if its a private message

            }
        }        

        // Get Status
        [Command("stat")]
        public async Task GetNodeStatuses()
        {

            SocketGuildUser user = (SocketGuildUser)Context.User;
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "test");
            if (user.Roles.Contains(role))
            {
                _ = Task.Run(() =>
                {
                    Server[] serverList = Functions.GetServerList();
                    string output = Functions.GetNodeStatuses(serverList);
                    ReplyAsync(output);
                });
                await ReplyAsync("Accepted");
            }

        }
        /* ---- Example ----
        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage ="You don't have the permission ``ban_member``!")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("Please specify a user!"); 
                return;
            }
            if (reason == null) reason = "Not specified";

            await Context.Guild.AddBanAsync(user, 1, reason);

            var EmbedBuilder = new EmbedBuilder()
                .WithDescription($":white_check_mark: {user.Mention} was banned\n**Reason** {reason}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);

            ITextChannel logChannel = Context.Client.GetChannel(642698444431032330) as ITextChannel;
            var EmbedBuilderLog = new EmbedBuilder()
                .WithDescription($"{user.Mention} was banned\n**Reason** {reason}\n**Moderator** {Context.User.Mention}")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("User Ban Log")
                    .WithIconUrl("https://i.imgur.com/6Bi17B3.png");
                });
            Embed embedLog = EmbedBuilderLog.Build();
            await logChannel.SendMessageAsync(embed: embedLog);

        }
        */
    }
}
