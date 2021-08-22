using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public class ActivityMonitor
    {
        private DiscordRestClient restClient;
        private DiscordSocketClient client;
        private SocketGuild guild;
        public ActivityMonitor(string token, ulong guildID, DiscordSocketClient client)
        {
            this.client = client;
            restClient = new DiscordRestClient();
            restClient.LoginAsync(TokenType.Bot, token).Wait();
            guild = client.GetGuild(guildID);
        }

        public List<ulong> GetActiveUsers()
        {
            List<ulong> userIDs = new List<ulong>();
            foreach(var channel in guild.Channels)
            {
                foreach (var user in channel.Users)
                {
                    userIDs.Add(user.Id);
                }
            }
            return userIDs;
        }
    }
}
