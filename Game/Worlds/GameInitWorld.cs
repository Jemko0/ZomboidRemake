using Iso.Engine.Core;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Game.Worlds
{
    public class GameInitWorld : World
    {
        public GameInitWorld() { }

        public override async void Init()
        {
            TileDefLoadResult result = new TileDefLoadResult(false, "NO MESSAGE");

            try
            {
                IsoLog.Log("LogTileDefinitions", "Loading Tile Definitions...", ELogVerbosity.Log);
                result = await TileDefinitionLoader.LoadTileDefinitions();
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }

            if (!result.success)
            {
                IsoLog.Log("LogTileDefinitions", result.message, ELogVerbosity.Fatal);
            }

            SceneManager.LoadWorldDeferred<IsoMainWorld>();
        }

        public override void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            base.Render(ref gdm, ref sb);
        }
    }
}
