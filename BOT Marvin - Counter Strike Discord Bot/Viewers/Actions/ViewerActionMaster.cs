using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Actions
{
    public static class ViewerActionMaster
    {
        public static async Task ReactionHandler(Cacheable<IUserMessage, UInt64> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.GetValueOrDefault().IsBot)
                return;

            ItemViewer.ViewerByMessageID.TryGetValue((await message.DownloadAsync()).Id, out ItemViewer v);
            if(v != null)
            {
                v.HandleAction(reaction.Emote, reaction.User.Value);
            }
        }

    }
}
