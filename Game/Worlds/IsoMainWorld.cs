using Iso.Engine.Core;
using Iso.Engine.Core.Assets;
using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Iso.Game.Worlds
{
    public class IsoMainWorld : World
    {
        private Effect singleTileShader = null;
        public override void Init()
        {
            base.Init();
            tilemap = new Tilemap();
            tilemap.Init();

            activeCamera = new IsoCamera();

            singleTileShader = AssetHelper.globalContentManager.Load<Effect>("shaders/single_tile_shader");
        }

        double lastDelta = 0;
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            lastDelta = deltaTime;
        }

        public override void SetRenderContext()
        {
            isoRenderContext = new IsoRenderContextBuilder()
                                    .WithCamera(activeCamera)
                                    .WithExtra("debug", true)
                                    .WithExtra("single_tile_shader", singleTileShader)
                                    .Build();
        }

        public override void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            base.Render(ref gdm, ref sb);

            IsoLog.Log("LogDebug", "RENDER EVENT RENDER EVENT");

            IntVector2 currentChunkPos = TileChunk.WorldToChunkPosition(activeCamera.GetPosition().ToIntVector3());

            int fps = (int)Math.Clamp(1.0 / lastDelta, 0.0, double.MaxValue);

            float scale = 1.0f;

            sb.Begin(SpriteSortMode.Immediate);
            sb.DrawString(Fonts.monospace, string.Format("CX: {0} CY: {1}", currentChunkPos.x, currentChunkPos.y), new Vector2(100, 72), Color.Red, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            sb.DrawString(Fonts.monospace, string.Format("X: {0} Y: {1}", activeCamera.GetPosition().x, activeCamera.GetPosition().y), new Vector2(100, 100), Color.Red, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            sb.DrawString(Fonts.monospace, string.Format("D: {0}", lastDelta), new Vector2(100, 132), Color.Red, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            sb.DrawString(Fonts.monospace, string.Format("FPS: {0}", fps), new Vector2(100, 164), Color.Red, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            sb.End();
        }
    }
}
