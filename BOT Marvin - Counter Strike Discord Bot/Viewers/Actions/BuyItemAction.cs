using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;

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
            throw new NotImplementedException();
        }
    }
}
