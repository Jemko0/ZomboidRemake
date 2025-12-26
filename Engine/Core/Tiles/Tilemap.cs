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
                    TileChunk c = new TileChunk();
                    c.BasicFillWithTiles();

                    chunks.Add(new IntVector2(x, y), c);
                }
            }

            UpdateVertexBufferTimed();
        }

        public void UpdateVertexBufferTimed()
        {
            DateTime start = DateTime.Now;
            UpdateVertexBuffer();
            DateTime end = DateTime.Now;

            TimeSpan time = end - start;

            IsoLog.Log("LogTilemap", string.Format("UpdateVertexBuffer() took {0}ms", time.TotalMilliseconds), ELogVerbosity.Debug);
        }

        public void UpdateVertexBuffer()
        {
            GraphicsDeviceManager gdm = IsoGame.graphics;
            VisibleChunkCoords v = GetVisibleChunkCoordinates(ref gdm, SceneManager.GetWorld().activeCamera);

            int vertexCount = 0;

            for (int chunkY = v.minChunkY; chunkY <= v.maxChunkY; chunkY++)
            {
                for (int chunkX = v.minChunkX; chunkX <= v.maxChunkX; chunkX++)
                {
                    if (!chunks.TryGetValue(new IntVector2(chunkX, chunkY), out var chunk))
                    {
                        continue;
                    }

                    int startX = Math.Max(0, v.minX - chunkX * TileChunk.CHUNKSIZE);
                    int endX = Math.Min(TileChunk.CHUNKSIZE - 1, v.maxX - chunkX * TileChunk.CHUNKSIZE);
                    int startY = Math.Max(0, v.minY - chunkY * TileChunk.CHUNKSIZE);
                    int endY = Math.Min(TileChunk.CHUNKSIZE - 1, v.maxY - chunkY * TileChunk.CHUNKSIZE);

                    for (int z = v.minZ; z <= v.maxZ; z++)
                    {
                        for (int lx = startX; lx <= endX; lx++)
                        {
                            for (int ly = startY; ly <= endY; ly++)
                            {
                                int worldX = chunkX * TileChunk.CHUNKSIZE + lx;
                                int worldY = chunkY * TileChunk.CHUNKSIZE + ly;

                                List<TileObject> objects = chunk.tiles[lx, ly, z]?.objects;

                                if (objects == null)
                                {
                                    continue;
                                }

                                foreach (TileObject o in objects)
                                {
                                    if(o == null)
                                    {
                                        continue;
                                    }

                                    if (vertexCount + VERTICES_PER_TILE > vertexBuffer.Length)
                                    {
                                        IsoLog.Log("LogTilemap", "Vertex buffer full!");
                                        actualVertexCount = vertexCount;
                                        return;
                                    }

                                    TileRendering.AddTileQuad(vertexBuffer, ref vertexCount, worldX, worldY, TILEWIDTH, TILEHEIGHT, o.type);
                                }
                            }
                        }
                    }
                }
            }

            actualVertexCount = vertexCount;
        }

        public void Dispose()
        {
            chunks.Clear();
            chunks = null;
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            GPURender(ref gdm, ref sb, ref renderContext);
            return;

            IsoCamera camera = renderContext.camera;
            if (camera == null || chunks == null)
                return;

            Vector2 tl = Vector2.Zero;
            Vector2 tr = new Vector2(gdm.PreferredBackBufferWidth, 0);
            Vector2 bl = new Vector2(0, gdm.PreferredBackBufferHeight);
            Vector2 br = new Vector2(gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight);

            Vector2 wTL = RenderUtil.ScreenToWorld(tl, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wTR = RenderUtil.ScreenToWorld(tr, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wBL = RenderUtil.ScreenToWorld(bl, TILEWIDTH, TILEHEIGHT, camera);
            Vector2 wBR = RenderUtil.ScreenToWorld(br, TILEWIDTH, TILEHEIGHT, camera);

            int minX = (int)Math.Floor(Math.Min(Math.Min(wTL.X, wTR.X), Math.Min(wBL.X, wBR.X))) - 2;
            int maxX = (int)Math.Ceiling(Math.Max(Math.Max(wTL.X, wTR.X), Math.Max(wBL.X, wBR.X))) + 2;
            int minY = (int)Math.Floor(Math.Min(Math.Min(wTL.Y, wTR.Y), Math.Min(wBL.Y, wBR.Y))) - 2;
            int maxY = (int)Math.Ceiling(Math.Max(Math.Max(wTL.Y, wTR.Y), Math.Max(wBL.Y, wBR.Y))) + 2;

            int minChunkX = Math.DivRem(minX, TileChunk.CHUNKSIZE, out int _);
            int maxChunkX = Math.DivRem(maxX, TileChunk.CHUNKSIZE, out int _);
            int minChunkY = Math.DivRem(minY, TileChunk.CHUNKSIZE, out int _);
            int maxChunkY = Math.DivRem(maxY, TileChunk.CHUNKSIZE, out int _);

            if (minX < 0) minChunkX--;
            if (minY < 0) minChunkY--;

            int zMin = Math.Max(0, (int)camera.GetPosition().z - 1);
            int zMax = Math.Min(TileChunk.MAX_Z - 1, (int)camera.GetPosition().z + 1);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int chunkY = minChunkY; chunkY <= maxChunkY; chunkY++)
            {
                for (int chunkX = minChunkX; chunkX <= maxChunkX; chunkX++)
                {
                    if (!chunks.TryGetValue(new IntVector2(chunkX, chunkY), out var chunk))
                        continue;

                    int startX = Math.Max(0, minX - chunkX * TileChunk.CHUNKSIZE);
                    int endX = Math.Min(TileChunk.CHUNKSIZE - 1, maxX - chunkX * TileChunk.CHUNKSIZE);
                    int startY = Math.Max(0, minY - chunkY * TileChunk.CHUNKSIZE);
                    int endY = Math.Min(TileChunk.CHUNKSIZE - 1, maxY - chunkY * TileChunk.CHUNKSIZE);

                    for (int z = zMin; z <= zMax; z++)
                    {
                        for (int lx = startX; lx <= endX; lx++)
                        {
                            for (int ly = startY; ly <= endY; ly++)
                            {
                                SquareTileData square = chunk.tiles[lx, ly, z];
                                if (square?.objects == null)
                                    continue;

                                int worldX = chunkX * TileChunk.CHUNKSIZE + lx;
                                int worldY = chunkY * TileChunk.CHUNKSIZE + ly;

                                FVector3 worldPos = new FVector3(worldX, worldY, z);
                                Vector2 screenPos = RenderUtil.WorldToScreen(worldPos, TILEWIDTH, TILEHEIGHT, camera);
                                float scale = RenderUtil.GetOnScreenSize(camera.GetZoom());

                                foreach (var obj in square.objects)
                                {
                                    if (obj.type == ETileType.NONE)
                                        continue;

                                    TileDefinitionData def = TileDefinitions.definitions[obj.type];
                                    Vector2 origin = new Vector2(def.tileTexture.Width / 2f, def.tileTexture.Height);

                                    sb.Draw(
                                        def.tileTexture,
                                        screenPos,
                                        null,
                                        Color.White,
                                        0f,
                                        origin,
                                        scale,
                                        SpriteEffects.None,
                                        0.0f
                                    );

                                    //sb.DrawString(Fonts.monospace, string.Format("{0} {1}", lx, ly), screenPos, Color.Black);
                                }
                            }
                        }
                    }
                }
            }
            sb.End();
        }

        public void GPURender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            IsoCamera camera = renderContext.camera;
            Effect singleTileShader = renderContext.GetExtra<Effect>("single_tile_shader");
            GraphicsDevice device = gdm.GraphicsDevice;

            Rectangle viewport = device.Viewport.Bounds;
            float zoom = camera.GetZoom();

            float halfWidth = viewport.Width / (2f * zoom);
            float halfHeight = viewport.Height / (2f * zoom);

            Matrix projection = Matrix.CreateOrthographicOffCenter(
                -halfWidth,     // left
                halfWidth,      // right
                halfHeight,     // bottom
                -halfHeight,    // top
                0.0f,           // near
                1.0f            // far
            );

            Matrix view = camera.viewMatrix;
            Matrix world = Matrix.Identity;

            Matrix wvp = world * view * projection;

            singleTileShader.Parameters["WorldViewProjection"].SetValue(wvp);
            singleTileShader.Parameters["TileTexture"]?.SetValue(RenderUtil.tileAtlas.Texture);;

            device.RasterizerState = new RasterizerState { CullMode = CullMode.None };
            device.DepthStencilState = DepthStencilState.None;

            if(actualVertexCount < 3)
            {
                return;
            }

            foreach (EffectPass pass in singleTileShader.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserPrimitives<TileVertex>(
                    PrimitiveType.TriangleList,
                    vertexBuffer,
                    0,
                    actualVertexCount / 3
                );
            }
        }

        public VisibleChunkCoords GetVisibleChunkCoordinates(ref GraphicsDeviceManager gdm, IsoCamera camera)
        {
            float zoom = camera.GetZoom();

            float viewportWidth = gdm.PreferredBackBufferWidth / zoom;
            float viewportHeight = gdm.PreferredBackBufferHeight / zoom;

            Vector2 tl = Vector2.Zero;
            Vector2 tr = new Vector2(viewportWidth, 0);
            Vector2 bl = new Vector2(0, viewportHeight);
            Vector2 br = new Vector2(viewportWidth, viewportHeight);

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
                UpdateVertexBufferTimed();
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
