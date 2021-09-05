using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class HelpAttribute : Attribute
    {
        public string Example;
        public string OneLineDesc;
        public string LongDescription;

        public HelpAttribute(string example) => Example = example;
    }
}
