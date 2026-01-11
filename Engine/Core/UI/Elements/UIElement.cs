using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public abstract class UIElement : IIsoUpdateable
    {
        public UIElement() { }

        protected Rectangle absoluteBounds; // calculated screen pixels

        public Rectangle bounds;
        public string name = "Unnamed Element";
        public Vector2 anchorMin = Vector2.Zero;
        public Vector2 anchorMax = Vector2.Zero;
        public Vector2 pivot = Vector2.Zero;
        public UIElement parent = null;
        public UIVisibilityMode visibility = UIVisibilityMode.VISIBLE;
        public bool clipChildren = false;

        public MouseCursor cursor = MouseCursor.Arrow;

        /// <summary>
        /// Use to free up any resources before this Element is removed from the Hierarchy
        /// </summary>
        public virtual void OnDestroy()
        {

        }

        public void Destroy(bool mark = true)
        {
            if(mark)
            {
                UIRenderer.MarkForDeletion(this);
            }

            List<UIElement> children = GetChildren();
            int count = children.Count;

            for (int i = 0; i < count; i++)
            {
                children[i].Destroy(false);
            }
        }

        public Rectangle GetAbsolouteBounds()
        {
            return absoluteBounds;
        }

        /// <summary>
        /// Returns the children of this element, on classes derived from <see cref="Panel"/> will return
        /// mutable List directly. On classes derived from <see cref="SingleChildElement"/> will return a NEW
        /// list with the singular child. If you want to modify the single child, use <see cref="SingleChildElement.child"/> or <see cref="Extensions.UIExtensions.SetChild{T}(T, UIElement)"/>
        /// </summary>
        /// <returns></returns>
        public virtual List<UIElement> GetChildren()
        {
            return new List<UIElement>();
        }

        public T GetData<T>(string name, Dictionary<string, object?> data)
        {
            if(data == null)
            {
                return default;
            }

            if(data[name] == null)
            {
                return default;
            }

            return (T)data[name];
        }

        public void BubbleEvent(UIEvent eventName, Dictionary<string, object?> data = null)
        {
            if(!OnEventReceived(eventName, data))
            {
                parent?.BubbleEvent(eventName, data);
            }
        }

        public bool IsPressed()
        {
            return IsHovered() && Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public bool IsHovered()
        {
            Point logicalMousePos = Input.GetLogicalMousePosition();

            if (!absoluteBounds.Contains(logicalMousePos)) return false;

            UIElement current = parent;

            while (current != null)
            {
                if (current.clipChildren && !current.GetAbsolouteBounds().Contains(logicalMousePos))
                {
                    return false;
                }
                current = current.parent;
            }

            return true;
        }

        protected virtual bool OnEventReceived(UIEvent eventName, Dictionary<string, object?> data)
        {
            //IsoLog.Log("LogUI", string.Format("EVENT RECEIVED: {0} / Who?: {1}", eventName.ToString(), this.name));
            return false;
        }

        public Rectangle CalculateBounds(Rectangle parentRect)
        {
            int anchorX = parentRect.X + (int)(parentRect.Width * anchorMin.X);
            int anchorY = parentRect.Y + (int)(parentRect.Height * anchorMin.Y);

            int finalX, finalY, finalW, finalH;

            if (anchorMin.X == anchorMax.X)
            {
                finalW = bounds.Width;
                // Position = Anchor + Offset - (Pivot * Size)
                finalX = anchorX + bounds.X - (int)(finalW * pivot.X);
            }
            else
            {
                int x1 = anchorX + bounds.X;
                int x2 = parentRect.X + (int)(parentRect.Width * anchorMax.X) - bounds.Width;
                finalX = x1;
                finalW = x2 - x1;
            }

            if (anchorMin.Y == anchorMax.Y)
            {
                finalH = bounds.Height;
                finalY = anchorY + bounds.Y - (int)(finalH * pivot.Y);
            }
            else
            {
                int y1 = anchorY + bounds.Y;
                int y2 = parentRect.Y + (int)(parentRect.Height * anchorMax.Y) - bounds.Height;
                finalY = y1;
                finalH = y2 - y1;
            }

            return new Rectangle(finalX, finalY, finalW, finalH);
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle parentRect)
        {
            if (visibility == UIVisibilityMode.COLLAPSED) return;

            absoluteBounds = CalculateBounds(parentRect);

            if (visibility == UIVisibilityMode.HIDDEN || visibility == UIVisibilityMode.HIDDEN_NO_HIT_TEST) return;

            if(clipChildren)
            {
                Rectangle oldScissor = gdm.GraphicsDevice.ScissorRectangle;

                sb.End();
                Clip(gdm);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, UIRenderer.rasterizerState);

                UIRender(ref gdm, ref sb, ref renderContext, absoluteBounds);

                sb.End();

                gdm.GraphicsDevice.ScissorRectangle = oldScissor;
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, UIRenderer.rasterizerState);
            }
            else
            {
                UIRender(ref gdm, ref sb, ref renderContext, absoluteBounds);
            }
        }

        public void Clip(GraphicsDeviceManager gdm)
        {
            Rectangle parentScissor = gdm.GraphicsDevice.ScissorRectangle;
            Rectangle myRect = absoluteBounds;

            // Intersection: This ensures the child can't draw outside the parent
            int x = Math.Max(parentScissor.X, myRect.X);
            int y = Math.Max(parentScissor.Y, myRect.Y);
            int width = Math.Min(parentScissor.Right, myRect.Right) - x;
            int height = Math.Min(parentScissor.Bottom, myRect.Bottom) - y;

            gdm.GraphicsDevice.ScissorRectangle = new Rectangle(x, y, Math.Max(0, width), Math.Max(0, height));
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

        public void Update(double deltaTime)
        {
            if (visibility == UIVisibilityMode.COLLAPSED) return;

            //IsoLog.Log("UI", string.Format("UPDATED : {0}", name));
            UIUpdate(deltaTime);

            if (GetChildren().Count < 1) return;

            List<UIElement> children = GetChildren();

            for (int i = 0; i < children.Count; i++)
            {
                children[i]?.Update(deltaTime);
            }
        }

        public virtual void UIUpdate(double deltaTime)
        {
            //Subclasses override for custom behaviour
        }
    }
}
