using BOT_Marvin___Counter_Strike_Discord_Bot.Users;
using LightBlueFox.Util.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class ActivityRewarder
    {
        public static ManualResetEvent Enabled = new ManualResetEvent(true);
        public static int RewardAmount = 1;
        public static int IntervalInMilliseconds = 10000;

        public static async void StartGivingActivityRewards()
        {
            await Task.Run(() => {
                while (true)
                {
                    Enabled.WaitOne();
                    var users = ActivityMonitor.GetActiveUsers();
                    foreach (var user in users)
                    {
                        User u = User.FromID(user);
                        u.Coins += RewardAmount;
                        Logger.Log(LogLevel.DEBUG, "Gave {0} coins to user with ID {1} for activity.", RewardAmount, user);
                    }
                    Task.Delay(IntervalInMilliseconds).Wait();
                }
            });
        }
    }
}
