using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.UI.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class ScrollBox : SingleChildElement
    {
        public ScrollBox()
        {
            clipChildren = true;
        }

        public int scrollbarWidth = 10;
        public Image scrollbarTrackImage = new Image().SetBrush(UIRenderer.onePxWhite).SetTint(Color.Gray).FillParent();
        public Image scrollbarThumbImage = new Image().SetBrush(UIRenderer.onePxWhite).SetTint(Color.White).FillParent();

        public float scrollOffset = 0.0f;
        public float maxScrollOffset = 0.0f;

        private Rectangle thumbRect = Rectangle.Empty;
        private Rectangle trackRect = Rectangle.Empty;
        private bool isDraggingThumb = false;
        private float dragStartMouseY = 0f;
        private float dragStartScrollOffset = 0f;

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            Rectangle contentRect = actualRect;
            contentRect.Width -= scrollbarWidth;

            float viewportHeight = contentRect.Height;
            float contentHeight = child.bounds.Height;

            maxScrollOffset = Math.Max(0, contentHeight - viewportHeight);

            if (scrollOffset < 0) scrollOffset = 0;
            if (scrollOffset > maxScrollOffset) scrollOffset = maxScrollOffset;

            child.bounds.Y = -(int)scrollOffset;

            RenderScrollbar(ref gdm, ref sb, ref renderContext, actualRect, viewportHeight, contentHeight);

            Rectangle paddedContentRect = contentRect;
            paddedContentRect.Inflate(-innerSlotPadding.x, -innerSlotPadding.y);

            child.Render(ref gdm, ref sb, ref renderContext, paddedContentRect);

            /*
            DrawOutline(sb, actualRect, Color.Red, 2);
            DrawOutline(sb, paddedContentRect, Color.Orange, 2);

            sb.DrawString(Fonts.arial, string.Format("SO: {0} / MS: {1}", scrollOffset, maxScrollOffset), actualRect.Location.ToVector2(), Color.Red);
            */
        }

        public override void OnDestroy()
        {
            scrollbarThumbImage.brush = null;
            scrollbarTrackImage.brush = null;
        }

        private void RenderScrollbar(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle containerRect, float viewH, float contentH)
        {
            trackRect = new Rectangle(
                containerRect.X + containerRect.Width - scrollbarWidth,
                containerRect.Y,
                scrollbarWidth,
                containerRect.Height
            );

            scrollbarTrackImage.Render(ref gdm, ref sb, ref renderContext, trackRect);

            if (contentH <= viewH) return;

            float viewRatio = viewH / contentH;
            float thumbHeight = Math.Max(20, viewH * viewRatio);

            float availableTrackHeight = viewH - thumbHeight;
            float scrollRatio = scrollOffset / maxScrollOffset;

            int thumbY = trackRect.Y + (int)(availableTrackHeight * scrollRatio);

            thumbRect = new Rectangle(
                trackRect.X,
                thumbY,
                scrollbarWidth,
                (int)thumbHeight
            );

            scrollbarThumbImage.Render(ref gdm, ref sb, ref renderContext, thumbRect);
        }

        public void Scroll(float amount)
        {
            scrollOffset += amount;
        }

        protected override bool OnEventReceived(UIEvent eventName, Dictionary<string, object> data)
        {
            switch (eventName)
            {
                case UIEvent.MOUSE_WHEEL:
                    float wd = GetData<MouseEventArgs>("mouseArguments", data).wheelDelta / 40.0f;
                    Scroll(wd);
                    return true;

                case UIEvent.MOUSE_LMBCLICK:
                    MouseEventArgs clickArgs = GetData<MouseEventArgs>("mouseArguments", data);
                    Vector2 clickPos = UIRenderer.GetLogicalMouse(clickArgs.position.ToPoint()).ToVector2();

                    if (thumbRect.Contains(clickPos))
                    {
                        isDraggingThumb = true;
                        dragStartMouseY = clickPos.Y;
                        dragStartScrollOffset = scrollOffset;
                        return true;
                    }

                    if (trackRect.Contains(clickPos))
                    {
                        // calculate where in the track we clicked (0.0 to 1.0)
                        float clickRatio = (clickPos.Y - trackRect.Y) / trackRect.Height;
                        scrollOffset = clickRatio * maxScrollOffset;
                        return true;
                    }
                    return false;

                case UIEvent.MOUSE_LMBRELEASE:
                    if (isDraggingThumb)
                    {
                        isDraggingThumb = false;
                        return true;
                    }
                    return false;

                case UIEvent.MOUSE_MOVE:
                    if (isDraggingThumb)
                    {
                        MouseEventArgs moveArgs = GetData<MouseEventArgs>("mouseArguments", data);
                        Vector2 currentMousePos = UIRenderer.GetLogicalMouse(moveArgs.position.ToPoint()).ToVector2();

                        float mouseDelta = currentMousePos.Y - dragStartMouseY;

                        // Convert mouse movement to scroll movement
                        float viewportHeight = trackRect.Height;
                        float contentHeight = child.bounds.Height;
                        float thumbHeight = thumbRect.Height;
                        float availableTrackHeight = viewportHeight - thumbHeight;

                        if (availableTrackHeight > 0)
                        {
                            float scrollDelta = (mouseDelta / availableTrackHeight) * maxScrollOffset;
                            scrollOffset = dragStartScrollOffset + scrollDelta;
                        }

                        return true;
                    }
                    return false;
            }
            return false;
        }
    }
}
