using Iso.Engine.Core.Logging;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements.Templates
{
    public class WindowTitleBar : Wrapper
    {
        protected override bool OnEventReceived(string eventName, Dictionary<string, object> data)
        {
            IsoLog.Log("LogUI", string.Format("i consumed :3 -> {0}", name));
            return true;
        }
    }
}
