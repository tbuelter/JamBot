using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Configuration;
using System.Data.Common;
using Discord.Commands;
using System.Data.SqlClient;
using System.Linq;

namespace JamBot
{
    public class Program
    {
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private DiscordSocketClient? _client;
        private CommandHandler? _handler;
        private CommandService? _service;

        public async Task MainAsync()
        {

                
            _client = new DiscordSocketClient();
            _client.Log += Log;

            //Setup the Bot Token
            var token = ConfigurationManager.AppSettings["BotToken"];

            //Start the Bot
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //Commandhandler
            _service = new CommandService();
            _handler = new CommandHandler(_client, _service);
            await _handler.InstallCommandsAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
           
        }
    }
}
