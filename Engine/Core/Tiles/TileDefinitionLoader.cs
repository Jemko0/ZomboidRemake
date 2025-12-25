using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Iso.Engine.Core.Assets;
using Iso.Engine.Core.Logging;

namespace Iso.Engine.Core.Tiles
{
    public struct TileDefLoadResult
    {
        public bool success;
        public string message;

        public TileDefLoadResult()
        {
            success = false;
            message = "Unspecified Error.";
        }

        public TileDefLoadResult(bool s, string m)
        {
            success = s;
            message = m;
        }
    }

    public class TileDefinitionData
    {
        public TileDefinitionData()
        {
            flags = new HashSet<string>();
        }

        public string name { get; set; }
        public string texture { get; set; }
        public HashSet<string> flags { get; set; }

        public Texture2D tileTexture;
    }


    public class TileDefinitionLoader
    {
        public TileDefinitionLoader()
        {
        }

        public static string workingDir = AppContext.BaseDirectory;
        public static string tileDefDir = Path.Combine(AssetHelper.tilesPath, "TileDefinitions.json");
        public static async Task<TileDefLoadResult> LoadTileDefinitions()
        {
            using var reader = File.OpenText(tileDefDir);
            string jsonContent = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(jsonContent))
                return new TileDefLoadResult(false, "Loaded JSON was empty.");

            var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent);

            if (root == null)
                return new TileDefLoadResult(false, "Failed to deserialize tile definition JSON.");

            TileDefinitions.definitions = new Dictionary<ETileType, TileDefinitionData>();

            foreach (var category in root)
            {
                foreach (var prop in category.Value.EnumerateObject())
                {
                    // Skip metadata
                    if (prop.NameEquals("friendly_name"))
                        continue;

                    // Convert key -> enum
                    if (!Enum.TryParse<ETileType>(prop.Name, out var tileType))
                    {
                        IsoLog.Log("LogTileDefinitionLoader", $"Unknown tile type '{prop.Name}' in JSON << Ignoring Tile Definition", ELogVerbosity.Warning);
                        continue;
                    }

                    var tileData = prop.Value.Deserialize<TileDefinitionData>();

                    if (TileDefinitions.definitions.ContainsKey(tileType))
                    {
                        return new TileDefLoadResult(
                            false,
                            $"Duplicate tile definition for '{tileType}'."
                        );
                    }

                    TileDefinitions.AddDefinition(tileType, tileData);
                }
            }

            return new TileDefLoadResult(true, "Loaded TileDefs Successfully");
        }
    }

    public static class TileDefinitions
    {
        public static Dictionary<ETileType, TileDefinitionData> definitions = new();

        public static void AddDefinition(ETileType type, TileDefinitionData data)
        {
            Texture2D texture = null;
            try
            {
                texture = AssetHelper.globalContentManager.Load<Texture2D>(data.texture);
            }
            catch (Exception e)
            {
                IsoLog.Log("LogTileDefinitionLoader", string.Format("Caught Exception: {0}", e.Message), ELogVerbosity.Debug);
                texture = AssetHelper.globalContentManager.Load<Texture2D>("tiles/missing");
            }
            data.tileTexture = texture;

            definitions[type] = data;
        }
    }
}
