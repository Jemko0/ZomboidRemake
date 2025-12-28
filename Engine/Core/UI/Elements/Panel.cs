using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class Panel : UIElement
    {
        private List<UIElement> children = new List<UIElement>();

        public override List<UIElement> GetChildren()
        {
            return children;
        }

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            for (int i = 0; i < children.Count; i++)
            {
                UIElement child = children[i];
                child.Render(ref gdm, ref sb, ref renderContext, actualRect);
            }
        }
    }
}
