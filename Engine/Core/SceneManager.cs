using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core
{
    public class SceneManager : IIsoUpdateable
    {
        protected static World loadedWorld;
        protected static World pendingWorld;
        protected static Input inputManager = new Input();

        public static Input GetInputManager() { return inputManager; }

        public static void LoadWorldDeferred<T>() where T : World
        {
            World instance = Activator.CreateInstance<T>();
            pendingWorld = instance;

            IsoLog.Log("LogSceneManager", "Loading World Deferred...");
        }

        public void LoadWorldImmediate<T>() where T : World
        {
            T newWorld = Activator.CreateInstance<T>();
            newWorld.Init();

            loadedWorld = newWorld;
        }

        public static World GetWorld()
        {
            return loadedWorld;
        }

        private unsafe void LoadPendingWorld()
        {
            World pending = pendingWorld;
            pendingWorld = null;
            loadedWorld = pending;
            loadedWorld.Init();


            IsoLog.Log("LogSceneManager", string.Format("Pending World Loaded: {0}", loadedWorld.GetType()));
        }

        public void UnloadWorld()
        {
            if (loadedWorld == null) return;

            loadedWorld.Unload();
            loadedWorld = null;

            IsoLog.Log("LogSceneManager", "Loaded World Unloaded!");
        }

        public void Update(double deltaTime)
        {
            inputManager?.Update(deltaTime);

            if (loadedWorld != null)
            {
                loadedWorld.Update(deltaTime);
            }

            if (pendingWorld != null)
            {
                UnloadWorld();
                LoadPendingWorld();
                return;
            }
        }

        public void SendRenderEvent(ref Microsoft.Xna.Framework.GraphicsDeviceManager gdm, ref SpriteBatch sb)
        {
            loadedWorld.Render(ref gdm, ref sb);
        }
    }
}
