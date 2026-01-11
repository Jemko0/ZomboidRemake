using System.Collections.Generic;
using Iso.Engine.Core.Components.IsoObjectComponents;
using Iso.Engine.Core.Interfaces;

namespace Iso.Engine.Core.Components
{
    public class IsoObject : IIsoUpdateable
    {
        private int objectID = 0;
        private IsoGuid guid;
        private List<IsoObjectComponent> components;

        public IsoObject(int oID)
        {
            objectID = oID;
            guid = IsoGuidProvider.Instance.GenerateGUIDFromSeed(objectID);
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