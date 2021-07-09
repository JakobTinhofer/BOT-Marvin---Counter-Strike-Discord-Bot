using BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers
{
    public class ViewerPage
    {
        public List<ViewerAction> PageActions { get; private set; } = new List<ViewerAction>();
        public EmbedBuilder Page { get; private set; }

        public Embed Build()
        {
            return Page.Build();
        }

        public ViewerPage(EmbedBuilder builder) => Page = builder;
    }
}
