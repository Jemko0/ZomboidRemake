using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Rendering
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    public struct TileVertex : IVertexType
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Color Color;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public TileVertex(Vector3 pos, Vector2 uv, Color col)
        {
            Position = pos;
            TexCoord = uv;
            Color = col;
        }
    }

    public class TileRendering
    {
        public static void AddTileQuad(ref List<TileVertex> vertices, float x, float y, float tileWidth, float tileHeight)
        {
            // First triangle
            vertices.Add(new TileVertex(new Vector3(x, y, 0), new Vector2(0, 0), Color.White));
            vertices.Add(new TileVertex(new Vector3(x + tileWidth, y, 0), new Vector2(1, 0), Color.White));
            vertices.Add(new TileVertex(new Vector3(x, y + tileHeight, 0), new Vector2(0, 1), Color.White));

            // Second triangle
            vertices.Add(new TileVertex(new Vector3(x + tileWidth, y, 0), new Vector2(1, 0), Color.White));
            vertices.Add(new TileVertex(new Vector3(x + tileWidth, y + tileHeight, 0), new Vector2(1, 1), Color.White));
            vertices.Add(new TileVertex(new Vector3(x, y + tileHeight, 0), new Vector2(0, 1), Color.White));
        }
    }
}
