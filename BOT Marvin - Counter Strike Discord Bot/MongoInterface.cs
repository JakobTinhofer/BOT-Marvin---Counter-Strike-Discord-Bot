using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBlueFox.Util.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class MongoInterface
    {
        private static MongoClient dbClient;
        public static IMongoDatabase db;
        public static string connectionTemplate = "mongodb://{#mongo-ip#}/";
        public static void Setup()
        {
            if (SettingsManager.IsDevEnv)
                SettingsManager.MongoIPs.Insert(0, "localhost:27017");


            foreach (var ip in SettingsManager.MongoIPs)
            {
                Logger.Log(LogLevel.INFO, "Trying to connect to mongodb at {0}.", connectionTemplate.Replace("{#mongo-ip#}", ip));
                try
                {
                    var settings = MongoClientSettings.FromConnectionString(connectionTemplate.Replace("{#mongo-ip#}", ip));
                    settings.SocketTimeout = TimeSpan.FromMilliseconds(300);
                    settings.ConnectTimeout = TimeSpan.FromMilliseconds(300);
                    dbClient = new MongoClient(settings);
                    if (dbClient.GetDatabase("csgo-discord-bot").RunCommandAsync((Command<BsonDocument>)"{ping:1}").GetAwaiter().GetResult().GetValue("ok") != 1)
                        throw new Exception("Ping failed!");
                    Logger.Log(LogLevel.INFO, "Connected to mongodb!");
                    Logger.Log(LogLevel.DEBUG, "DBClient: {0}", dbClient.GetDatabase("csgo-discord-bot"));
                    break;
                }
                catch (Exception e)
                {
                    dbClient = null;
                    Logger.Log(LogLevel.ERROR, "Tried to connect to mongodb at {0}, but the connection failed.", ip);
                    Logger.Log(LogLevel.DEBUG, "Error while trying to connect to mongo at {0}. Error: {1}.", ip, e);
                }

            }
            if (dbClient != null)
                db = dbClient.GetDatabase("csgo-discord-bot");
            else
            {
                Logger.Log(LogLevel.FATAL, "Did not find a valid mongo database! Please add the ip of the mongo database in the settings.xml file.");
                Environment.Exit(111);
            }
        }
        
    }
}
