using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LightBlueFox.Util.Logging;

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

        
        [Command("inventory", RunMode = RunMode.Async)]
        [Alias("inv")]
        public async Task ShowInventory()
        {
            await Task.Run(() => {
                Logger.Log(LogLevel.DEBUG, "Handling show inventory command for user " + Context.User.Username + ":" + Context.User.Id + ".");
            });
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


        [Command("coins", RunMode = RunMode.Async)]
        public async Task GetCoinBalance(IUser targetUser = null)
        {
            User u;
            if (targetUser is null)
                u = User.FromID(Context.User.Id);
            else
                u = User.FromID(targetUser.Id);

            Logger.Log(LogLevel.INFO, "User " + Context.User.Username + ":" + Context.User.Id + " checking coin balance of " + (targetUser == null ? Context.User.Username : targetUser.Username) + ":" + u.UserID + ".");
            
            await Context.Channel.SendMessageAsync((targetUser == null ? (Context.User.Username + ", you have ") : (targetUser.Username + " has ")) + u.Coins + " coins.");
        }

        
        [Command("addcoins", RunMode = RunMode.Async)]
        public async Task AddCoins(int amount, IUser targetUser = null)
        {
            // Get the user that executed the command cast it to SocketGuildUser
            // so we can access the Roles property
            if (Context.User is SocketGuildUser user)
            {
                // Check if the user has the requried role
                if (user.Roles.Any(r => r.Name == "Developer"))
                {
                    User u;
                    if (targetUser is null)
                        u = User.FromID(Context.User.Id);
                    else
                        u = User.FromID(targetUser.Id);
                    u.Coins += amount;
                    Logger.Log(LogLevel.WARNING, "User " + Context.User.Username + ":" + Context.User.Id + " added " + amount + " coins to the account of " + (targetUser == null ? Context.User.Username : targetUser.Username) + ":" + u.UserID + " using the $addcoins command!");
                    await Context.Channel.SendMessageAsync("Successfully added " + amount + " coins to the account of " + (targetUser == null ? Context.User.Username : targetUser.Username) + ".");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Sorry, you don't have permission to do that.");
                }
            }

        }
    }
}
