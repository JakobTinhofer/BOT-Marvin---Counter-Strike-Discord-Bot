using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{
    public class SellItemModifier : ViewerModifier
    {
        protected override bool _isApplicable(ViewerPage page, ViewerDisplayArgs args)
        {
            SingleItemViewer s = args.Sender as SingleItemViewer;
            if (s == null)
                return false;
            User u = User.FromID(args.CommandRequester.Id);
            if((s.GetDisplayedItem() as IEconomyItem) == null)
                return false;
            if(u.CountItem(s.GetDisplayedItem().ItemID) > 0)
            {
                return true;
            }
            return false;
        }

        protected override ViewerPage _modify(ViewerPage page, ViewerDisplayArgs args)
        {
            page.PageActions.Add(new CustomViewerAction(sellItem, new Emoji("💵"), true, args.CommandRequester, true));
            return page;
        }

        private void sellItem(ActionArgs args, CustomViewerAction sender)
        {
            SingleItemViewer s = args.Sender as SingleItemViewer;
            User u = User.FromID(args.Actor.Id);

            u.RemoveItem(s.GetDisplayedItem().ItemID);
            u.Coins += ((IEconomyItem)s.GetDisplayedItem()).GetPrice();

            args.Channel.SendMessageAsync(args.Actor.Username + " sold his " + s.GetDisplayedItem().Name + " for " + ((IEconomyItem)s.GetDisplayedItem()).GetPrice() + " coins.");
        }
    }
}
