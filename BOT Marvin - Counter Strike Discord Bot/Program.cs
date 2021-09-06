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
using BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff;

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
            Logger.AddLogWriter(new ConsoleLogWriter(LogLevel.ALL));
            SettingsManager.Initialize(tkn: cmdToken, tokenPath: cmdTokenPath, settingsPath: cmdSettingsPath, mip: cmdMongoIP);
            MongoInterface.Setup();
            ViewerModifier.InitViewerModifiers();
            Logger.Log(LogLevel.INFO, "Starting discord bot! Connecting to discord api.");
            _CLIENT = new DiscordSocketClient();
            _CLIENT.Log += Log;
            await _CLIENT.SetGameAsync("$help", type: ActivityType.Listening);
            _CLIENT.ReactionAdded += ViewerActionMaster.ReactionHandler;
            _CLIENT.ReactionRemoved += ViewerActionMaster.ReactionHandler;
            try
            {
                await _CLIENT.LoginAsync(TokenType.Bot, SettingsManager.Token);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.FATAL, "Error authenticating with the discord api... Is the token correct? Error: {0}", e.Message);
                Logger.Log(LogLevel.FATAL, "Fatal Error encountered while connecting to api. Exiting...");
                Environment.Exit(999);
            }
            
            await _CLIENT.StartAsync();
            _COMMANDSERVICE = new CommandService();
            _HANDLER = new CommandHandler(_CLIENT, _COMMANDSERVICE, '$');
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            ActivityMonitor.Init(_CLIENT);
            ActivityRewarder.StartGivingActivityRewards();
            await _HANDLER.InstallCommandsAsync();
        }


        public static void Main(string[] args)
        {
            HandleCMDLineArgs(args);
            new Program().AsyncMain().GetAwaiter().GetResult();
        }
            
        public async Task AsyncMain()
        {
            await StartBot();
            await Task.Delay(-1);
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _CLIENT.Dispose();
        }


        private static string cmdMongoIP = null;
        private static string cmdToken = null;
        private static string cmdSettingsPath = null;
        private static string cmdTokenPath = null;
        private static void HandleCMDLineArgs(string[] args)
        {
            try
            {
                foreach (var a in args)
                {
                    switch (a.ToLower())
                    {
                        case "-dev":
                            SettingsManager.IsDevEnv = true;
                            break;
                        default:
                            break;
                        case "--token":
                            var i = Array.FindIndex<string>(args, new Predicate<string>((string val) => { return val == "--token"; }));
                            cmdToken = args[++i];
                            break;
                        case "--mongo":
                            var i2 = Array.FindIndex<string>(args, new Predicate<string>((string val) => { return val == "--mongo"; }));
                            cmdMongoIP = args[++i2];
                            break;
                        case "--settings":
                            var i3 = Array.FindIndex<string>(args, new Predicate<string>((string val) => { return val == "--settings"; }));
                            cmdSettingsPath = args[++i3];
                            break;
                        case "--token-path":
                            var i4 = Array.FindIndex<string>(args, new Predicate<string>((string val) => { return val == "--token-path"; }));
                            cmdTokenPath = args[++i4];
                            break;
                    }
                }

                if (cmdToken != null && cmdTokenPath != null)
                {
                    Console.WriteLine("--token-path and --token are mutually exclusive!");
                    Environment.Exit(55);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid command line arguments.");
            }
            
        }


        private static void WriteCommandLineHelp(string program_name)
        {
            Console.WriteLine("Usage: {0} <command line options>");
        }

        
    }
}
