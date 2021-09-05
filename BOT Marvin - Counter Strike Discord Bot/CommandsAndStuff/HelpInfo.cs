using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Discord.Commands;
using LightBlueFox.Util.Types.Exceptions;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.CommandsAndStuff
{
    public class HelpInfo
    {
        public readonly HelpAttribute Attribute;
        public readonly string Name;
        public readonly CommandInfo Command;
        public readonly List<ParameterHelpInfo> Parameters;

        public HelpInfo(HelpAttribute atr, string name, CommandInfo command, List<ParameterHelpInfo> parameters)
        {
            Name = name; Attribute = atr; Command = command; Parameters = parameters;
        }
    }
}
