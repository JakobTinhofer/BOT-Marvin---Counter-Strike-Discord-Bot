using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    public delegate void CustomViewerActionHandler(ActionArgs args, CustomViewerAction sender);
    public class CustomViewerAction : ViewerAction
    {
        public event CustomViewerActionHandler OnCustomViewerActionTriggered;
        public CustomViewerAction(CustomViewerActionHandler handler, IEmote e, bool autoset, SocketUser usableBy, bool send403Msg) : base(e, autoset, usableBy, send403Msg)
        {
            OnCustomViewerActionTriggered += handler;
        }
        public CustomViewerAction(CustomViewerActionHandler handler, IEmote e, bool autoset) : base(e, autoset)
        {
            OnCustomViewerActionTriggered += handler;
        }

        public override void TriggerAction(ActionArgs args)
        {
            OnCustomViewerActionTriggered?.Invoke(args, this);
        }
    }
}
