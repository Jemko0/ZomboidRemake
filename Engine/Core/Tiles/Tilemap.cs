using System;
using System.Collections.Generic;
using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.Rendering.Interfaces;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering;

namespace Iso.Engine.Core.Tiles
{
    internal class Tilemap : IRenderable, IDisposable
    {
        public Tilemap() { }

        public Dictionary<IntVector3, FTileData> tiles;
        private static int viewRange = 300;

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
        }

        public void Dispose()
        {
            tiles = null;
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            IsoCamera camera = renderContext.camera;

            if(camera == null)
            {
                return;
            }

            FVector3 camPosition = camera.GetPosition();
        }
    }
}
