using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Rendering
{
    using Iso.Engine.Core.DataStructures;
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
        public static void AddTileQuad(TileVertex[] buffer, ref int index, int tx, int ty, float tileWidth, float tileHeight, ETileType type)
        {
            float x = (tx - ty) * (tileWidth / 2.0f);
            float y = (tx + ty) * (tileHeight / 2.0f);

            float textureWidth = 32;
            float textureHeight = 48;

            Color instanceData = new Color((int)type, 0, 0, 255);

            float yOffset = textureHeight - tileHeight;

            // First triangle
            buffer[index++] = new TileVertex(new Vector3(x, y - yOffset, 0), new Vector2(0, 0), instanceData);
            buffer[index++] = new TileVertex(new Vector3(x + textureWidth, y - yOffset, 0), new Vector2(1, 0), instanceData);
            buffer[index++] = new TileVertex(new Vector3(x, y - yOffset + textureHeight, 0), new Vector2(0, 1), instanceData);

            // Second triangle
            buffer[index++] = new TileVertex(new Vector3(x + textureWidth, y - yOffset, 0), new Vector2(1, 0), instanceData);
            buffer[index++] = new TileVertex(new Vector3(x + textureWidth, y - yOffset + textureHeight, 0), new Vector2(1, 1), instanceData);
            buffer[index++] = new TileVertex(new Vector3(x, y - yOffset + textureHeight, 0), new Vector2(0, 1), instanceData);
        }
    }
}
