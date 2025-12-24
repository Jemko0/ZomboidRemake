using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.Interfaces
{
    internal interface IUpdateable
    {
        void Update(double deltaTime);
    }
}
