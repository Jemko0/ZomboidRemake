using Iso.Engine.Core.Logging;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements.Templates
{
    public class WindowTitleBar : Wrapper
    {
        protected override bool OnEventReceived(UIEvent eventName, Dictionary<string, object> data)
        {
            if(eventName == UIEvent.MOUSE_LMBCLICK)
            {
                StartDrag(data);
                IsoLog.Log("LogUI", string.Format("i consumed :3 -> {0}", name));
                return true;
            }

            if(eventName == UIEvent.MOUSE_LMBRELEASE)
            {
                EndDrag(data);
                IsoLog.Log("LogUI", string.Format("i consumed :3 -> {0}", name));
                return true;
            }

            return false;
        }

        public void StartDrag(Dictionary<string, object> data)
        {
            parent.parent.BubbleEvent(UIEvent.DRAG_START, data);
        }

        public void EndDrag(Dictionary<string, object> data)
        {
            parent.parent.BubbleEvent(UIEvent.DRAG_STOP, data);
        }
    }
}
