using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class UIElement
    {
        public UIElement() { }

        protected Rectangle absoluteBounds; // Calculated screen pixels
        public Rectangle bounds; // Local offsets / size

        public string name = "Unnamed Element";

        public Vector2 anchorMin = Vector2.Zero; // Top-Left by default
        public Vector2 anchorMax = Vector2.Zero;

        public bool scaleWithDPI = false;

        public UIElement parent;

        public Rectangle GetAbsolouteBounds()
        {
            return absoluteBounds;
        }

        public virtual List<UIElement> GetChildren()
        {
            return new List<UIElement>();
        }

        public void BubbleEvent(string eventName, Dictionary<string, object?> data = null)
        {
            if(!OnEventReceived(eventName, data))
            {
                parent?.BubbleEvent(eventName, data);
            }
        }
        public bool IsPressed(Point logicalMousePos)
        {
            return IsHovered(logicalMousePos) &&
                   Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public bool IsHovered(Point logicalMousePos)
        {
            return absoluteBounds.Contains(logicalMousePos);
        }

        protected virtual bool OnEventReceived(string eventName, Dictionary<string, object?> data)
        {
            IsoLog.Log("LogUI", string.Format("EVENT RECEIVED: {0} / Who?: {1}", eventName, this.name));
            return false;
        }

        public Rectangle CalculateBounds(Rectangle parentRect)
        {
            // Horizontal
            int x1 = parentRect.X + (int)(parentRect.Width * anchorMin.X) + bounds.X;
            int x2 = parentRect.X + (int)(parentRect.Width * anchorMax.X) - bounds.Width;

            int finalW;
            if (anchorMin.X == anchorMax.X)
                finalW = bounds.Width; // Use fixed width
            else
                finalW = x2 - x1;      // stretch width

            // Vertical
            int y1 = parentRect.Y + (int)(parentRect.Height * anchorMin.Y) + bounds.Y;
            int y2 = parentRect.Y + (int)(parentRect.Height * anchorMax.Y) - bounds.Height;

            int finalH;
            if (anchorMin.Y == anchorMax.Y)
                finalH = bounds.Height; // use fixed
            else
                finalH = y2 - y1;       // stretch

            return new Rectangle(x1, y1, finalW, finalH);
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle parentRect)
        {
            absoluteBounds = CalculateBounds(parentRect);

            if (scaleWithDPI)
            {
                float dpi = RenderUtil.GetDPIScale();
                absoluteBounds = new Rectangle(
                    (int)(absoluteBounds.X * dpi),
                    (int)(absoluteBounds.Y * dpi),
                    (int)(absoluteBounds.Width * dpi),
                    (int)(absoluteBounds.Height * dpi)
                );
            }

            //DrawOutline(sb, absoluteBounds, Color.Orange, 5);

            UIRender(ref gdm, ref sb, ref renderContext, absoluteBounds);
        }
        protected void DrawOutline(SpriteBatch sb, Rectangle rect, Color color, int thickness = 1)
        {
            if (UIRenderer.onePxWhite == null) return;

            // t
            sb.Draw(UIRenderer.onePxWhite, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            // b
            sb.Draw(UIRenderer.onePxWhite, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            // l
            sb.Draw(UIRenderer.onePxWhite, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            // r
            sb.Draw(UIRenderer.onePxWhite, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }

        protected virtual void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            // subclasses use actualRect to draw
        }
    }
}
