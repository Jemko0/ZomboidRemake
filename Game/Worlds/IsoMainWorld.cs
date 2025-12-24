using Iso.Engine.Core;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Tiles;
using Iso.Engine.Core.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Game.Worlds
{
    public class IsoMainWorld : World
    {
        public override void Init()
        {
            base.Init();
            tilemap = new Tilemap();
            tilemap.Init();

            activeCamera = new IsoCamera();
        }

        public override void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            base.Render(ref gdm, ref sb);

            IsoLog.Log("LogDebug", "RENDER EVENT RENDER EVENT");
        }
    }
}
