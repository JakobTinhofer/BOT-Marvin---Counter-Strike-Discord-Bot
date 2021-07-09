using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    public class CloseViewerAction : ViewerAction
    {
        public CloseViewerAction() : base(new Emoji("❌"), true) { }

        public override void TriggerAction(ActionArgs args)
        {
            args.Sender.Destroy();
        }
    }
}
