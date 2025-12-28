using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.UI.Elements
{
    public class Spacer : UIElement
    {
        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            return;
        }
    }
}
