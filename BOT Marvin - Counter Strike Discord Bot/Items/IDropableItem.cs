using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Items
{
    /// <summary>
    /// An interface for all <see cref="Item"/>s that can be dropped to users active on discord.
    /// </summary>
    interface IDropableItem 
    {
        /// <summary>
        /// Whether or not the Item can be dropped.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsActivityDropping();


        /// <summary>
        /// Determines how the price of the item relates to the drop propability.
        /// </summary>
        /// <returns></returns>
        public abstract double GetDropValueMultiplier(); 
    }
}
