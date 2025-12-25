using Iso.Engine.Core.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Tiles
{
    public class TileChunk
    {
        public const byte CHUNKSIZE = 96;
        public const byte MAX_Z = 4;
        public SquareTileData[,,] tiles;

        public TileChunk()
        {
            tiles = new SquareTileData[CHUNKSIZE, CHUNKSIZE, MAX_Z];
        }

        public SquareTileData? GetTileAt(IntVector3 position)
        {
            if (!IsValidPosition(position))
            {
                return null;
            }

            return tiles[position.x, position.y, position.z];
        }

        public bool IsValidPosition(IntVector3 position)
        {
            return !(tiles == null ||
                position.x >= tiles.GetLength(0) ||
                position.y >= tiles.GetLength(1) ||
                position.z >= tiles.GetLength(2) ||
                position.x < 0 ||
                position.y < 0 ||
                position.z < 0
                );
        }

        public bool SetTileAt(IntVector3 position, SquareTileData newTile)
        {
            if (!IsValidPosition(position))
            {
                return false;
            }

            tiles[position.x, position.y, position.z] = newTile;
            return true;
        }

        public void BasicFillWithTiles()
        {
            for (int x = 0; x < CHUNKSIZE; x++)
            {
                for (int y = 0; y < CHUNKSIZE; y++)
                {
                    List<TileObject> t = new List<TileObject>();

                    t.Add(new TileObject(ETileType.F_DEBUG));

                    if(Random.Shared.Next(100) >= 80)
                    {
                        t.Add(new TileObject(ETileType.W_DEBUG));
                    }

                    SetTileAt(new IntVector3(x, y, 0), new SquareTileData(t));
                }
            }
        }

        public void SingleTileWall()
        {
            List<TileObject> t = new List<TileObject>();

            t.Add(new TileObject(ETileType.F_DEBUG));
            t.Add(new TileObject(ETileType.W_DEBUG));

            SetTileAt(new IntVector3(0, 0, 0), new SquareTileData(t));
        }

        public static IntVector2 WorldToChunkPosition(IntVector3 worldPosition)
        {
            int tileX = worldPosition.x / Tilemap.TILEWIDTH;
            int tileY = worldPosition.y / Tilemap.TILEHEIGHT;

            int chunkX = (int)Math.Floor((double)tileX / CHUNKSIZE);
            int chunkY = (int)Math.Floor((double)tileY / CHUNKSIZE);

            return new IntVector2(chunkX, chunkY);
        }

        public SquareTileData GetLocalTile(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 ||
                x >= CHUNKSIZE || y >= CHUNKSIZE || z >= MAX_Z)
                return null;

            return tiles[x, y, z];
        }
    }
}
