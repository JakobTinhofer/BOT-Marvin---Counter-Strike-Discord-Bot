using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    
    public abstract class ViewerAction
    {
        public IEmote TriggeringEmoji { get; private set; }
        public bool SetEmoji { get; private set; }
        public SocketUser usableBy { get; private set; }
        public bool send403Msg { get; private set; }

        public ViewerAction(IEmote e, bool autoset)
        {
            TriggeringEmoji = e;
            SetEmoji = autoset;
        }

        public ViewerAction(IEmote e, bool autoset, SocketUser usableBy, bool send403Msg)
        {
            TriggeringEmoji = e;
            SetEmoji = autoset;
            this.usableBy = usableBy;
            this.send403Msg = send403Msg;
        }

        public abstract void TriggerAction(ActionArgs args);
    }
}
