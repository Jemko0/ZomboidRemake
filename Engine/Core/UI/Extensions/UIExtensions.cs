using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.UI.Elements;
using Microsoft.Xna.Framework;
using SharpDX.DXGI;
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

        /// <summary>
        /// Sets this element´s relative position to its parent. Depending on parent layout,
        /// this may not do anything
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="position">New Position</param>
        /// <returns></returns>
        public static T SetPosition<T>(this T element, IntVector2 position) where T : UIElement
        {
            var bounds = element.bounds;
            element.bounds = new Rectangle(position.x, position.y, bounds.Width, bounds.Height);
            return element;
        }

        /// <summary>
        /// Adds a new child to this element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child">New Child</param>
        /// <returns></returns>
        public static T Attach<T>(this T parent, UIElement child) where T : Panel
        {
            parent.GetChildren().Add(child);
            child.parent = parent;
            return parent;
        }

        /// <summary>
        /// Sets this elements Anchor in normalized coordinates (0, 0) = top left, (1, 1) = bottom right
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="min">Minimum Anchor Point (0, 0) = top left, (1, 1) = bottom right</param>
        /// <param name="max">Maximum Anchor Point (0, 0) = top left, (1, 1) = bottom right</param>
        /// <returns></returns>
        public static T SetAnchor<T>(this T element, Vector2 min, Vector2 max) where T : UIElement
        {
            element.anchorMin = min;
            element.anchorMax = max;
            return element;
        }

        public static T SetSize<T>(this T element, int width, int height) where T : UIElement
        {
            var bounds = element.bounds;
            bounds.Width = width;
            bounds.Height = height;
            element.bounds = bounds;

            return element;
        }

        /// <summary>
        /// Clips children of the current element, if enabled, will cause
        /// the renderer to dispatch 2 draw calls for this element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="newClip">New Clipping State</param>
        /// <returns></returns>
        public static T SetClipChildren<T>(this T element, bool newClip) where T : UIElement
        {
            element.clipChildren = newClip;
            return element;
        }

        /// <summary>
        /// Sets this elements Anchor to (0, 0, 1, 1) effectively
        /// filling the entire parent element, will not always fill entire
        /// element space since that depends on the parent layout. (<see cref="HStack"/> handles filling differently than <see cref="Wrapper"/> for example.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static T FillParent<T>(this T element, int margin = 0) where T : UIElement
        {
            element.anchorMin = new Vector2(0, 0);
            element.anchorMax = new Vector2(1, 1);
            element.bounds = new Rectangle(margin, margin, margin, margin); // Margins
            return element;
        }

        /// <summary>
        /// Sets this elements Anchor to (0.5, 0.5, 0.5, 0.5) effectively
        /// centering itself inside the parent, will not always center in parent
        /// since that depends on the parent layout.
        /// 
        /// if no width/height given, will use parent size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
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

            // In stretch mode
            // bounds.X is the Left Margin
            // bounds.Width is the Right Margin
            element.bounds.X = margin;
            element.bounds.Width = margin;

            return element;
        }

        public static T FillHeight<T>(this T element, int margin = 0) where T : UIElement
        {
            element.anchorMin.Y = 0;
            element.anchorMax.Y = 1;

            element.bounds.Y = margin;
            element.bounds.Height = margin;

            return element;
        }

        /// <summary>
        /// Sets the child for a SingleChildElement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T SetChild<T>(this T element, UIElement child) where T : SingleChildElement
        {
            element.child = child;
            child.parent = element;
            return element;
        }

        public static T SetInnerSlotPadding<T>(this T element, IntVector2 innerSlotPadding) where T : SingleChildElement
        {
            element.innerSlotPadding = innerSlotPadding;
            return element;
        }

        /// <summary>
        /// Sets the name of a <see cref="UIElement"/>. Usually decorative only and for organization/visualization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T SetName<T>(this T element, string name) where T : UIElement
        {
            element.name = name;
            return element;
        }

        /// <summary>
        /// Sets this image´s tint when rendering.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="tint"></param>
        /// <returns></returns>
        public static T SetTint<T>(this T element, Color tint) where T : Image
        {
            element.tint = tint;
            return element;
        }
    }
}
