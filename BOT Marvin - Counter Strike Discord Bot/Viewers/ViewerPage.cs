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
        public Dictionary<string, EmbedFieldBuilder> Fields { get; private set; } = new Dictionary<string, EmbedFieldBuilder>();
        public Embed Build()
        {
            return Page.Build();
        }

        public void AddField(string name, object obj)
        {
            if (Fields.ContainsKey(name))
                throw new InvalidOperationException("Field with that name already added!");

            var emf = new EmbedFieldBuilder();
            emf.Name = name;
            emf.Value = obj;
            Fields.Add(name, emf);
        }

        public ViewerPage(EmbedBuilder builder) => Page = builder;
    }
}
