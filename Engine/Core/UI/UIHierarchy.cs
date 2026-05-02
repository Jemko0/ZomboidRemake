using System;
using Iso.Engine.Core.UI.Elements;

namespace Iso.Engine.Core.UI
{
    public static class UIHierarchy
    {
        public static void Attach(UIElement parent, UIElement child)
        {
            switch (parent)
            {
                case Panel panel:
                    panel.GetChildren().Add(child);
                    break;

                case SingleChildElement single:
                    if (single.child != null)
                        throw new Exception($"{parent.GetType().Name} can only have one child");

                    single.child = child;
                    break;

                default:
                    throw new Exception($"Cannot attach child to {parent.GetType().Name}");
            }

            child.parent = parent;
        }
    }
}