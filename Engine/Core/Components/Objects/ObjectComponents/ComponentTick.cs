using System;

namespace Iso.Engine.Core.Components.IsoObjectComponents
{
    public class IsoComponentTick
    {
        public IsoComponentTick(Action<double> thisTickFunction) 
        {
            tickFunction = thisTickFunction;
        }

        public bool canEverTick = true;
        public bool runtimeTickEnabled = true;
        public Action<double> tickFunction = null;

        public void TickComponent(double delta)
        {
            if(runtimeTickEnabled)
            {
                tickFunction(delta);
            }
        }
    }
}