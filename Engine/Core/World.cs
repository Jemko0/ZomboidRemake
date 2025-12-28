using Iso.Engine.Core.Tiles;
using Iso.Engine.Core.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.UI;
using Microsoft.Xna.Framework;

namespace Iso.Engine.Core
{
    public class World : IIsoUpdateable
    {
        public Tilemap tilemap;
        protected string mapFilePath = "";

        public IsoCamera activeCamera;
        public IsoRenderContext isoRenderContext = null;

        public UIRenderer uiRenderer = null!;

        public World()
        {
            SetupUIViewport();
        }

        public void SetupUIViewport()
        {
            if (uiRenderer != null) return;

            UIRenderer.onePxWhite = new Texture2D(IsoGame.graphics.GraphicsDevice, 1, 1);
            UIRenderer.onePxWhite.SetData(new Color[] { Color.White });

            uiRenderer = new UIRenderer();
            uiRenderer.uiRenderTarget = new RenderTarget2D(IsoGame.graphics.GraphicsDevice, RenderUtil.window.ClientBounds.Width, RenderUtil.window.ClientBounds.Height);
        }

        public virtual void Init()
        {
            
        }

        public virtual async void InitAsync()
        {
        }

        public virtual void SetRenderContext()
        {
            isoRenderContext = new IsoRenderContextBuilder()
                                                        .WithCamera(activeCamera)
                                                        .WithExtra("debug", true)
                                                        .Build();
        }

        public virtual void Render(ref Microsoft.Xna.Framework.GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            SetRenderContext();

            if(tilemap == null) return;

            uiRenderer.Render(ref gdm, ref sb, ref isoRenderContext);

            tilemap.Render(ref gdm, ref sb, ref isoRenderContext);

            uiRenderer.DrawToScreen(sb, gdm.GraphicsDevice);
        }

        public virtual void Unload()
        {
            tilemap = null;
        }

        public virtual void Update(double deltaTime)
        {
            uiRenderer.Update(deltaTime);
        }
    }
}
