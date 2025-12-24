using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Rendering
{
    public class RenderUtil
    {
        public static GameWindow window;
        public static Vector2 WorldToScreen(FVector3 pos, float tileWidth, float tileHeight, IsoCamera camera)
        {
            float screenX = (pos.x - pos.y) * (tileWidth / 2f) - camera.GetPosition().x;
            float screenY = (pos.x + pos.y) * (tileHeight / 2f) - (pos.z * tileHeight / 2f) - camera.GetPosition().y - camera.GetPosition().z;

            //apply zoom
            screenX *= camera.GetZoom();
            screenY *= camera.GetZoom();

            //apply dpi
            float dpiScale = GetDPIScale();
            screenX *= dpiScale;
            screenY *= dpiScale;

            //center to screen
            screenX += window.ClientBounds.Width / 2f;
            screenY += window.ClientBounds.Height / 2f;

            return new Vector2(screenX, screenY);
        }

        public static float GetOnScreenSize(float originalSize)
        {
            return originalSize * GetDPIScale();
        }

        public static float GetDPIScale()
        {
            float widthScale = window.ClientBounds.Width / 1920f;
            float heightScale = window.ClientBounds.Height / 1080f;

            return Math.Max(widthScale, heightScale);
        }
    }
}
