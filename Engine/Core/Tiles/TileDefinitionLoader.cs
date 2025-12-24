using Iso.Engine.Core.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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

    public struct TileDefinitionData
    {
        public string name;
        public bool walkable;
    }


    public class TileDefinitionLoader
    {
        public TileDefinitionLoader() { }
        public static string workingDir = AppContext.BaseDirectory;
        public static string tileDefDir = workingDir + "Engine/Core/Tiles/TileDefinitions.json";
        public static async Task<TileDefLoadResult> LoadTileDefinitions()
        {
            using (StreamReader reader = File.OpenText(tileDefDir))
            {
                string jsonContent = await reader.ReadToEndAsync();

                if(jsonContent == "")
                {
                    return new TileDefLoadResult(false, "Loaded JSON was empty.");
                }

                Dictionary<string, TileDefinitionData> tileDefs = JsonSerializer.Deserialize<Dictionary<string, TileDefinitionData>>(jsonContent);

                if(tileDefs == null)
                {
                    return new TileDefLoadResult(false, "Failed to deserialize tile definition JSON.");
                }

                //@todo: finish parsing etc.
            }

            return new TileDefLoadResult(true, "Loaded TileDefs Successfully");
        }
    }

    public static class TileDefinitions
    {
        public static Dictionary<ETileType, TileDefinitionData> definitions;
    }
}
