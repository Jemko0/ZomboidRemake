using System;
using System.Collections.Generic;
using Iso.Engine.Core.Components.IsoObjectComponents;

namespace Iso.Engine.Core.Components
{
    public class IsoObjectManager
    {
        private IsoObjectManager() {}
        private static IsoObjectManager _instance;
        public static IsoObjectManager Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new IsoObjectManager();
                }
                return _instance;
            }
        }

        public static List<IsoComponentTick> registeredTickComponents;
        public static Dictionary<int, IsoObject> objects;
        private static int nextObjID = 0;
        public static void MakeNewTickComponent(ref IsoComponentTick tickComponent, ref Action<float> tickFunction)
        {
            tickComponent = new IsoComponentTick(tickFunction);
            registeredTickComponents.Add(tickComponent);
        }

        public void UpdateObjectManager(float delta)
        {
            foreach(var component in registeredTickComponents)
            {
                component.TickComponent(delta);
            }
        }

        public void CreateObject<T>() where T : IsoObject
        {
            //doo
            nextObjID++;
        }

        public void Reset()
        {
            objects.Clear();
        }
    }
}