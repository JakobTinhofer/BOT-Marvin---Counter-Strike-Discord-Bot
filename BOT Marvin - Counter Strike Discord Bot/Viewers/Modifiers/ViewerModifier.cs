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
        public bool isEnabled = true;

        public bool IsApplicable(ViewerPage page, ViewerDisplayArgs args)
        {
            if (!isEnabled)
                return false;
            return _isApplicable(page, args);
        }
        protected abstract bool _isApplicable(ViewerPage page, ViewerDisplayArgs args);
        public ViewerPage Modify(ViewerPage page, ViewerDisplayArgs args)
        {
            if (!IsApplicable(page, args))
                return page;
            return _modify(page, args);
        }

        protected abstract ViewerPage _modify(ViewerPage page, ViewerDisplayArgs args);

    }
}
