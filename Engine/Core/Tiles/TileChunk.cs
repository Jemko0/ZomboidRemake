using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Iso.Engine.Core.Tiles
{
    public class TileChunk
    {
        public const byte CHUNKSIZE = 96;
        public const byte MAX_Z = 4;
        public SquareTileData[,,] tiles;

        private TileVertex[] cpuVertexBuffer;
        private int vertexCount = 0;

        public VertexBuffer gpuVertexBuffer { get; private set; }
        public bool dirty = true;

        public IntVector2 chunkPosition = new(0, 0);

        public TileChunk(IntVector2 pos)
        {
            tiles = new SquareTileData[CHUNKSIZE, CHUNKSIZE, MAX_Z];
            chunkPosition = pos;

            int maxVertices = CHUNKSIZE * CHUNKSIZE * MAX_Z * 6;
            cpuVertexBuffer = new TileVertex[maxVertices];
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
            MarkDirty();

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

        public void MarkDirty()
        {
            dirty = true;
        }

        public void RebuildVertexBuffer(GraphicsDevice graphicsDevice)
        {
            if (tiles == null) return;

            vertexCount = 0;

            for (int z = 0; z < MAX_Z; z++)
            {
                for (int x = 0; x < CHUNKSIZE; x++)
                {
                    int worldX = chunkPosition.x * CHUNKSIZE + x;

                    for (int y = 0; y < CHUNKSIZE; y++)
                    {
                        SquareTileData tileSquare = tiles[x, y, z];
                        if (tileSquare == null || tileSquare.objects == null) continue;

                        int worldY = chunkPosition.y * CHUNKSIZE + y;
                        int objCount = tileSquare.objects.Count;

                        for (int i = 0; i < objCount; i++)
                        {
                            TileObject o = tileSquare.objects[i];
                            if (o == null) continue;

                            // Pass the CPU array
                            TileRendering.AddTileQuad(cpuVertexBuffer, ref vertexCount, worldX, worldY, Tilemap.TILEWIDTH, Tilemap.TILEHEIGHT, o.type);
                        }
                    }
                }
            }

            //upload data to GPU

            if (vertexCount > 0)
            {
                // Dispose previous buffer to free VRAM
                gpuVertexBuffer?.Dispose();
                gpuVertexBuffer = new VertexBuffer(graphicsDevice, typeof(TileVertex), vertexCount, BufferUsage.WriteOnly);
                gpuVertexBuffer.SetData(cpuVertexBuffer, 0, vertexCount);
            }
            else
            {
                gpuVertexBuffer = null; // chunk is empty
            }

            dirty = false; //mark clean
        }

        public static IntVector2 WorldToChunkPosition(FVector3 cameraScreenPosition)
        {
            float tileWidth = Tilemap.TILEWIDTH;   // 32
            float tileHeight = Tilemap.TILEHEIGHT; // 16

            float tileX = (cameraScreenPosition.x / (tileWidth / 2.0f) + cameraScreenPosition.y / (tileHeight / 2.0f)) / 2.0f;
            float tileY = (cameraScreenPosition.y / (tileHeight / 2.0f) - cameraScreenPosition.x / (tileWidth / 2.0f)) / 2.0f;

            int chunkX = (int)Math.Floor(tileX / CHUNKSIZE);
            int chunkY = (int)Math.Floor(tileY / CHUNKSIZE);

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
