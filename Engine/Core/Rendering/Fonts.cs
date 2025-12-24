using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace Iso.Engine.Core.Rendering
{
    public static class Fonts
    {
        public static ContentManager cnt;

        public static SpriteFont arial { get; private set; }

        public static void SetupFonts(ContentManager cntmgr)
        {
            cnt = cntmgr;
            arial = cnt.Load<SpriteFont>("fonts/arial");
        }
        
    }
}
