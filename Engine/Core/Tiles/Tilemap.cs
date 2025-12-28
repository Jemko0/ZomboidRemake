using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;

namespace Iso.Engine.Core.Tiles
{
    public class Tilemap : IIsoRenderable, IIsoUpdateable, IDisposable
    {
        public Dictionary<IntVector2, TileChunk> chunks;
        private static int viewRange = 300;

        public const int TILEWIDTH = 32;
        public const int TILEHEIGHT = TILEWIDTH / 2;
        public const int VERTICES_PER_TILE = 6;

        public const int MAX_TILES = (TileChunk.CHUNKSIZE * TileChunk.CHUNKSIZE * TileChunk.MAX_Z) * VERTICES_PER_TILE * 4;

        public TileVertex[] vertexBuffer = null!;
        public int actualVertexCount = 0;

        public Tilemap()
        {
            vertexBuffer = new TileVertex[MAX_TILES];
        }

        public void SetViewRange(int newViewRange)
        {
            viewRange = newViewRange;
        }

        public TileChunk GetChunkAt(IntVector2 chunkPosition)
        {
            chunks.TryGetValue(chunkPosition, out var chunk);

            if(chunk == null)
            {
                return null;
            }

            return chunk;
        }

        private bool TryGetSquare(int worldX, int worldY, int z, out SquareTileData square)
        {
            square = null;

            int chunkX = Math.DivRem(worldX, TileChunk.CHUNKSIZE, out int localX);
            int chunkY = Math.DivRem(worldY, TileChunk.CHUNKSIZE, out int localY);

            // Fix negative modulo
            if (localX < 0) { chunkX--; localX += TileChunk.CHUNKSIZE; }
            if (localY < 0) { chunkY--; localY += TileChunk.CHUNKSIZE; }

            if (!chunks.TryGetValue(new IntVector2(chunkX, chunkY), out var chunk))
                return false;

            square = chunk.GetLocalTile(localX, localY, z);
            return square != null;
        }

        //Behaviours
        public void Init()
        {
            chunks = new Dictionary<IntVector2, TileChunk>();

            for (int x = -2; x < 2; x++)
            {
                for(int y = -2; y < 2; y++)
                {
                    TileChunk c = new TileChunk(new(x, y));
                    c.BasicFillWithTiles();

                    chunks.Add(new IntVector2(x, y), c);
                }
            }
        }

        public void Dispose()
        {
            chunks.Clear();
            chunks = null;
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            GPURender(ref gdm, ref sb, ref renderContext);
        }

        public void GPURender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            IsoCamera camera = renderContext.camera;
            Effect singleTileShader = renderContext.GetExtra<Effect>("single_tile_shader");
            GraphicsDevice device = gdm.GraphicsDevice;

            device.SetRenderTarget(null); //ensure we are targeting the backbuffer

            Rectangle viewport = device.Viewport.Bounds;

            float zoom = RenderUtil.GetOnScreenSize(camera.GetZoom());

            float halfWidth = viewport.Width / (2f * zoom);
            float halfHeight = viewport.Height / (2f * zoom);

            Matrix projection = Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, halfHeight, -halfHeight, 0.0f, 1.0f);

            Matrix view = camera.viewMatrix;

            Matrix wvp = view * projection;

            singleTileShader.Parameters["WorldViewProjection"].SetValue(wvp);
            singleTileShader.Parameters["TileTexture"]?.SetValue(RenderUtil.tileAtlas.Texture);

            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.None;
            device.BlendState = BlendState.AlphaBlend;
            device.SamplerStates[0] = SamplerState.PointClamp;

            VisibleChunkCoords v = GetVisibleChunkCoordinates(ref gdm, camera);

            foreach (EffectPass pass in singleTileShader.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (int y = v.minChunkY; y <= v.maxChunkY; y++)
                {
                    for (int x = v.minChunkX; x <= v.maxChunkX; x++)
                    {
                        if (!chunks.TryGetValue(new IntVector2(x, y), out TileChunk chunk))
                            continue;

                        // if the chunk was modified, bake it now
                        if (chunk.dirty)
                        {
                            chunk.RebuildVertexBuffer(device);
                        }

                        if (chunk.gpuVertexBuffer != null && chunk.gpuVertexBuffer.VertexCount > 0)
                        {
                            device.SetVertexBuffer(chunk.gpuVertexBuffer);

                            device.DrawPrimitives(
                                PrimitiveType.TriangleList,
                                0,
                                chunk.gpuVertexBuffer.VertexCount / 3
                            );
                        }
                    }
                }
            }
        }

        public VisibleChunkCoords GetVisibleChunkCoordinates(ref GraphicsDeviceManager gdm, IsoCamera camera)
        {
            int screenW = gdm.GraphicsDevice.Viewport.Width;
            int screenH = gdm.GraphicsDevice.Viewport.Height;

            Vector2 tl = Vector2.Zero;
            Vector2 tr = new Vector2(screenW, 0);
            Vector2 bl = new Vector2(0, screenH);
            Vector2 br = new Vector2(screenW, screenH);

            Vector2 wTL = RenderUtil.ScreenToWorld(tl, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wTR = RenderUtil.ScreenToWorld(tr, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wBL = RenderUtil.ScreenToWorld(bl, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wBR = RenderUtil.ScreenToWorld(br, TILEWIDTH, TILEHEIGHT, camera);

            int minX = (int)Math.Floor(Math.Min(Math.Min(wTL.X, wTR.X), Math.Min(wBL.X, wBR.X))) - 2;
            int maxX = (int)Math.Ceiling(Math.Max(Math.Max(wTL.X, wTR.X), Math.Max(wBL.X, wBR.X))) + 2;
            int minY = (int)Math.Floor(Math.Min(Math.Min(wTL.Y, wTR.Y), Math.Min(wBL.Y, wBR.Y))) - 2;
            int maxY = (int)Math.Ceiling(Math.Max(Math.Max(wTL.Y, wTR.Y), Math.Max(wBL.Y, wBR.Y))) + 2;

            int minChunkX = (int)Math.Floor((double)minX / TileChunk.CHUNKSIZE);
            int maxChunkX = (int)Math.Floor((double)maxX / TileChunk.CHUNKSIZE);
            int minChunkY = (int)Math.Floor((double)minY / TileChunk.CHUNKSIZE);
            int maxChunkY = (int)Math.Floor((double)maxY / TileChunk.CHUNKSIZE);

            int zMin = Math.Max(0, (int)camera.GetPosition().z - 1);
            int zMax = Math.Min(TileChunk.MAX_Z - 1, (int)camera.GetPosition().z + 1);

            return new VisibleChunkCoords(minChunkX, minChunkY, maxChunkX, maxChunkY, zMin, zMax, minX, minY, maxX, maxY);
        }

        IntVector2 lastChunkPosition = new IntVector2(0, 0);
        float lastZoom = 1.0f;
        public void Update(double deltaTime)
        {
            IntVector2 currentChunkPos = TileChunk.WorldToChunkPosition(SceneManager.GetWorld().activeCamera.GetPosition());
            float zoom = SceneManager.GetWorld().activeCamera.GetZoom();

            if (currentChunkPos != lastChunkPosition || lastZoom != zoom)
            {
                lastZoom = zoom;
                lastChunkPosition = currentChunkPos;

                IsoLog.Log("LogTilemap", $"Entered new chunk: ({currentChunkPos.x}, {currentChunkPos.y})");
            }
        }

        public struct VisibleChunkCoords
        {
            public VisibleChunkCoords(int minChunkX, int minChunkY, int maxChunkX, int maxChunkY, int minZ, int maxZ, int minX, int minY, int maxX, int maxY)
            {
                this.minChunkX = minChunkX;
                this.minChunkY = minChunkY;
                this.maxChunkX = maxChunkX;
                this.maxChunkY = maxChunkY;

                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;
                this.minZ = minZ;
                this.maxZ = maxZ;
            }

            public int minChunkX;
            public int maxChunkX;
            public int minChunkY;
            public int maxChunkY;

            public int minX;
            public int minY;
            public int maxX;
            public int maxY;
            public int minZ;
            public int maxZ;
        }
    }
}
