using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{
    [ViewerModifier(true)]
    public class ShopModifier : ViewerModifier
    {
        

        protected override bool _isApplicable(ViewerPage page, ViewerDisplayArgs args)
        {
            var s = args.Sender as SingleItemViewer;
            if (s != null) {
                
                IEconomyItem i = s.GetDisplayedItem() as IEconomyItem;

                if (i != null && ((Item)i).getType() == ItemType.Case)
                    return true;
                
            }
            return false;
        }

        protected override ViewerPage _modify(ViewerPage page, ViewerDisplayArgs args)
        {

            var siv = args.Sender as SingleItemViewer;

            IEconomyItem iei = siv.GetDisplayedItem() as IEconomyItem;
            CaseItem ci = siv.GetDisplayedItem() as CaseItem;

            page.Page.AddField("Buy this case", "React with 💰 to buy this case.");
            page.PageActions.Add(new BuyItemAction(args.CommandRequester));

            return page;
        }
    }
}
