using System;
using System.Collections.Generic;
using System.Text;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{
    public abstract class ViewerModifier
    {
        public static List<ViewerModifier> KnownModifiers { get; private set; }

        public static void InitViewerModifiers()
        {
            KnownModifiers = ViewerModifierAttribute.GetAllModifiers();
        }

        public abstract bool isApplicable(ViewerPage page, ViewerDisplayArgs args);
        public abstract ViewerPage Modify(ViewerPage page, ViewerDisplayArgs args);

    }
}
