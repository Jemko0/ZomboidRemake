using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Assets
{
    public static class AssetHelper
    {
        public static readonly string assetPath = Path.Combine(AppContext.BaseDirectory, "Assets");
        public static readonly string tilesPath = Path.Combine(AppContext.BaseDirectory, "Assets" , "Tiles");

        public static ContentManager globalContentManager;
    }
}
