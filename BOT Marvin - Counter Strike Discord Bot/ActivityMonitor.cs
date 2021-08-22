using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class ActivityMonitor
    {
        private static DiscordSocketClient Client;
        public static void Init(DiscordSocketClient client)
        {
            Client = client;
        }

        public static List<ulong> GetActiveUsers()
        {
            List<ulong> userIDs = new List<ulong>();
            foreach (var guild in Client.Guilds)
            {
                foreach (SocketVoiceChannel channel in guild.VoiceChannels)
                {
                    
                    foreach (var user in channel.Users)
                    {
                        if (user.IsBot || user.IsSelfDeafened)
                            continue;
                        userIDs.Add(user.Id);
                    }
                }
            }
            return userIDs;
        }
    }
}
