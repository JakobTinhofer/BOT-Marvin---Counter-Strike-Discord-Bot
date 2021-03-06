using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBlueFox.Util.Logging;
using System.Threading;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers
{
    public abstract class ItemViewer
    {
        private static CloseViewerAction closeViewer = new CloseViewerAction();

        #region Private Members
        private RestUserMessage CurrentMessage;
        #endregion

        #region Public & Protected Members
        protected ISocketMessageChannel Channel { private set; get; }
        protected SocketUser User { private set; get; }
        public List<ItemHolder<Item>> Items { get; private set; } = new List<ItemHolder<Item>>();
        public ViewerPage Page { get; private set; }
        public List<ViewerModifier> Modifiers;
        protected abstract ViewerPage Display();

        protected bool isDestroyed = false;
        #endregion

        #region Methods
        private AutoResetEvent isUpdateFree = new AutoResetEvent(true);
        public async void UpdateAsync()
        {
            
            await Task.Run(() => {
                if (isDestroyed)
                    return;
                isUpdateFree.WaitOne();
                Logger.Log(LogLevel.DEBUG, "Running Update!");
                
                Page = Display();
                foreach (var mod in Modifiers)
                {
                    mod.Modify(Page, new ViewerDisplayArgs(User, this));
                }
                foreach (var item in Page.Fields.Values)
                {
                    Page.Page.AddField(item);
                }
                Logger.Log(LogLevel.DEBUG, "---> Finished building page and modifying it up to " + ViewerModifier.KnownModifiers.Count + " times!");
                if (CurrentMessage == null)
                {
                    CurrentMessage = Channel.SendMessageAsync(null, false, Page.Build()).Result;
                    ViewerByMessageID.Add(CurrentMessage.Id, this);
                    Logger.Log(LogLevel.DEBUG, "---> Sent message!");
                }
                else
                {
                    CurrentMessage.ModifyAsync(msg => msg.Embed = Page.Build());
                    Logger.Log(LogLevel.DEBUG, "---> Updated message!");
                }
                
                Dictionary<IEmote, ViewerAction> newActions = new Dictionary<IEmote, ViewerAction>();
                Page.PageActions.Add(closeViewer);
                foreach (ViewerAction a in Page.PageActions)
                {
                    newActions.Add(a.TriggeringEmoji, a);
                    if (!actionsByEmoji.ContainsKey(a.TriggeringEmoji))
                    {
                        
                        if (a.SetEmoji)
                            CurrentMessage.AddReactionAsync(a.TriggeringEmoji);

                    }
                }
                actionsByEmoji = newActions;
                List<IEmote> reactionsToRemove = new List<IEmote>();
                CurrentMessage.UpdateAsync().Wait();
                foreach (var item in CurrentMessage.Reactions)
                {
                    if (!actionsByEmoji.ContainsKey(item.Key))
                        reactionsToRemove.Add(item.Key);
                }
                foreach (var r in reactionsToRemove)
                {
                    Logger.Log(LogLevel.DEBUG, "Removing {0} reaction.", r.Name);
                    CurrentMessage.RemoveAllReactionsForEmoteAsync(r).Wait();
                }

                Logger.Log(LogLevel.DEBUG, "---> Registered " + newActions.Count + " actions!");
                isUpdateFree.Set();
            });
            
        }

        public void Destroy()
        {
            if (isDestroyed)
                return;
            isDestroyed = true;
            ViewerByMessageID.Remove(CurrentMessage.Id);
            actionsByEmoji.Clear();
            CurrentMessage.DeleteAsync();
            Items.Clear();
        }
        #endregion

        #region Constructors And Static Viewer Dictionaries
        public static Dictionary<ulong, ItemViewer> ViewerByMessageID = new Dictionary<ulong, ItemViewer>();

        

        public ItemViewer(List<ItemHolder<Item>> items, ISocketMessageChannel channel, SocketUser requester, ViewerModifier[] mods = null, bool displayImidiately = true)
        {
            Modifiers = new List<ViewerModifier>(ViewerModifier.KnownModifiers);
            Items = items;
            Channel = channel;
            User = requester;
            if (mods != null)
            {
                foreach (var item in mods)
                {
                    Modifiers.Add(item);
                }
            }
            if(displayImidiately)
                UpdateAsync();
        }

        public ItemViewer(List<BsonDocument> items, ISocketMessageChannel channel, SocketUser requester, ViewerModifier[] mods = null, bool displayImidiately = true)
        {
            Modifiers = new List<ViewerModifier>(ViewerModifier.KnownModifiers);
            Logger.Log(LogLevel.DEBUG, "Creating new Item Viewer from BSON Documents!");
            Channel = channel;
            User = requester;
            foreach (BsonDocument b in items)
            {
                this.Items.Add(new ItemHolder<Item>(b));
            }

            if (mods != null)
            {
                foreach (var item in mods)
                {
                    Modifiers.Add(item);
                }
            }

            Logger.Log(LogLevel.DEBUG, "Finished Parsing " + Items.Count + " items!");
            if(displayImidiately)
                UpdateAsync();
        }

        #endregion

        #region ViewerAction Handeling
        private Dictionary<IEmote, ViewerAction> actionsByEmoji = new Dictionary<IEmote, ViewerAction>();
        public void HandleAction(IEmote actionEmoji, IUser actor)
        {
            if (actionsByEmoji.ContainsKey(actionEmoji))
            {
                ViewerAction action = actionsByEmoji[actionEmoji];
                if (action.usableBy != null && action.usableBy.Id != actor.Id)
                {
                    if(action.send403Msg)
                        Channel.SendMessageAsync("You are not allowed to interact with this, only " + action.usableBy.Username + " is!");
                }
                else
                {
                    action.TriggerAction(new ActionArgs(this, actor, Channel));
                    UpdateAsync();  
                }
                
            }
        }

        #endregion

    }
}
