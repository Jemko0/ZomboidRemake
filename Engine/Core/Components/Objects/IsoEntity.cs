using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Iso.Engine.Core.Components.IsoObjectComponents;
using Iso.Engine.Core.Interfaces;
using SharpDX.X3DAudio;

namespace Iso.Engine.Core.Components
{
    public class IsoEntity : IIsoUpdateable
    {
        public int objectID
        {
            protected get => objectID;
            set
            {
                if(objectID == -1)
                {
                    objectID = value;
                }
                else
                {
                    throw new Exception("objectID cannot be set after initialization!");
                }
            }
        }
        private IsoGuid guid;
        private List<IsoObjectComponent> components = new List<IsoObjectComponent>();

        public IsoEntity(int oID)
        {
            objectID = oID;
            guid = IsoGuidProvider.Instance.GenerateGUIDFromSeed(objectID);
            InitWithDefaults();
        }

        public string GetEntityName()
        {
            return string.Format("{0}_{1}", ToString(), guid.ToString());
        }

        protected virtual void InitWithDefaults()
        {
            Init();
        }

        public virtual void Init()
        {
            
        }

        public IsoGuid GetGUID()
        {
            return guid;
        }

        public int GetObjectID()
        {
            return objectID;
        }

        public List<IsoObjectComponent> GetComponents()
        {
            return components;
        }

        public T GetComponentByClass<T>() where T : IsoObjectComponent
        {
            return components.OfType<T>().FirstOrDefault();
        }

        public void AddComponent<T>() where T : IsoObjectComponent
        {
            T comp = new IsoObjectComponent(this) as T;
            components.Add(comp);
        }

        public virtual void Update(double deltaTime)
        {
        }
    }
}