using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.UI.Elements
{
    public enum ImageDrawMode : byte
    {
        /// <summary>
        /// Draw as a simple image
        /// </summary>
        Image,

        /// <summary>
        /// Draw as a border with a transparent center
        /// </summary>
        Border,

        /// <summary>
        /// Draw as a box with a filled center
        /// </summary>
        Box
    }
    public class Image : UIElement
    {
        public Texture2D brush;
        public Color tint = Color.White;
        public ImageDrawMode drawMode = ImageDrawMode.Image;
        public int sliceSize = 8; // Number of pixels for corners

        public Image SetBrush(Texture2D tex) { brush = tex; return this; }
        public Image SetDrawMode(ImageDrawMode mode) { drawMode = mode; return this; }
        public Image SetSliceSize(int size) { sliceSize = size; return this; }

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            if (brush == null) return;

            if (drawMode == ImageDrawMode.Image)
            {
                sb.Draw(brush, actualRect, tint);
            }
            else
            {
                RenderNineSlice(sb, actualRect);
            }
        }

        private void RenderNineSlice(SpriteBatch sb, Rectangle dest)
        {
            int s = sliceSize;
            int texW = brush.Width;
            int texH = brush.Height;

            // source rects
            Rectangle[] sources = new Rectangle[]
            {
                new Rectangle(0, 0, s, s),             // Top Left
                new Rectangle(s, 0, texW - 2*s, s),     // Top Middle
                new Rectangle(texW - s, 0, s, s),       // Top Right
                new Rectangle(0, s, s, texH - 2*s),     // Left Middle
                new Rectangle(s, s, texW - 2*s, texH - 2*s), // Center
                new Rectangle(texW - s, s, s, texH - 2*s),   // Right Middle
                new Rectangle(0, texH - s, s, s),       // Bottom Left
                new Rectangle(s, texH - s, texW - 2*s, s),   // Bottom Middle
                new Rectangle(texW - s, texH - s, s, s)      // Bottom Right
            };

            // destination rects (Where they draw on screen)
            Rectangle[] destinations = new Rectangle[]
            {
                new Rectangle(dest.X, dest.Y, s, s),
                new Rectangle(dest.X + s, dest.Y, dest.Width - 2*s, s),
                new Rectangle(dest.Right - s, dest.Y, s, s),
                new Rectangle(dest.X, dest.Y + s, s, dest.Height - 2*s),
                new Rectangle(dest.X + s, dest.Y + s, dest.Width - 2*s, dest.Height - 2*s),
                new Rectangle(dest.Right - s, dest.Y + s, s, dest.Height - 2*s),
                new Rectangle(dest.X, dest.Bottom - s, s, s),
                new Rectangle(dest.X + s, dest.Bottom - s, dest.Width - 2*s, s),
                new Rectangle(dest.Right - s, dest.Bottom - s, s, s)
            };

            for (int i = 0; i < 9; i++)
            {
                // If mode is Border, skip the center slice
                if (drawMode == ImageDrawMode.Border && i == 4) continue; //center is index 4

                sb.Draw(brush, destinations[i], sources[i], tint);
            }
        }
    }
}
