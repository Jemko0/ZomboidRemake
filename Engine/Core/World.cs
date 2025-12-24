using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iso.Engine.Core.Tiles;

namespace Iso.Engine.Core
{
    internal class World
    {
        public Tilemap tilemap;
        protected string mapFilePath = "";

        public void Init()
        {

        }

        public void Unload()
        {
            tilemap = null;
        }
    }
}
