using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Iso.Engine.Core.Rendering;

namespace Iso.Engine.Core.Rendering.DataStructures
{
    public class IsoRenderContext
    {
        public IsoCamera? camera;
        public Dictionary<string, object?> extras { get; } = new();
    }

    public class IsoRenderContextBuilder
    {
        IsoRenderContext renderContext;

        public IsoRenderContextBuilder()
        {
            renderContext = new IsoRenderContext();
        }

        public IsoRenderContextBuilder WithCamera(IsoCamera isoCam)
        {
            renderContext.camera = isoCam;
            return this;
        }

        public IsoRenderContextBuilder WithExtra(Dictionary<string, object?> extra)
        {
            foreach (var kvp in extra)
                renderContext.extras[kvp.Key] = kvp.Value;

            return this;
        }

        public IsoRenderContext Build()
        {
            return renderContext;
        }

    }
}
