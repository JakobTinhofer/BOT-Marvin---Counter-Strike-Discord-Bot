using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using LightBlueFox.Util;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    public class OpenCaseAction : ViewerAction
    {
        public OpenCaseAction(SocketUser usableBy, bool send403Msg) : base(new Emoji("🔑"), true, usableBy, send403Msg) { }

        public override void TriggerAction(ActionArgs args)
        {

            SingleItemViewer siv = args.Sender as SingleItemViewer;
            if (siv == null)
                throw new InvalidOperationException("This action cannot be triggered on a viewer different than the SingleItemViewer!");
            User u = User.FromID(args.Actor.Id);
            CaseItem i = siv.GetDisplayedItem() as CaseItem;
            if (i == null)
                throw new InvalidOperationException("This action can only be triggered on case items!");

            if(u.CountItem(i.ItemID) < 1)
            {
                args.Channel.SendMessageAsync("You do not own this case!");
                return;
            }


            if (i.RequiresKey)
            {
                if(u.Coins >= 210)
                    u.Coins -= 210;
                else
                {
                    args.Channel.SendMessageAsync("You do not have enough coins to buy this case! Use $coins in order to see how many coins you have.");
                    return;
                }
            }

            var item = i.OpenCase();
            var l = new List<ItemHolder<Item>>();
            l.Add(new ItemHolder<Item>(item.Get()));
            u.AddItem(item.Get().ItemID);
            u.RemoveItem(i.ItemID);
            args.Sender.Destroy();
            SingleItemViewer v = new SingleItemViewer(l, args.Channel, (SocketUser)args.Actor, 0, displayImidiately: false);
            v.Modifiers.Clear();
            v.Modifiers.Add(new CaseWinningModifier());
            v.UpdateAsync();
        }
    }
}
