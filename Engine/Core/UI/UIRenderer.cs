using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.UI.Elements;
using Iso.Engine.Core.UI.Extensions;
using Iso.Engine.Core.Rendering;

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
                    .SetWidth(1920)
                    .SetHeight(1080)
                    .FillParent();

            RenderUtil.window.ClientSizeChanged += OnWindowResized;
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
