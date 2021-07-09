using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    /// <summary>
    /// The arguments given to all <see cref="ViewerAction"/>s.
    /// </summary>
    public class ActionArgs
    {
        /// <summary>
        /// The ItemViewer this action is associated to.
        /// </summary>
        public ItemViewer Sender;

        /// <summary>
        /// The User triggering the action.
        /// </summary>
        public IUser Actor;

        /// <summary>
        /// Creates the arguments for a new action.
        /// </summary>
        /// <param name="sender">The ItemViewer this action is associated to.</param>
        /// <param name="actor">The user that triggered the action.</param>
        public ActionArgs(ItemViewer sender, IUser actor) { Sender = sender; Actor = actor; }
    }
}
