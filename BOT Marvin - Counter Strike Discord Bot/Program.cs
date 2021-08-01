using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBlueFox.Util.Logging;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    class Program
    {
        private Task Log(LogMessage msg)
        {
            Logger.Log(LogLevel.INFO, msg.ToString());
            return Task.CompletedTask;
        }

        private DiscordSocketClient _CLIENT;
        private CommandHandler _HANDLER;
        private CommandService _COMMANDSERVICE;


        private async Task StartBot()
        {
            ViewerModifier.InitViewerModifiers();
            Logger.AddLogWriter(new ConsoleLogWriter(LogLevel.ALL));
            Logger.Log(LogLevel.INFO, "Starting discord bot ... connecting to server.");
            _CLIENT = new DiscordSocketClient();
            _CLIENT.Log += Log;
            _CLIENT.ReactionAdded += ViewerActionMaster.ReactionHandler;
            _CLIENT.ReactionRemoved += ViewerActionMaster.ReactionHandler;
            await _CLIENT.LoginAsync(TokenType.Bot, SettingsManager.Token);
            await _CLIENT.StartAsync();
            MongoInterface.Setup();
            _COMMANDSERVICE = new CommandService();
            _HANDLER = new CommandHandler(_CLIENT, _COMMANDSERVICE, '$');
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;   
            await _HANDLER.InstallCommandsAsync();
        }


        static void Main(string[] args)
            => new Program().AsyncMain().GetAwaiter().GetResult();
        public async Task AsyncMain()
        {
            await StartBot();
            await Task.Delay(-1);
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {

        }

       
    }
}
