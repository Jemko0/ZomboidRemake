using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.UI.Elements;
using Iso.Engine.Core.UI.Extensions;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Interfaces;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI
{
    public class UIRenderer : IIsoRenderable
    {
        public RenderTarget2D uiRenderTarget = null;
        public static Texture2D onePxWhite = null;
        public static Panel root = null!;
        public UIRenderer()
        {
            root = new Panel()
                    .SetName("root")
                    .SetWidth(1920)
                    .SetHeight(1080)
                    .FillParent();

            RenderUtil.window.ClientSizeChanged += OnWindowResized;
            BindEvents();
        }

        private void BindEvents()
        {
            SceneManager.GetInputManager().onMouseLeftClick += UIRenderer_onMouseLeftClick;
            SceneManager.GetInputManager().onMouseRightClick += UIRenderer_onMouseRightClick; ;
        }

        private void UIRenderer_onMouseRightClick(MouseEventArgs e)
        {
            Point logicalMouse = GetLogicalMouse(e.position.ToPoint());

            UIElement hit = GetElementAt(root, logicalMouse);
            if (hit == null) return;

            Dictionary<string, object?> data = new Dictionary<string, object?>();
            data.Add("args", e);

            hit.BubbleEvent("LeftRight", data);
        }

        private void UIRenderer_onMouseLeftClick(MouseEventArgs e)
        {
            Point logicalMouse = GetLogicalMouse(e.position.ToPoint());

            UIElement hit = GetElementAt(root, logicalMouse);
            if (hit == null) return;

            Dictionary<string, object?> data = new Dictionary<string, object?>();
            data.Add("args", e);
            
            hit.BubbleEvent("LeftClick", data);
        }

        private Point GetLogicalMouse(Point physicalPos)
        {
            float dpi = RenderUtil.GetDPIScale();
            return new Point((int)(physicalPos.X / dpi), (int)(physicalPos.Y / dpi));
        }

        private UIElement GetElementAt(UIElement parent, Point mousePos)
        {
            if (parent == null) return null;

            var children = parent.GetChildren();
            if (children != null)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    var hit = GetElementAt(children[i], mousePos);
                    if (hit != null) return hit;
                }
            }

            // if no child was hit check this element
            if (parent.GetAbsolouteBounds().Contains(mousePos))
            {
                return parent;
            }

            return null;
        }

        private void OnWindowResized(object sender, System.EventArgs e)
        {
            int newWidth = RenderUtil.window.ClientBounds.Width;
            int newHeight = RenderUtil.window.ClientBounds.Height;

            newWidth = (int)(newWidth / RenderUtil.GetDPIScale());
            newHeight = (int)(newHeight / RenderUtil.GetDPIScale());

            if (uiRenderTarget.Width != newWidth || uiRenderTarget.Height != newHeight)
            {
                uiRenderTarget.Dispose();
                uiRenderTarget = new RenderTarget2D(IsoGame.graphics.GraphicsDevice, newWidth, newHeight);
            }
        }

        public void Add(params UIElement[] newElements)
        {
            if (newElements == null) return;
            if (newElements.Length == 0) return;

            for(int i  = 0; i < newElements.Length; i++)
            {
                root.Attach(newElements[i]);
            }
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            var device = gdm.GraphicsDevice;

            device.SetRenderTarget(uiRenderTarget);
            device.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            Rectangle screenSpace = new Rectangle(0, 0, uiRenderTarget.Width, uiRenderTarget.Height);
            root.Render(ref gdm, ref sb, ref renderContext, screenSpace);

            sb.End();

            device.SetRenderTarget(null);
        }

        public void DrawToScreen(SpriteBatch sb, GraphicsDevice device)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(uiRenderTarget, device.Viewport.Bounds, Color.White);
            sb.End();
        }
    }
}
