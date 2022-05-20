using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualBasic;
using Discord;

namespace JamBot{
    public class CommandHandler{
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands){
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync(){
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }
        private async Task HandleCommandAsync(SocketMessage messageParam){

            //Get the message
            SocketUserMessage message = (SocketUserMessage)messageParam;

            //Check if the Message is empty or is from the Bot
            if (message == null || message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            //-- Command Checkings -- //
            //Filter: Prefix !, + is a user and not a Bot
            int argPos = 0;
            
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot)
                return;
                
            //The Command              
            await _commands.ExecuteAsync(  context,  argPos, null );
        }
    }
}