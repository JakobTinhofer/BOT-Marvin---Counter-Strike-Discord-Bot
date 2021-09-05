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
using LightBlueFox.Util;
using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
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
                    Context.Message.DeleteAsync();
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
                BOT_Marvin___Counter_Strike_Discord_Bot.Users.User u = User.FromID(Context.User.Id);
                
                if(u.InventorySize == 0)
                {
                    Context.Channel.SendMessageAsync("You do not have any items yet.");
                }
                else
                {
                    Context.Message.DeleteAsync();
                    SingleItemViewer v = new SingleItemViewer(u.GetItems(), Context.Channel, Context.User, 0);
                }

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
        private static Random r = new Random();
        public static Dictionary<ulong, LongCooldown> dropCooldownsByUser = new Dictionary<ulong, LongCooldown>();
        public static TimeSpan defaultCooldownLen = new TimeSpan(0, 45, 0);
        [Command("drop", RunMode = RunMode.Async)]
        public async Task DropCommand()
        {
            var userid = Context.User.Id;
            await Task.Run(() => {
                if (!Context.User.IsBot)
                {
                    var userid = Context.User.Id;
                    if (!dropCooldownsByUser.ContainsKey(userid))
                    {
                        dropCooldownsByUser.Add(userid, new LongCooldown(defaultCooldownLen));
                    }
                    if (dropCooldownsByUser[userid].Trigger())
                    {
                        Logger.Log(LogLevel.DEBUG, "Handling drop command...");
                        var col = MongoInterface.db.GetCollection<BsonDocument>("skins");
                        var filter = Builders<BsonDocument>.Filter.Lt("price", 500);
                        var items = col.Find(filter).ToList();
                        var itemBson = items[r.Next(0, items.Count)];
                        col = MongoInterface.db.GetCollection<BsonDocument>("items");
                        filter = Builders<BsonDocument>.Filter.Eq("_id", itemBson["item_id"]);
                        var item = new ItemHolder<Item>(col.Find<BsonDocument>(filter).FirstOrDefault());
                        User u = User.FromID(userid);
                        var l = new List<ItemHolder<Item>>();
                        l.Add(new ItemHolder<Item>(item.Get()));
                        u.AddItem(item.Get().ItemID);
                        SingleItemViewer v = new SingleItemViewer(l, Context.Channel, Context.User, 0, false);
                        v.Modifiers.Clear();
                        v.Modifiers.Add(new CaseWinningModifier());
                        v.UpdateAsync();
                        return;
                    }
                    else
                    {
                        var downtime = (dropCooldownsByUser[userid].Duration - (DateTime.Now - dropCooldownsByUser[userid].StartingPoint));
                        Context.Channel.SendMessageAsync("You cannot claim yet, you need to wait " + downtime.Minutes + " minutes " + downtime.Seconds + " seconds.");
                    }
                }
                else
                {
                    return;
                }
            });
            
        }

        [Command("help")]
        public async Task HelpCommand()
        {
            await Context.Channel.SendMessageAsync("You can view your coin balance using $coins. You will gain coins for being in a voice channel. Using those coins, buy cases from $cases. View all owned items using $inv. When viewing a case of which you own atleast one, you can open it to get a skin. Also, you can use $drop to get a random item.");
        }

        [Command("clear_inventory")]
        public async Task ClearInventory()
        {
            User u = User.FromID(Context.User.Id);
            u.ClearInventory();
            await Context.Channel.SendMessageAsync("Inventory cleared.");
            Logger.Log(LogLevel.WARNING, "User with id {0} cleared his inventory!", u.UserID);
        }
    }
}
