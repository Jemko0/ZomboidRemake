using Iso.Engine.Core.Assets;
using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace Iso.Engine.Core.Rendering
{
    public class RenderUtil
    {
        public static GameWindow window;
        public static AssetHelper.TileAtlas tileAtlas = null;
        public static Vector2 WorldToScreen(FVector3 pos, float tileWidth, float tileHeight, IsoCamera camera)
        {
            var camPos = camera.GetPosition();

            float isoX = (pos.x - pos.y) * (tileWidth / 2f);
            float isoY = (pos.x + pos.y) * (tileHeight / 2f) - (pos.z * tileHeight / 2f);

            isoX -= camPos.x;
            isoY -= camPos.y - camPos.z;

            float scale = camera.GetZoom() * GetDPIScale();
            float screenX = isoX * scale;
            float screenY = isoY * scale;

            screenX += window.ClientBounds.Width / 2f;
            screenY += window.ClientBounds.Height / 2f;

            return new Vector2(screenX, screenY);
        }

        public static Vector2 ScreenToWorld(
            Vector2 screen,
            float tileWidth,
            float tileHeight,
            IsoCamera camera)
        {
            float x = screen.X - window.ClientBounds.Width / 2f;
            float y = screen.Y - window.ClientBounds.Height / 2f;

            float dpi = GetDPIScale();
            x /= dpi;
            y /= dpi;

            float zoom = camera.GetZoom();
            x /= zoom;
            y /= zoom;

            x += camera.GetPosition().x;
            y += camera.GetPosition().y + camera.GetPosition().z;

            float halfW = tileWidth / 2f;
            float halfH = tileHeight / 2f;

            float worldX = (x / halfW + y / halfH) / 2f;
            float worldY = (y / halfH - x / halfW) / 2f;

            return new Vector2(worldX, worldY);
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
