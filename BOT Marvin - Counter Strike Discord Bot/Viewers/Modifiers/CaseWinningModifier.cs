using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{
    [ViewerModifier(false)]
    public class CaseWinningModifier : ViewerModifier
    {
        protected override bool _isApplicable(ViewerPage page, ViewerDisplayArgs args)
        {
            var s = args.Sender as SingleItemViewer;
            if(s != null && (s.GetDisplayedItem() as SkinItem) != null)
            {
                return true;
            }
            return false;
        }

        protected override ViewerPage _modify(ViewerPage page, ViewerDisplayArgs args)
        {
            page.Fields.Clear();

            SingleItemViewer s = args.Sender as SingleItemViewer;
            var c = ((SkinItem)s.GetDisplayedItem()).Rarity.Color;
            page.Page.Color = new Discord.Color(c.R, c.G, c.B);
            page.AddField("Congratulations!", "You won this item!");
            return page;
        }
    }
}
