using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
{
    public class ParameterHelpInfo
    {
        public readonly HelpAttribute Attribute;
        public readonly ParameterInfo Parameter;
        public readonly string Name;

        public ParameterHelpInfo(string name, HelpAttribute attr, ParameterInfo param)
        {
            Name = name; Attribute = attr; Parameter = param;
        }

    }
}
