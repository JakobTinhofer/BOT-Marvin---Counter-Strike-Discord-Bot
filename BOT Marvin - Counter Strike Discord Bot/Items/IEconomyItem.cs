using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Items
{
    /// <summary>
    /// For all items that have value, can be bought/sold
    /// </summary>
    interface IEconomyItem
    {
        /// <summary>
        /// How much this item costs in coins. 100 Coins = 1 Euro
        /// </summary>
        /// <returns></returns>
        public abstract int GetPrice();
    }
}
