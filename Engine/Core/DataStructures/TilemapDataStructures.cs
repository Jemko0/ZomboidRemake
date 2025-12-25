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

        F_DEBUG,
        W_DEBUG,

        F_GRASS_01,
    }

    public class SquareTileData
    {
        public List<TileObject> objects;

        public SquareTileData()
        {
            objects = new List<TileObject>();
        }

        public SquareTileData(List<TileObject> initializerList)
        {
            objects = initializerList;
        }

        public void AddObject(TileObject obj)
        {
            objects.Add(obj);
        }

        public void RemoveObject(TileObject obj)
        {
            objects.Remove(obj);
        }
    }

    public class TileObject
    {
        public ETileType type;
        public bool rotated = false;

        public TileObject(ETileType type)
        {
            this.type = type;
        }
    }
} 
