using Iso.Engine.Core.DataStructures;
using Microsoft.Xna.Framework;
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

        public Matrix viewMatrix;
        public IsoCamera(FVector3 position, float zoom)
        {
            this.position = position;
            this.zoom = zoom;

            UpdateViewMatrix();
        }

        public IsoCamera()
        {
            position = new FVector3(0.0f, 0.0f, 0.0f);
            zoom = 1.0f;

            UpdateViewMatrix();
        }

        public void SetPosition(FVector3 position)
        {
            this.position = position;
            UpdateViewMatrix();
        }

        public void UpdateViewMatrix()
        {
            viewMatrix = Matrix.CreateTranslation(-position.x, -position.y, -position.z);
        }

        public void Translate(FVector3 delta)
        {
            this.position.x += delta.x;
            this.position.y += delta.y;
            this.position.z += delta.z;

            UpdateViewMatrix();
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
