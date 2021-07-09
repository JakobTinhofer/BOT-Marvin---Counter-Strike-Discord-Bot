//using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
//using Discord;
//using Discord.Rest;
//using Discord.WebSocket;
//using MongoDB.Bson;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace BOT_Marvin___Counter_Strike_Discord_Bot
//{
//    //public delegate void ViewerModifier(EmbedBuilder em);
//    public class ItemListViewer
//    {

//        #region Members

//        private int index;
//        private ISocketMessageChannel channel;
//        private List<Item> items = new List<Item>();
//        public RestUserMessage currentMessage;
//        private List<ViewerAction> specificActions = new List<ViewerAction>();
//        private ViewerModes mode;
//        private SocketUser commandSender;
//        #endregion

//        #region Default Actions and Modifiers

//        private static ViewerAction navLeft = new ViewerAction(new Emoji("⬅"), Navigate, true);
//        private static ViewerAction navRight = new ViewerAction(new Emoji("➡"), Navigate, true);
//        private static ViewerAction closeViewer = new ViewerAction(new Emoji("❌"), CloseViewer__Handler, true);

//        private static void InventoryContentDisplayModifier(ItemListViewer v, EmbedBuilder em, BsonDocument item)
//        {

//        }

//        public static void Navigate(ItemListViewer viewer, ViewerAction sender)
//        {

//            if (sender == navLeft)
//                viewer.index -= viewer.index > 0 ? 1 : 0;
//            else if (sender == navRight)
//                viewer.index += viewer.index - 1 < viewer.items.Count ? 1 : 0;
//            viewer.Display(viewer.channel, viewer.items[viewer.index]);
//        }



//        #endregion

//        #region Close

//        public void CloseViewer()
//        {
//            Program.ClearAllActions(currentMessage);
//            currentMessage.DeleteAsync().Wait();
//        }

//        private static void CloseViewer__Handler(ItemListViewer v, ViewerAction sender)
//        {
//            v.CloseViewer();
//        }

//        #endregion

        

        

       

//        public ItemListViewer(List<BsonDocument> items, int startIndex, ISocketMessageChannel channel, ViewerModes mode, SocketUser requester)
//        {
//            this.channel = channel;
//            this.mode = mode;
//            commandSender = requester;
//            this.index = startIndex;
//            foreach(BsonDocument b in items)
//            {
//                this.items.Add(Item.Parse(b));
//            }
//            Display(channel, this.items[startIndex]);
//        }

//        public ItemListViewer(List<Item> items, int startIndex, ISocketMessageChannel channel, ViewerModes mode, SocketUser requester)
//        {
//            this.items = items;
//            this.index = startIndex;
//            this.channel = channel;
//            this.mode = mode;
//            commandSender = requester;
//        }

//        private void Display(ISocketMessageChannel channel, Item item)
//        {
            

//            EmbedBuilder em = new EmbedBuilder();
//            em.Title = item.Name;
//            em.ImageUrl = item.ImageURL;
//            List<ViewerAction> actionsToAdd = new List<ViewerAction>();
//            if (currentMessage == null)
//            {
//                currentMessage = channel.SendMessageAsync("", false, em.Build()).GetAwaiter().GetResult();
//                actionsToAdd.Add(navLeft);
//                actionsToAdd.Add(navRight);
//                actionsToAdd.Add(closeViewer);
//            }
//            else
//            {
//                Program.RemoveMultipleActions(currentMessage, specificActions);
//                specificActions.Clear();
//                currentMessage.ModifyAsync(msg => msg.Embed = em.Build());
//            }

            
//            foreach (ViewerAction a in item.GetSpecificActions(new ViewerDisplayArgs(commandSender, mode)))
//            {
//                actionsToAdd.Add(a);
//                if (a.SetEmoji)
//                {
                    
//                    specificActions.Add(a);
//                }
//            }


//            item.ModifyEmbed(new ViewerDisplayArgs(commandSender, mode), em);
//            currentMessage.ModifyAsync(msg => msg.Embed = em.Build());
//        }
//    }

//    public enum ViewerModes
//    {
//        cases,
//        inventory
//    }
//}
