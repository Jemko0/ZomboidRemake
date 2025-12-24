using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core
{
    internal class SceneManager
    {
        public static World loadedWorld;

        public static void LoadWorld<T>() where T : World
        {
            T newWorld = Activator.CreateInstance<T>();
            newWorld.Init();

            loadedWorld = newWorld;
        }

        public static void UnloadWorld()
        {
            loadedWorld.Unload();
            loadedWorld = null;
        }
    }
}
