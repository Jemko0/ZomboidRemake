using System;
using System.Collections.Generic;
using Iso.Engine.Core.Components.IsoObjectComponents;
using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Iso.Engine.Core.Components
{
    public class IsoEntityManager : IIsoUpdateable, IIsoRenderable
    {
        private IsoEntityManager() {}
        private static IsoEntityManager _instance;
        public static IsoEntityManager Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new IsoEntityManager();
                }
                return _instance;
            }
        }

        public static List<IsoComponentTick> registeredTickComponents;
        public static Dictionary<int, IsoEntity> entities = new Dictionary<int, IsoEntity>();
        private static int nextObjID = 0;
        public static void MakeNewTickComponent(ref IsoComponentTick tickComponent, ref Action<double> tickFunction)
        {
            tickComponent = new IsoComponentTick(tickFunction);
            registeredTickComponents.Add(tickComponent);
        }

        public void UpdateManager(double delta)
        {
            foreach(var component in registeredTickComponents)
            {
                component.TickComponent(delta);
            }

            for(int i = 0; i < entities.Count; i++)
            {
                IsoEntity ent = entities[i];
                ent.Update(delta);
            }
        }

        public IsoEntity CreateObject<T>() where T : IsoEntity
        {
            //doo
            T instance = Activator.CreateInstance<T>();
            instance.objectID = nextObjID;

            IsoLog.Logf("Spawned ENT: {0}", instance.GetEntityName());
            
            nextObjID++;

            return instance;
        }

        public void Reset()
        {
            entities.Clear();
        }

        public void Update(double deltaTime)
        {
            UpdateManager(deltaTime);
        }

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            for(int i = 0; i < entities.Count; i++)
            {
                IsoEntity ent = entities[i];
            }
        }
    }
}