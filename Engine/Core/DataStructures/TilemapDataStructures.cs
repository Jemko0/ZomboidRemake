using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.DataStructures
{
    public enum ETileType : UInt16
    {
        NONE = 0,

        GRASS_01 = 100,
        GRASS_02,
        GRASS_03,

        DIRT_01 = 200,
    }

    public struct FTileData
    {
        public FTileData()
        {
            type = ETileType.NONE;
        }

        public FTileData(ETileType type)
        {
            this.type = type;
        }

        public ETileType type;

        public bool IsNone()
        {
            return type == ETileType.NONE;
        }
    }
}
