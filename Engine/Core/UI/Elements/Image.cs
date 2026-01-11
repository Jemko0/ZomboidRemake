using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            if (brush == null)
            {
                //IsoLog.Log("LogImage", "brush was null!");
                return;
            }

            if (drawMode == ImageDrawMode.Image)
            {
                sb.Draw(brush, actualRect, tint);
            }
            else
            {
                RenderNineSlice(sb, actualRect);
            }
        }

        public override void OnDestroy()
        {
            brush = null;
        }

        private void RenderNineSlice(SpriteBatch sb, Rectangle dest)
        {
            int s = sliceSize;
            int texW = brush.Width;
            int texH = brush.Height;

            if (texW <= 2 * s || texH <= 2 * s || s <= 0)
            {
                sb.Draw(brush, dest, tint);
                return;
            }

            int renderS_X = Math.Min(s, dest.Width / 2);
            int renderS_Y = Math.Min(s, dest.Height / 2);

            int middleSrcW = texW - 2 * s;
            int middleSrcH = texH - 2 * s;

            Rectangle[] sources = new Rectangle[]
            {
                new Rectangle(0, 0, s, s),                     // 0: Top Left
                new Rectangle(s, 0, middleSrcW, s),            // 1: Top Middle
                new Rectangle(texW - s, 0, s, s),              // 2: Top Right
                new Rectangle(0, s, s, middleSrcH),            // 3: Left Middle
                new Rectangle(s, s, middleSrcW, middleSrcH),   // 4: Center
                new Rectangle(texW - s, s, s, middleSrcH),     // 5: Right Middle
                new Rectangle(0, texH - s, s, s),              // 6: Bottom Left
                new Rectangle(s, texH - s, middleSrcW, s),     // 7: Bottom Middle
                new Rectangle(texW - s, texH - s, s, s)        // 8: Bottom Right
            };

            // dest rects
            int middleDestW = dest.Width - 2 * renderS_X;
            int middleDestH = dest.Height - 2 * renderS_Y;

            Rectangle[] destinations = new Rectangle[]
            {
                new Rectangle(dest.X, dest.Y, renderS_X, renderS_Y),                                 // 0: TL
                new Rectangle(dest.X + renderS_X, dest.Y, middleDestW, renderS_Y),                  // 1: TM
                new Rectangle(dest.Right - renderS_X, dest.Y, renderS_X, renderS_Y),                // 2: TR
                new Rectangle(dest.X, dest.Y + renderS_Y, renderS_X, middleDestH),                  // 3: LM
                new Rectangle(dest.X + renderS_X, dest.Y + renderS_Y, middleDestW, middleDestH),    // 4: C
                new Rectangle(dest.Right - renderS_X, dest.Y + renderS_Y, renderS_X, middleDestH),  // 5: RM
                new Rectangle(dest.X, dest.Bottom - renderS_Y, renderS_X, renderS_Y),               // 6: BL
                new Rectangle(dest.X + renderS_X, dest.Bottom - renderS_Y, middleDestW, renderS_Y), // 7: BM
                new Rectangle(dest.Right - renderS_X, dest.Bottom - renderS_Y, renderS_X, renderS_Y)// 8: BR
            };

            for (int i = 0; i < 9; i++)
            {
                //skip center if we are in Border mode
                if (drawMode == ImageDrawMode.Border && i == 4) continue;

                // (prevents drawing rectangles with 0 width/height)
                if (destinations[i].Width > 0 && destinations[i].Height > 0 &&
                    sources[i].Width > 0 && sources[i].Height > 0)
                {
                    sb.Draw(brush, destinations[i], sources[i], tint);
                }
            }
        }
    }
}
