namespace Iso.Engine.Core.Components.IsoObjectComponents
{
    public class IsoObjectComponent
    {
        IsoComponentTick tickComponent = null!;
        public IsoObject Owner = null;

        public IsoObjectComponent(IsoObject Owner)
        {
            this.Owner = Owner;
            tickComponent.tickFunction = UpdateComponent;

            IsoObjectManager.MakeNewTickComponent(ref tickComponent, ref tickComponent.tickFunction);
        }

        public virtual void InitializeComponent()
        {
            
        }

        public virtual void UpdateComponent(float delta)
        {
            
        }
    }
}