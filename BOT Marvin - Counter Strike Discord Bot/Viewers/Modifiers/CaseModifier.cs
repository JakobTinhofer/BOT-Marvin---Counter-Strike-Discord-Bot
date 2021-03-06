using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{
    [ViewerModifier(true)]
    public class CaseModifier : ViewerModifier
    {
        protected override bool _isApplicable(ViewerPage page, ViewerDisplayArgs args)
        {

            var s = args.Sender as SingleItemViewer;
            if (s != null)
            {
                CaseItem i = s.GetDisplayedItem() as CaseItem;
                if (i != null && ((Item)i).getType() == ItemType.Case)
                    return true;
            }
            return false;
        }

        protected override ViewerPage _modify(ViewerPage page, ViewerDisplayArgs args)
        {

            var siv = args.Sender as SingleItemViewer;
            CaseItem it = siv.GetDisplayedItem() as CaseItem;
            if (it.RequiresKey)
                page.Fields["Description"].Value += "This case requires a key to open. It will cost an additional 210 coins.";

            User u = User.FromID(args.CommandRequester.Id);
            if(u.CountItem(it.ItemID) > 0)
            {
                page.PageActions.Add(new Actions.OpenCaseAction(args.CommandRequester, true));
                page.AddField("Open this crate", args.CommandRequester.Username + ", since you own " + u.CountItem(it.ItemID) + " crates you can open this crate by reacting with " + "🔑" + ".");
            }

            return page;
        }
    }
}
