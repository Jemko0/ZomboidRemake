using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.UI.Elements
{
    public class HStack : Panel
    {
        public int spacing = 0;

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            var children = GetChildren();
            int fixedWidths = 0;
            int fillWeights = 0;

            // calculate how much space is taken by fixed items
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].visibility == UIVisibilityMode.COLLAPSED) continue;

                // If anchors are 0, it's fixed size
                if (children[i].anchorMin.X == children[i].anchorMax.X)
                {
                    fixedWidths += children[i].bounds.Width + spacing;
                }
                else
                {
                    fillWeights++; // its a fill item
                }
            }

            int remainingSpace = actualRect.Width - fixedWidths;
            int widthPerFillItem = fillWeights > 0 ? remainingSpace / fillWeights : 0;

            // render
            int currentXOffset = 0;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = children[i];
                if (child.visibility == UIVisibilityMode.COLLAPSED) continue;

                int calculatedWidth;
                if (child.anchorMin.X == child.anchorMax.X)
                {
                    calculatedWidth = child.bounds.Width; // Use fixed
                }
                else
                {
                    calculatedWidth = widthPerFillItem;   // Use the shared fill width
                }

                Rectangle childSlot = new Rectangle(
                    actualRect.X + currentXOffset,
                    actualRect.Y,
                    calculatedWidth,
                    actualRect.Height
                );

                child.Render(ref gdm, ref sb, ref renderContext, childSlot);
                currentXOffset += calculatedWidth + spacing;
            }
        }
    }
}
