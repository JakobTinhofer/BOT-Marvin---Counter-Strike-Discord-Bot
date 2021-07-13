using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers
{
    public class SingleItemViewer : ItemViewer
    {
        public static CustomViewerAction navLeft = new CustomViewerAction(NavigationHandler, new Emoji("⬅"), true);
        public static CustomViewerAction navRight = new CustomViewerAction(NavigationHandler, new Emoji("➡"), true);

        public int Index { get; private set; }

        public SingleItemViewer(List<BsonDocument> items, ISocketMessageChannel channel, SocketUser requester, int startIndex) : base(items, channel, requester)
        {
            Index = startIndex;
        }

        public override ViewerPage Display()
        {
            Item i = Items[Index].Get();
            EmbedBuilder eb = new EmbedBuilder();
            var p = new ViewerPage(eb);
            if (i.getType() == ItemType.Skin)
                eb.Title = ((SkinItem)i).WeaponName + " | " + i.Name;
            else
                eb.Title = i.Name;


            p.AddField("Description", i.Description);
            string pr = ((IEconomyItem)i).GetPrice().ToString();
            p.AddField("Price", pr + " coins");
            //if (pr.Length > 2)
            //    p.AddField("Price", pr.Insert(pr.Length - 2, ",") + "€");
            //else
            //    p.AddField("Price", pr + " cents");
            p.AddField("Navigation", "React with ⬅ to navigate left, or with ➡ to navigate right. React with ❌ to close the viewer!");
            
            eb.ImageUrl = i.ImageURL;
            
            Logger.Log(LogLevel.DEBUG, "Displaying item with index " + Index + " and name " + i.Name + ".");
            if (Index > 0)
            {
                p.PageActions.Add(navLeft);
            }
            if(Index < Items.Count - 1)
            {
                p.PageActions.Add(navRight);
            }

            return p;

        }

        public Item GetDisplayedItem()
        {
            return Items[Index].Get();
        }

        private static void NavigationHandler(ActionArgs a, CustomViewerAction sender)
        {
            Logger.Log(LogLevel.DEBUG, "Handling a navigation action by " + a.Actor.Username + ".");
            SingleItemViewer viewer = (SingleItemViewer)a.Sender;
            if (sender == navLeft)
                viewer.Index -= viewer.Index > 0 ? 1 : 0;
            else if (sender == navRight)
                viewer.Index += viewer.Index - 1 < viewer.Items.Count ? 1 : 0;
            viewer.UpdateAsync();
        }
    }
}
