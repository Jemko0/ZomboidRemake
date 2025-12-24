using Iso.Engine.Core;
using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Tiles;
using Iso.Game.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Iso
{
    public class IsoGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public SceneManager mainSceneManager;

        public IsoGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Fonts.SetupFonts(Content);
            RenderUtil.window = Window;
            TileDefinitionLoader.contentManager = Content;

            mainSceneManager = new SceneManager();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SceneManager.LoadWorldDeferred<GameInitWorld>();
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalMilliseconds;

            mainSceneManager.Update(delta);

            if(SceneManager.loadedWorld.activeCamera == null)
            {
                return;
            }

            FVector3 camDelta = new FVector3();

            camDelta.x += Keyboard.GetState().IsKeyDown(Keys.D) ? 5 : 0;
            camDelta.x += Keyboard.GetState().IsKeyDown(Keys.A) ? -5 : 0;
            camDelta.y += Keyboard.GetState().IsKeyDown(Keys.W) ? -2.5f : 0;
            camDelta.y += Keyboard.GetState().IsKeyDown(Keys.S) ? 2.5f : 0;

            float zoom = SceneManager.loadedWorld.activeCamera.GetZoom();

            zoom += Keyboard.GetState().IsKeyDown(Keys.OemPlus) ? 0.02f * zoom : 0.0f;
            zoom += Keyboard.GetState().IsKeyDown(Keys.OemMinus) ? -0.02f * zoom : 0.0f;

            SceneManager.loadedWorld.activeCamera.Translate(camDelta);
            SceneManager.loadedWorld.activeCamera.SetZoom(zoom);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mainSceneManager.SendRenderEvent(ref graphics, ref spriteBatch);

            base.Draw(gameTime);
        }
    }
}
