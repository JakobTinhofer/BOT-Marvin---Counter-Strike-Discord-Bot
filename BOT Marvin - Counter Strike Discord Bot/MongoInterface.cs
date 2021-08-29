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
        public static string connectionTemplate = "mongodb://{#mongo-ip#}:27017/?socketTimeoutMS=300";
        public static void Setup()
        {
            foreach (var ip in SettingsManager.MongoIPs)
            {
                Logger.Log(LogLevel.INFO, "Trying to connect to mongodb at {0}.", ip);
                try
                {
                    dbClient = new MongoClient(connectionTemplate.Replace("{#mongo-ip#}", "ip"));
                    Logger.Log(LogLevel.INFO, "Connected to mongodb!");
                    break;
                }
                catch (Exception e)
                {
                    dbClient = null;
                    Logger.Log(LogLevel.ERROR, "Tried to connect to mongodb at {0}, but the connection failed.", ip);
                    Logger.Log(LogLevel.DEBUG, "Error while trying to connect to mongo at {0}. Error: {1}.", ip, e);
                }
                
            }
            if(dbClient != null)
                db = dbClient.GetDatabase("csgo-discord-bot");
            else
            {
                Logger.Log(LogLevel.FATAL, "Did not find a valid mongo database! Please add the ip of the mongo database in the settings.xml file.");
                Environment.Exit(111);
            }
        }

        
    }
}
