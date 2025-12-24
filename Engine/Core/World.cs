using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iso.Engine.Core.Tiles;
using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering;
using System.Reflection.Metadata.Ecma335;

namespace Iso.Engine.Core
{
    public class World : IUpdateable
    {
        public Tilemap tilemap;
        protected string mapFilePath = "";

        public IsoCamera activeCamera;
        public virtual void Init()
        {
            
        }

        public virtual async void InitAsync()
        {
        }

        public virtual void Render(ref Microsoft.Xna.Framework.GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            IsoRenderContext isoRenderContext = new IsoRenderContextBuilder()
                                                        .WithCamera(activeCamera)
                                                        .WithExtra("debug", true)
                                                        .Build();

            if(tilemap == null)
            {
                return;
            }

            tilemap.Render(ref gdm, ref sb, ref isoRenderContext);
        }

        public virtual void Unload()
        {
            tilemap = null;
        }

        public virtual void Update(double deltaTime)
        {
            
        }
    }
}
