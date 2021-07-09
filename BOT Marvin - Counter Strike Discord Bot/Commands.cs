using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("cases", RunMode = RunMode.Async)]
        public async Task ListCasesCommand()
        {
            await Task.Run(() => {
                try
                {
                    Logger.Log(LogLevel.DEBUG, "Handling cases command...");
                    var col = MongoInterface.db.GetCollection<BsonDocument>("items");
                    var filter = Builders<BsonDocument>.Filter.Eq("item-type", "Case");
                    var items = col.Find(filter).ToList();
                    Logger.Log(LogLevel.DEBUG, "Cases retrieved from database. Total: " + items.Count + " cases.");
                    SingleItemViewer v = new SingleItemViewer(items, Context.Channel, Context.User, 0);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.ERROR, e.ToString());
                }
                
            });
            
        }

        
        [Command("inventory")]
        [Alias("inv")]
        public async Task ShowInventory()
        {

        }

        [Command("skins")]
        public async Task BrowseSkins()
        {
            await Task.Run(() => {
                Logger.Log(LogLevel.DEBUG, "Handling skin browser command...");
                var col = MongoInterface.db.GetCollection<BsonDocument>("items");
                var filter = Builders<BsonDocument>.Filter.Eq("item-type", "Skin");
                var items = col.Find(filter).ToList();
                Logger.Log(LogLevel.DEBUG, "Skins retrieved from database. Total: " + items.Count + " skins.");
                SingleItemViewer v = new SingleItemViewer(items, Context.Channel, Context.User, 0);
            });
        }

    }
}
