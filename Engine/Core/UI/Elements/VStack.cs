using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class VStack : Panel
    {
        public int spacing = 0;

        public int GetTotalContentHeight()
        {
            int total = 0;
            List<UIElement> children = GetChildren();
            foreach (var child in children)
            {
                if (child.visibility == UIVisibilityMode.COLLAPSED) continue;
                total += child.bounds.Height + spacing;
            }
            return total;
        }

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            bounds.Height = GetTotalContentHeight();

            var children = GetChildren();
            int fixedHeights = 0;
            int fillWeights = 0;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].visibility == UIVisibilityMode.COLLAPSED) continue;

                if (children[i].anchorMin.Y == children[i].anchorMax.Y)
                {
                    fixedHeights += children[i].bounds.Height + spacing;
                }
                else
                {
                    fillWeights++;
                }
            }

            int remainingSpace = actualRect.Height;
            int heightPerFillItem = fillWeights > 0 ? remainingSpace / fillWeights : 0;

            // render
            int currentYOffset = 0;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = children[i];
                if (child.visibility == UIVisibilityMode.COLLAPSED) continue;

                int calculatedHeight;
                if (child.anchorMin.Y == child.anchorMax.Y)
                {
                    calculatedHeight = child.bounds.Height; // Use fixed
                }
                else
                {
                    calculatedHeight = heightPerFillItem;   // Use the shared fill width
                }

                Rectangle childSlot = new Rectangle(
                    actualRect.X,
                    actualRect.Y + currentYOffset,
                    actualRect.Width,
                    calculatedHeight
                );

                child.Render(ref gdm, ref sb, ref renderContext, childSlot);
                currentYOffset += calculatedHeight + spacing;
            }
        }
    }
}
