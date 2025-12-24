using Iso.Engine.Core.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Rendering
{
    public class IsoCamera
    {
        protected FVector3 position = new FVector3(0.0f, 0.0f, 0.0f);
        protected float zoom = 1.0f;

        public IsoCamera(FVector3 position, float zoom)
        {
            this.position = position;
            this.zoom = zoom;
        }

        public IsoCamera()
        {
            position = new FVector3(0.0f, 0.0f, 0.0f);
            zoom = 1.0f;
        }

        public void SetPosition(FVector3 position)
        {
            this.position = position;
        }

        public void Translate(FVector3 delta)
        {
            this.position.x += delta.x;
            this.position.y += delta.y;
            this.position.z += delta.z;
        }

        public FVector3 GetPosition()
        {
            return position;
        }

        public void SetZoom(float newZoom)
        {
            zoom = newZoom;
        }

        public float GetZoom()
        {
            return zoom;
        }
    }
}
