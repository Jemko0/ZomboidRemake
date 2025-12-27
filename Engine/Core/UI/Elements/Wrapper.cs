using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.UI.Elements
{
    public class Wrapper : SingleChildElement
    {
        public Image background = null;

        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            //DrawOutline(sb, actualRect, Color.DarkGreen, 2);

            background?.Render(ref gdm, ref sb, ref renderContext, actualRect);
            RenderSingleChild(ref gdm, ref sb, ref renderContext, actualRect);
        }

        public Wrapper SetBackground(Image bg)
        { 
            background = bg; 
            return this; 
        }
    }
}
