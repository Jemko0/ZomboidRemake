using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DXGI;

namespace Iso.Engine.Core.UI.Elements
{
    public class VStack : Panel
    {
        public int spacing = 0;

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            int currentYOffset = 0;
            var children = GetChildren();

            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = children[i];

                Rectangle childSlot = new Rectangle(
                    actualRect.X,
                    actualRect.Y + currentYOffset,
                    actualRect.Width,
                    child.bounds.Height
                );

                child.Render(ref gdm, ref sb, ref renderContext, childSlot);
                currentYOffset += child.bounds.Height + spacing;
            }
        }
    }
}
