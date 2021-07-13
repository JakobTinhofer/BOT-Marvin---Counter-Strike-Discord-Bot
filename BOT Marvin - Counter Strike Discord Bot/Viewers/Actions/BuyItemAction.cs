using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;
using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using BOT_Marvin___Counter_Strike_Discord_Bot.Items;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    /// <summary>
    /// An action allowing a certain user to buy an item.
    /// </summary>
    public class BuyItemAction : ViewerAction
    {
        /// <summary>
        /// Creates a new BuyItemAction. This can, in a <see cref="ViewerModifier"/>, be added to the <see cref="ViewerPage.PageActions"/>.
        /// </summary>
        /// <param name="buyer">The user that is allowed to buy the item. If null, everyone can buy the item.</param>
        public BuyItemAction(SocketUser buyer = null) : base(new Emoji("💰"), true, buyer, true) { }

        /// <summary>
        /// Called from the <see cref="ItemViewer"/> when the action is triggered. Inherited from <see cref="ViewerAction"/>.
        /// </summary>
        /// <param name="args">The arguments given to the action.</param>
        public override void TriggerAction(ActionArgs args)
        {
            SingleItemViewer siv = args.Sender as SingleItemViewer;
            if (siv == null)
                throw new InvalidOperationException("This action cannot be triggered on a viewer different than the SingleItemViewer!");
            IEconomyItem i = siv.GetDisplayedItem() as IEconomyItem;
            if (i == null)
                throw new InvalidOperationException("This action can only be triggered on economy items!");
            User u = User.FromID(args.Actor.Id);

            if (u.Coins >= i.GetPrice())
            {
                u.Coins -= i.GetPrice();
                u.AddItem(((Item)i).ItemID);
                args.Sender.UpdateAsync();
                Logger.Log(LogLevel.INFO, "User " + args.Actor.Username + " bought item '" + ((Item)i).Name + "'.");
            }
            else
            {
                args.Channel.SendMessageAsync("You do not have enough coins to buy this case! Use $coins in order to see how many coins you have.");
            }

        }
    }
}
