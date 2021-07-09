using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class MongoInterface
    {
        private static MongoClient dbClient;
        public static IMongoDatabase db;
        public static void Setup()
        {
            dbClient = new MongoClient("mongodb://localhost:27017/");
            db = dbClient.GetDatabase("csgo-discord-bot");
        }

        
    }
}
