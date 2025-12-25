using Iso.Engine.Core;
using Iso.Engine.Core.Assets;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iso.Game.Worlds
{
    public class GameInitWorld : World
    {
        public GameInitWorld() { }

        public enum InitializationStage : byte
        {
            NOT_STARTED,
            TILE_DEF_LOADING,
            TEX_ATLAS,
            FINISHED
        }

        public InitializationStage currentInitStage = InitializationStage.NOT_STARTED;

        public override async void Init()
        {
            TileDefLoadResult result = new TileDefLoadResult(false, "NO MESSAGE");

            currentInitStage = InitializationStage.TILE_DEF_LOADING;

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

            currentInitStage = InitializationStage.TEX_ATLAS;

            RenderUtil.tileAtlas = AssetHelper.GenerateTileTextureAtlas(IsoGame.graphics.GraphicsDevice);

            currentInitStage = InitializationStage.FINISHED;

            SceneManager.LoadWorldDeferred<IsoMainWorld>();
        }

        public override void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            base.Render(ref gdm, ref sb);

            Vector2 textpos = RenderUtil.window.ClientBounds.Center.ToVector2();
            sb.DrawString(Fonts.monospace, currentInitStage.ToString(), textpos, Color.White);
        }
    }
}
