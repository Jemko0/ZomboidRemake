namespace Iso.Engine.Core.Components.IsoObjectComponents
{
    public class IsoObjectComponent
    {
        IsoComponentTick tickComponent = null!;
        public IsoEntity Owner = null;

        public IsoObjectComponent(IsoEntity Owner)
        {
            this.Owner = Owner;
            tickComponent.tickFunction = UpdateComponent;

            IsoEntityManager.MakeNewTickComponent(ref tickComponent, ref tickComponent.tickFunction);
        }

        public virtual void InitializeComponent()
        {
            
        }

        public virtual void UpdateComponent(float delta)
        {
            
        }
    }
}