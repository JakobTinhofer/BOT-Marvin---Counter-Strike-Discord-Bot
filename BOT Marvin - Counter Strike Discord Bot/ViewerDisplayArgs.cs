using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public class ViewerDisplayArgs
    {
        public SocketUser CommandRequester;
        public ItemViewer Sender;

        public ViewerDisplayArgs(SocketUser requester, ItemViewer Sender)
        {
            CommandRequester = requester; this.Sender = Sender;
        }
    }
}
