

using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LightBlueFox.Util.Logging;
using System.Linq;
using LightBlueFox.Util.Types.Exceptions;
using Discord;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
{
    public class CommandHandler
    {
        private readonly char _prefix;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        public static readonly List<HelpInfo> CommandDescriptions = new List<HelpInfo>();

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, char prefix)
        {
            _commands = commands;
            _client = client;
            _prefix = prefix;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
            foreach (var item in _commands.Commands)
            {
                HelpAttribute attr;
                List<ParameterHelpInfo> paramHelps = new List<ParameterHelpInfo>();
                var parameters = item.Parameters;
                foreach (var param in parameters)
                {
                    attr = param.Attributes.Where(p => p.GetType() == typeof(HelpAttribute)).FirstOrDefault() as HelpAttribute;
                    paramHelps.Add(new ParameterHelpInfo(param.Name, attr, param));
                }
                
                
                attr = item.Attributes.Where(p => p.GetType() == typeof(HelpAttribute)).FirstOrDefault() as HelpAttribute;
                CommandDescriptions.Add(new HelpInfo((HelpAttribute)attr, item.Name, item, paramHelps));
            }
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {

            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(_prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

            


            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            if (!SettingsManager.ChannelIDs.Contains(context.Channel.Id) && !context.Channel.GetType().IsAssignableFrom(typeof(IPrivateChannel)))
            {
                await context.Channel.SendMessageAsync("You are not allowed to use the bot in this channel!");
                return;
            }
                
            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            Logger.Log(LogLevel.DEBUG, "Received command: " + message.ToString());
            var res = _commands.Search(context, argPos);
            if (!res.IsSuccess || res.Commands.Count == 0)
                await context.Channel.SendMessageAsync("Unknown command.");

            try
            {
                await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
            }
            catch (FatalException ex)
            {
                Logger.Log(LogLevel.FATAL, "Fatal exception thrown while evaluating command {0}!!!!!!! Exception: {1}.", message.Content, ex.GetType().ToString());
                Logger.Log(LogLevel.DEBUG, "Exception: {0}", ex);
                await context.Channel.SendMessageAsync("Encountered a fatal error while trying to evaluate this command! Shutting down. Please contact me at Jakob#8695.");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.ERROR, "Exception thrown while evaluating command {0}. Exception: {1}.", message.Content, ex.GetType().ToString());
                Logger.Log(LogLevel.DEBUG, "Exception: {0}", ex);
                await context.Channel.SendMessageAsync("Encountered an error while trying to evaluate this command.");
            }
            
        }
    }
}
