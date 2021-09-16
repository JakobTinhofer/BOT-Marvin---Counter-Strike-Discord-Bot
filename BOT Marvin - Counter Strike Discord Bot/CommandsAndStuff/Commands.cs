using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LightBlueFox.Util;
using LightBlueFox.Util.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        #region List Cases

        [Command("cases", RunMode = RunMode.Async)]
        [Help("Lists available cases.")]
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

        #endregion

        #region Show Inventory

        [Command("inv", RunMode = RunMode.Async)]
        [Alias("inventory")]
        [Help("Shows your inventory.")]
        public async Task ShowInventory()
        {
            await Task.Run(() => {
                Logger.Log(LogLevel.DEBUG, "Handling show inventory command for user " + Context.User.Username + ":" + Context.User.Id + ".");
                BOT_Marvin___Counter_Strike_Discord_Bot.Users.User u = User.FromID(Context.User.Id);

                if (u.InventorySize == 0)
                {
                    Context.Channel.SendMessageAsync("You do not have any items yet.");
                }
                else
                {
                    Context.Message.DeleteAsync();
                    SingleItemViewer v = new SingleItemViewer(u.GetItems(), Context.Channel, Context.User, 0, new ViewerModifier[1] { new SellItemModifier() });
                }

            });
        }

        #endregion

        #region Skin List
        [Command("skins")]
        [Help("Lets you see all skins in the database.")]
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
        #endregion

        #region Coin Balance
        [Command("coins", RunMode = RunMode.Async)]
        [Help("Displays your coin balance, or, if targetUser is tagged (@Jakob), the coin balance of the target.")]
        public async Task GetCoinBalance([Help("The user targeted by this command. Defaults to the command sender. Tag the user to provide input here.")] IUser targetUser = null)
        {
            User u;
            if (targetUser is null)
            {
                u = User.FromID(Context.User.Id);
            }
            else
            {
                u = User.FromID(targetUser.Id);
            }

            Logger.Log(LogLevel.INFO, "User " + Context.User.Username + ":" + Context.User.Id + " checking coin balance of " + (targetUser == null ? Context.User.Username : targetUser.Username) + ":" + u.UserID + ".");

            await Context.Channel.SendMessageAsync((targetUser == null ? (Context.User.Username + ", you have ") : (targetUser.Username + " has ")) + u.Coins + " coins.");
        }
        #endregion

        #region Modify Coin Balance
        [Command("addcoins", RunMode = RunMode.Async)]
        [Help("Admin command. Allows admins to modify the coin balance of a user or themselves.")]
        public async Task AddCoins([Help("The amount of coins to add. Can be negative.")] int amount, [Help("The user whose coin balance should be modified. Defaults to command sender.")] IUser targetUser = null)
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
                    {
                        u = User.FromID(Context.User.Id);
                    }
                    else
                    {
                        u = User.FromID(targetUser.Id);
                    }

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
        #endregion

        #region Drop Command
        private static Random r = new Random();
        public static Dictionary<ulong, LongCooldown> dropCooldownsByUser = new Dictionary<ulong, LongCooldown>();
        public static TimeSpan defaultCooldownLen = new TimeSpan(0, 45, 0);
        [Command("drop", RunMode = RunMode.Async)]
        [Help("Drops a random item of low to medium value. Has a cooldown.")]
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
                        SingleItemViewer v = new SingleItemViewer(l, Context.Channel, Context.User, 0, displayImidiately: false);
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
        #endregion

        #region Help
        [Command("help")]
        [Help("Displays information about the bot, for example a list of commands.")]
        public async Task HelpCommand([Help("The command to get more information on. Defaults to a list of all commands.")]string command = null)
        {
            if(command == null)
            {
                EmbedBuilder em = new EmbedBuilder();
                em.Title = "Help";
                em.Color = Color.Blue;

                em.AddField(new EmbedFieldBuilder().WithName("Introduction").WithValue(SettingsManager.IntroductionString));
                em.WithFooter("Written by jakob using Discord.NET :)");


                em.AddField(new EmbedFieldBuilder().WithName("List of Commands").WithValue("Format: $command <optional parameter> [required parameter]"));
                foreach (var cmdHelp in CommandHandler.CommandDescriptions.OrderBy(x => x.Name))
                {
                    
                    EmbedFieldBuilder emfb = new EmbedFieldBuilder();
                    
                    
                    StringBuilder commandListBuilder = new StringBuilder();
                    commandListBuilder.Append("$" + cmdHelp.Name);
                    foreach (var item in cmdHelp.Parameters)
                    {
                        
                        if (item.Parameter.IsOptional)
                            commandListBuilder.Append(" <" + item.Name + ">");
                        else
                            commandListBuilder.Append(" [" + item.Name + "]");
                    }
                    emfb.Name = commandListBuilder.ToString();
                    emfb.Value = cmdHelp.Attribute.OneLineDesc;
                    em.AddField(emfb);
                }

                


                em.AddField(new EmbedFieldBuilder().WithName("More Info").WithValue("Want to know more about a command (like examples, parameter types, etc.)? Use $help <command-name>"));
                await Context.Channel.SendMessageAsync("", false, em.Build());
            }
            else
            {
                HelpInfo cmdHelp = CommandHandler.CommandDescriptions.Where(p => p.Name == command).FirstOrDefault();
                if(cmdHelp == default(HelpInfo))
                {
                    await Context.Channel.SendMessageAsync("This command is not known! Use $help without parameters to see a list of commands.");
                }
                else
                {
                    EmbedBuilder em = new EmbedBuilder();
                    em.Color = Color.Green;
                    em.Title = "Help - " + command;
                    em.WithFooter("Written by jakob using Discord.NET :)");
                    if (cmdHelp.Attribute.LongDescription != null)
                        em.AddField(new EmbedFieldBuilder().WithName("Description").WithValue(cmdHelp.Attribute.LongDescription));
                    else
                        em.AddField(new EmbedFieldBuilder().WithName("Description").WithValue(cmdHelp.Attribute.OneLineDesc));

                    StringBuilder commandListBuilder = new StringBuilder();
                    commandListBuilder.Append("$" + cmdHelp.Name);


                    EmbedFieldBuilder emfb = new EmbedFieldBuilder();
                    if(cmdHelp.Parameters.Count > 0)
                    {
                        emfb.Name = "List of Parameters";
                        emfb.Value = "Describes all the parameters and what they do.";
                        em.AddField(emfb);
                        
                        foreach (var item in cmdHelp.Parameters)
                        {
                            emfb.Name = item.Name;
                            emfb.Value = item.Attribute.OneLineDesc;



                            if (item.Parameter.IsOptional)
                            {
                                commandListBuilder.Append(" <" + item.Name + ">");
                                emfb.Value += "This parameter is optional. ";
                            }
                            else
                                commandListBuilder.Append(" [" + item.Name + "]");

                            if (item.Attribute.Example != null)
                                emfb.Value += "\nExample: " + item.Attribute.Example;
                        }
                        emfb = new EmbedFieldBuilder();
                    }
                    
                    emfb.Name = "Usage";
                    emfb.Value = commandListBuilder.ToString();
                    em.AddField(emfb);
                    emfb = new EmbedFieldBuilder();
                    if (cmdHelp.Attribute.Example != null)
                        em.AddField(new EmbedFieldBuilder().WithName("Example command").WithValue(cmdHelp.Attribute.Example));

                    emfb.Name = "More Commands";
                    emfb.Value = "If you want to see a list of all commands, use $help without any parameters.";
                    em.AddField(emfb);
                    await Context.Channel.SendMessageAsync(embed: em.Build());
                }
            }
        }
        #endregion

        #region Clear Inventory
        [Command("clear_inventory")]
        [Help("Clears your inventory. Warning: this action is irreversible!")]
        public async Task ClearInventory()
        {
            User u = User.FromID(Context.User.Id);
            u.ClearInventory();
            await Context.Channel.SendMessageAsync("Inventory cleared.");
            Logger.Log(LogLevel.WARNING, "User with id {0} cleared his inventory!", u.UserID);
        }
        #endregion
    }
}
