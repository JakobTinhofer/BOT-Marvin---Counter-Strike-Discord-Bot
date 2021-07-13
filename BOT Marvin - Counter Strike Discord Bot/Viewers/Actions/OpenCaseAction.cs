using BOT_Marvin___Counter_Strike_Discord_Bot.Items;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

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
            
        }
    }
}
