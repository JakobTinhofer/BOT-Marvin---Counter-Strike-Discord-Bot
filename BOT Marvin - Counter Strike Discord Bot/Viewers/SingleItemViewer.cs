using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using LightBlueFox.Util.Logging;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers
{
    public class SingleItemViewer : ItemViewer
    {
        public static CustomViewerAction navLeft = new CustomViewerAction(NavigationHandler, new Emoji("⬅"), true);
        public static CustomViewerAction navRight = new CustomViewerAction(NavigationHandler, new Emoji("➡"), true);

        public int Index { get; private set; }

        public SingleItemViewer(List<BsonDocument> items, ISocketMessageChannel channel, SocketUser requester, int startIndex, ViewerModifier[] mods = null, bool displayImidiately = true) : base(items, channel, requester, mods, displayImidiately)
        {
            Index = startIndex;
        }

        public SingleItemViewer(List<ItemHolder<Item>> items, ISocketMessageChannel channel, SocketUser requester, int startIndex, ViewerModifier[] mods = null, bool displayImidiately = true) : base(items, channel, requester, mods, displayImidiately)
        {
            Index = startIndex;
        }


        protected override ViewerPage Display()
        {
            Item i = Items[Index].Get();
            EmbedBuilder eb = new EmbedBuilder();
            var p = new ViewerPage(eb);
            if (i.getType() == ItemType.Skin)
            {
                SkinItem s = i as SkinItem;
                eb.Color = s.Rarity.Color;
                if ((s.Modifier & SkinModifier.StatTrak) == SkinModifier.StatTrak)
                    eb.Title = "StatTrakᵀᴹ ";
                if ((s.Modifier & SkinModifier.Souvenir) == SkinModifier.Souvenir)
                    eb.Title = "Souvenier ";
                eb.Title += ((SkinItem)i).WeaponName + " | " + i.Name;

                string pr;
                if(s.Modifier != SkinModifier.Normal)
                    pr = s.ModifiedPrice[s.Modifier].ToString();
                else
                    pr = ((IEconomyItem)i).GetPrice().ToString();
                p.AddField("Price", pr + " coins");
            }
            else 
            {
                eb.Title = i.Name;
                if(i as IEconomyItem != null)
                {
                    string pr = ((IEconomyItem)i).GetPrice().ToString();
                    p.AddField("Price", pr + " coins");
                }
            }
                


            p.AddField("Description", i.Description);
            
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
