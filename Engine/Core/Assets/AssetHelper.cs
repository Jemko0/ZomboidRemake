using Iso.Engine.Core.Logging;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Iso.Engine.Core.Assets
{
    public static class AssetHelper
    {
        public static readonly string assetPath = Path.Combine(AppContext.BaseDirectory, "Assets");
        public static readonly string tilesPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Tiles");
        public static ContentManager globalContentManager;
        public static string contentTileDirPath { get; private set; } = "";
        public static string generatedAtlasPath { get; private set; } = "";
        public static TileAtlas RuntimeAtlas { get; private set; } = null!;

        public static void Setup()
        {
            contentTileDirPath = Path.Combine(globalContentManager.RootDirectory, "tiles");
            generatedAtlasPath = Path.Combine(tilesPath, "generated", "atlas");
        }

        public static TileAtlas GenerateTileTextureAtlas(GraphicsDevice graphicsDevice)
        {
            string tileDefPath = Path.Combine(tilesPath, "TileDefinitions.json");
            if (!File.Exists(tileDefPath))
                throw new FileNotFoundException("Tile definition file not found", tileDefPath);

            using var stream = File.OpenRead(tileDefPath);
            var tileDefs = JsonDocument.Parse(stream).RootElement;

            var tileList = new List<(string name, string texturePath)>();

            foreach (var category in tileDefs.EnumerateObject())
            {
                foreach (var tileEntry in category.Value.EnumerateObject())
                {
                    if (tileEntry.Value.ValueKind != JsonValueKind.Object)
                        continue; // skip strings like "friendly_name"

                    if (tileEntry.Value.TryGetProperty("texture", out var textureProp))
                    {
                        string tileName = tileEntry.Name;
                        string texturePath = textureProp.GetString()!;
                        tileList.Add((tileName, texturePath));
                    }
                }
            }

            IsoLog.Log("LogAtlas", $"Found {tileList.Count} tiles in definitions.");

            if (tileList.Count == 0)
            {
                throw new InvalidOperationException("No tiles found in definitions.");
            }

            // Load all textures
            List<Texture2D> textures = new();
            foreach (var (name, path) in tileList)
            {
                textures.Add(globalContentManager.Load<Texture2D>(path));
            }

            int tileWidth = textures[0].Width;
            int tileHeight = textures[0].Height;

            int atlasColumns = (int)Math.Ceiling(Math.Sqrt(textures.Count));
            int atlasRows = (int)Math.Ceiling((float)textures.Count / atlasColumns);

            int atlasWidth = atlasColumns * tileWidth;
            int atlasHeight = atlasRows * tileHeight;

            Texture2D atlas = new Texture2D(graphicsDevice, atlasWidth, atlasHeight);
            Color[] atlasData = new Color[atlasWidth * atlasHeight];

            Dictionary<string, TileRegion> tileRegions = new();

            for (int i = 0; i < textures.Count; i++)
            {
                Texture2D tile = textures[i];
                Color[] tileData = new Color[tileWidth * tileHeight];
                tile.GetData(tileData);

                int x = (i % atlasColumns) * tileWidth;
                int y = (i / atlasColumns) * tileHeight;

                for (int ty = 0; ty < tileHeight; ty++)
                    for (int tx = 0; tx < tileWidth; tx++)
                        atlasData[(x + tx) + (y + ty) * atlasWidth] = tileData[tx + ty * tileWidth];

                tileRegions[tileList[i].name] = new TileRegion { X = x, Y = y, W = tileWidth, H = tileHeight };
            }

            atlas.SetData(atlasData);

            generatedAtlasPath = Path.Combine(tilesPath, "generated", "atlas");
            Directory.CreateDirectory(generatedAtlasPath);

            string atlasPath = Path.Combine(generatedAtlasPath, "tiles_atlas.png");
            using (var fs = File.Create(atlasPath))
                atlas.SaveAsPng(fs, atlasWidth, atlasHeight);
            IsoLog.Log("LogAtlas", $"Atlas saved to: {atlasPath}");

            TileAtlasMeta meta = new TileAtlasMeta
            {
                FormatVersion = 1,
                GeneratorVersion = "1.0.0",
                SourceHash = ComputeAtlasHash(generatedAtlasPath),
                AtlasSize = new AtlasSize { Width = atlasWidth, Height = atlasHeight },
                TileSize = new TileSize { Width = tileWidth, Height = tileHeight },
                Tiles = tileRegions
            };

            string metaPath = Path.Combine(generatedAtlasPath, "tiles_atlas.json");
            File.WriteAllText(metaPath, JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
            IsoLog.Log("LogAtlas", $"Metadata saved to: {metaPath}");

            RuntimeAtlas = new TileAtlas(atlas, meta);
            return RuntimeAtlas;
        }

        private static string ComputeAtlasHash(string atlasPath)
        {
            atlasPath = Path.Combine(atlasPath, "tiles_atlas.png");
            if (!File.Exists(atlasPath))
                throw new FileNotFoundException("Atlas file not found", atlasPath);

            using var stream = File.OpenRead(atlasPath);
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }

        public sealed class TileAtlas
        {
            public Texture2D Texture { get; }
            public TileAtlasMeta Meta { get; }

            public TileAtlas(Texture2D texture, TileAtlasMeta meta)
            {
                Texture = texture;
                Meta = meta;
            }

            public Rectangle GetSourceRect(string tileName)
            {
                var r = Meta.Tiles[tileName];
                return new Rectangle(r.X, r.Y, r.W, r.H);
            }
        }

        public sealed class TileAtlasMeta
        {
            public int FormatVersion { get; set; }
            public string GeneratorVersion { get; set; } = "";
            public string SourceHash { get; set; } = "";
            public AtlasSize AtlasSize { get; set; } = new();
            public TileSize TileSize { get; set; } = new();
            public Dictionary<string, TileRegion> Tiles { get; set; } = new();
        }

        public sealed class AtlasSize { public int Width { get; set; } public int Height { get; set; } }
        public sealed class TileSize { public int Width { get; set; } public int Height { get; set; } }
        public sealed class TileRegion { public int X { get; set; } public int Y { get; set; } public int W { get; set; } public int H { get; set; } }
    }
}
