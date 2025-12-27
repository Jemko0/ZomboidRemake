using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class SingleChildElement : UIElement
    {
        public UIElement child = null;
        public IntVector2 innerSlotPadding = new(0, 0);

        public override List<UIElement> GetChildren()
        {
            return new List<UIElement> { child };
        }

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            RenderSingleChild(ref gdm, ref sb, ref renderContext, actualRect);
        }

        public void RenderSingleChild(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            if (child == null)
            {
                return;
            }

            Rectangle padded = actualRect;
            padded.Inflate(-innerSlotPadding.x, -innerSlotPadding.y);
            child.Render(ref gdm, ref sb, ref renderContext, padded);
        }
    }
}
