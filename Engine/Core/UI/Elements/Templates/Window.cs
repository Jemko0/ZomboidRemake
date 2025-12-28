using Iso.Engine.Core.Assets;
using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.UI.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements.Templates
{
    public class Window : Panel
    {
        public bool allowResizing = false;
        public Window AllowResize()
        {
            allowResizing = true;
            return this;
        }

        public Window Create()
        {
            this.Attach(
                new Wrapper()
                    .SetName("Window Main")
                    .FillParent(0)
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
                            .FillParent(0)
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
                                        .SetChild
                                        (
                                            new HStack()
                                                .FillParent()
                                                .Attach
                                                (
                                                    new Spacer().FillParent(0)
                                                )
                                                .Attach
                                                (
                                                    new Button()
                                                        .SetName("Window Close Button")
                                                        .SetSize(50, 25)
                                                        .SetChild
                                                        (
                                                            new TextBlock()
                                                                .FillParent()
                                                                .SetText("X")
                                                                .SetTint(Color.Black)
                                                                .SetFontScale(0.75f)
                                                                .SetFont(Fonts.monospace)
                                                                .SetVerticalAlignment(VerticalAlignmentMode.CENTER)
                                                                .SetJustification(TextJustificationMode.CENTER)
                                                        )
                                                        .OnReleased(() =>
                                                        {
                                                            this.Destroy();
                                                        })
                                                )
                                        )
                            )
                            .Attach
                            (
                                new ScrollBox()
                                    .SetName("Window Content")
                                    .FillParent(0)
                                    .SetChild
                                    (
                                        new VStack()
                                            .FillWidth()
                                            .Attach
                                            (
                                                new Button()
                                                    .FillWidth()
                                                    .SetHeight(24)
                                            )
                                            .Attach
                                            (
                                                new Button()
                                                    .FillWidth()
                                                    .SetHeight(2400)
                                            )
                                    )
                            )
                            .Attach
                            (
                                new Button()
                                    .SetAnchor(new Vector2(1.0f, 1.0f), new Vector2(1.0f, 1.0f))
                                    .OnClick(() => { isBeingResized = true; })
                                    .OnReleased(() => { isBeingResized = false; })
                            )
                    )
                );
            return this;
        }

        public bool isBeingDragged = false;
        public bool isBeingResized = false;
        private Point lastMousePos;
        protected override bool OnEventReceived(UIEvent eventName, Dictionary<string, object> data)
        {
            Point currentMouse = Point.Zero;

            if (data != null)
            {
                MouseEventArgs e = data["mouseArguments"] as MouseEventArgs;
                if (e != null)
                {
                    currentMouse = UIRenderer.GetLogicalMouse(e.position.ToPoint());
                }
            }

            if (eventName == UIEvent.DRAG_START)
            {
                isBeingDragged = true;
                lastMousePos = currentMouse;
                return true;
            }

            if(eventName == UIEvent.DRAG_STOP)
            {
                isBeingDragged = false;
                return true;
            }

            if (eventName == UIEvent.MOUSE_MOVE && isBeingDragged)
            {
                float dpi = RenderUtil.GetDPIScale();

                int deltaX = (int)((currentMouse.X - lastMousePos.X));
                int deltaY = (int)((currentMouse.Y - lastMousePos.Y));

                bounds.X += deltaX;
                bounds.Y += deltaY;

                lastMousePos = currentMouse;
                return true;
            }

            return false;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            isBeingDragged = false;
        }
    }
}
