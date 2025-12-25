using Iso.Engine.Core;
using Iso.Engine.Core.Assets;
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
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Fonts.SetupFonts(Content);
            RenderUtil.window = Window;
            AssetHelper.globalContentManager = Content;

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
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            mainSceneManager.Update(delta);

            if(SceneManager.loadedWorld.activeCamera == null)
            {
                return;
            }

            FVector3 camDelta = new FVector3();

            float camSpeed = Tilemap.TILEWIDTH * 5;

            camDelta.x += Keyboard.GetState().IsKeyDown(Keys.D) ? camSpeed : 0;
            camDelta.x += Keyboard.GetState().IsKeyDown(Keys.A) ? -camSpeed : 0;
            camDelta.y += Keyboard.GetState().IsKeyDown(Keys.W) ? -camSpeed/2.0f : 0;
            camDelta.y += Keyboard.GetState().IsKeyDown(Keys.S) ? camSpeed/2.0f : 0;

            camDelta *= delta;

            float zoom = SceneManager.loadedWorld.activeCamera.GetZoom();

            zoom += Keyboard.GetState().IsKeyDown(Keys.OemPlus) ? (float)delta * zoom : 0.0f;
            zoom += Keyboard.GetState().IsKeyDown(Keys.OemMinus) ? (float)-delta * zoom : 0.0f;

            SceneManager.loadedWorld.activeCamera.Translate(camDelta);
            SceneManager.loadedWorld.activeCamera.SetZoom(zoom);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            mainSceneManager.SendRenderEvent(ref graphics, ref spriteBatch);

            base.Draw(gameTime);
        }
    }
}
