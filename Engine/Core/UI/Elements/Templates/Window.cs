using Iso.Engine.Core.Assets;
using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Logging;
using Iso.Engine.Core.UI.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iso.Engine.Core.UI.Elements.Templates
{
    public class Window : Panel
    {
        public Window Create()
        {
            this.Attach(
                new Wrapper()
                    .SetName("Window Main")
                    .CenterInParent(500, 500)
                    .SetBackground
                    (
                        new Image()
                            .SetName("Window Background")
                            .FillParent()
                            .SetDrawMode(ImageDrawMode.Box)
                            .SetSliceSize(8)
                            .SetBrush(AssetHelper.globalContentManager.Load<Texture2D>("ui/ui_panel_background"))
                    )
                    .SetInnerSlotPadding(new IntVector2(5, 5))
                    .SetChild
                    (
                        new VStack()
                            .SetName("Window VStack")
                            .FillParent()
                            .Attach
                            (
                                new WindowTitleBar()
                                    .SetName("Title Bar")
                                    .FillWidth(0)
                                    .SetHeight(24)
                                    .SetBackground(
                                        new Image()
                                            .SetBrush(UIRenderer.onePxWhite)
                                            .SetTint(Color.Red)
                                            .FillParent()
                                    )
                            )
                            .Attach
                            (
                                new VStack()
                                    .SetName("Window Content")
                                    .FillParent()
                            )
                    )
                );

            return this;
        }
    }
}
