using Iso.Engine.Core.DataStructures;

namespace Iso.Engine.Core.Components.IsoObjectComponents
{
    public class IsoTransformComponent : IsoObjectComponent
    {
        protected FTransform transform;
        public IsoTransformComponent(IsoEntity Owner) : base(Owner)
        {
        }

        public override void InitializeComponent()
        {
            transform = FTransform.Identity;
        }

        public void SetTranslation(float x, float y, float z)
        {
            transform.translation.x = x;
            transform.translation.y = y;
            transform.translation.z = z;
        }

        public void Translate(float x, float y, float z)
        {
            transform.translation.x += x;
            transform.translation.y += y;
            transform.translation.z += z;
        }

        public void SetScale(float width, float height)
        {
            transform.scale.x = width;
            transform.scale.y = height;
        }

        public FTransform GetTransform()
        {
            return transform;
        }
    }
}