using System;

namespace Iso.Engine.Core.Components.IsoObjectComponents
{
    public class IsoComponentTick
    {
        public IsoComponentTick(Action<float> thisTickFunction) 
        {
            tickFunction = thisTickFunction;
        }

        public bool canEverTick = true;
        public bool runtimeTickEnabled = true;
        public Action<float> tickFunction = null;

        public void TickComponent(float delta)
        {
            if(runtimeTickEnabled)
            {
                tickFunction(delta);
            }
        }
    }
}