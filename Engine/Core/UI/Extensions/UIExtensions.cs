using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.UI.Elements;
using Microsoft.Xna.Framework;
using Iso.Engine.Core.UI.Elements;
using System.Runtime.CompilerServices;

namespace Iso.Engine.Core.UI.Extensions
{
    public static class UIExtensions
    {
        public static T SetWidth<T>(this T element, int width) where T : UIElement
        {
            var bounds = element.bounds;
            element.bounds = new Rectangle(bounds.X, bounds.Y, width, bounds.Height);
            return element;
        }

        public static T SetHeight<T>(this T element, int height) where T : UIElement
        {
            var bounds = element.bounds;
            element.bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, height);
            return element;
        }

        public static T SetPosition<T>(this T element, IntVector2 position) where T : UIElement
        {
            var bounds = element.bounds;
            element.bounds = new Rectangle(position.x, position.y, bounds.Width, bounds.Height);
            return element;
        }

        public static T Attach<T>(this T parent, UIElement child) where T : Panel
        {
            parent.GetChildren().Add(child);
            child.parent = parent;
            return parent;
        }

        public static T SetAnchor<T>(this T element, Vector2 min, Vector2 max) where T : UIElement
        {
            element.anchorMin = min;
            element.anchorMax = max;
            return element;
        }

        public static T FillParent<T>(this T element, int margin = 0) where T : UIElement
        {
            element.anchorMin = new Vector2(0, 0);
            element.anchorMax = new Vector2(1, 1);
            element.bounds = new Rectangle(margin, margin, margin, margin); // Margins
            return element;
        }

        public static T CenterInParent<T>(this T element, int width, int height) where T : UIElement
        {
            element.anchorMin = new Vector2(0.5f, 0.5f);
            element.anchorMax = new Vector2(0.5f, 0.5f);
            element.bounds = new Rectangle(-width / 2, -height / 2, width, height);
            return element;
        }

        public static T SetSpacing<T>(this T element, int spacing) where T : VStack
        {
            element.spacing = spacing;
            return element;
        }

        public static T FillWidth<T>(this T element, int margin = 0) where T : UIElement
        {
            element.anchorMin.X = 0;
            element.anchorMax.X = 1;

            // In stretch mode: 
            // bounds.X is the Left Margin
            // bounds.Width is the Right Margin
            element.bounds.X = margin;
            element.bounds.Width = margin;

            return element;
        }

        public static T SetChild<T>(this T element, UIElement child) where T : SingleChildElement
        {
            element.child = child;
            return element;
        }

        public static T SetInnerSlotPadding<T>(this T element, IntVector2 innerSlotPadding) where T : SingleChildElement
        {
            element.innerSlotPadding = innerSlotPadding;
            return element;
        }

        public static T SetName<T>(this T element, string name) where T : UIElement
        {
            element.name = name;
            return element;
        }

        public static T SetTint<T>(this T element, Color tint) where T : Image
        {
            element.tint = tint;
            return element;
        }
    }
}
