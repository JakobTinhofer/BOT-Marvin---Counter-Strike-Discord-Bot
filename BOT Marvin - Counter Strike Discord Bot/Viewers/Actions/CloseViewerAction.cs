using Discord;
using System;
using System.Collections.Generic;
using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    /// <summary>
    /// This action will close the viewer when clicked
    /// TODO: This can cause an exception when 2 interactions come in close after each other (see <see cref="ItemViewer.Destroy"/>).
    /// </summary>
    public class CloseViewerAction : ViewerAction
    {
        /// <summary>
        /// Creates a new action to be added to <see cref="ViewerPage.PageActions"/>. (See <see cref="ViewerModifier"/>)
        /// </summary>
        public CloseViewerAction() : base(new Emoji("❌"), true) { }

        /// <summary>
        /// Inherited from <see cref="ViewerAction"/>. Called whe the action is triggered.
        /// </summary>
        public override void TriggerAction(ActionArgs args)
        {
            args.Sender.Destroy();
        }
    }
}
