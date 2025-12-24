using System;
using System.Collections.Generic;
using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.Rendering.Interfaces;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;

namespace Iso.Engine.Core.Tiles
{
    public class Tilemap : IRenderable, IDisposable
    {
        public Tilemap() { }

        public Dictionary<IntVector3, FTileData> tiles;
        private static int viewRange = 300;

        public const int TILEWIDTH = 32;
        public const int TILEHEIGHT = TILEWIDTH / 2;

        public FTileData? GetTileAt(IntVector3 position)
        {
            if (tiles != null && tiles.TryGetValue(position, out var tile))
            {
                return tile;
            }

            return null;
        }

        public bool SetTileAt(IntVector3 position, FTileData newTile)
        {
            if(GetTileAt(position) == null)
            {
                return false;
            }

            tiles[position] = newTile;
            return true;
        }

        public void SetViewRange(int newViewRange)
        {
            viewRange = newViewRange;
        }

        //Behaviours
        public void Init()
        {
            tiles = new Dictionary<IntVector3, FTileData>();

            for (int x = 0; x < 50; x++)
            {
                for(int y = 0; y < 50; y++)
                {
                    tiles.Add(new IntVector3(x, y, (y == 1? 1 : 0)), new FTileData(ETileType.GRASS_01));
                }
            }
        }

        public void Dispose()
        {
            tiles = null;
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            IsoCamera camera = renderContext.camera;
            if (camera == null || tiles == null || tiles.Count == 0)
            {
                return;
            }

            float maxDepth = 50 + 50 + 10; // adjust based on map size and max height

            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap);

            foreach (var kvp in tiles)
            {
                IntVector3 pos = kvp.Key;
                FTileData tile = kvp.Value;
                TileDefinitionData tileData = TileDefinitions.definitions[tile.type];

                // depth
                float layerDepth = (pos.x + pos.y + pos.z) / maxDepth;

                Vector2 screenPos = RenderUtil.WorldToScreen(new FVector3(pos.x, pos.y, pos.z), TILEWIDTH, TILEHEIGHT, camera);
                float scale = RenderUtil.GetOnScreenSize(camera.GetZoom());

                sb.Draw(
                    tileData.tileTexture,
                    screenPos,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    layerDepth
                );

                sb.DrawString(Fonts.arial, "Z" + pos.z.ToString(), screenPos, Color.Red, 0.0f, Vector2.Zero, scale / 2.0f, SpriteEffects.None, layerDepth + 0.01f);
            }

            sb.End();
        }
    }
}
