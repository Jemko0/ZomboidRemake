using Iso.Engine.Core.Assets;
using Iso.Engine.Core.DataStructures;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.UI.Extensions;
using Iso.Engine.Core.UI.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements.Templates
{
    public class Window : Panel, IIsoUI
    {
        public bool allowResizing = false;
        public Window AllowResize()
        {
            allowResizing = true;
            return this;
        }

        public void OnNameSet(string newName)
        {
            TextBlock titleText = GetChildByName<TextBlock>("Title Text Block");
            titleText.SetText(newName);
        }

        public Window()
        {
            this.Attach(
                new Wrapper()
                    .SetName("Window Main")
                    .FillParent()
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
                                        .FillWidth()
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
                                                    new TextBlock()
                                                        .SetName("Title Text Block")
                                                        .SetText(name)
                                                        .FillParent()
                                                )
                                                .Attach
                                                (
                                                    new Spacer().FillParent()
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
                                                            Destroy();
                                                        })
                                                )
                                        )
                            )
                            .Attach
                            (
                                new ScrollBox()
                                    .SetName("Window Content")
                                    .FillParent()
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
                                    )
                            )
                            .Attach
                            (
                                new Button()
                                    .SetName("Resize Button")
                                    .SetVisibility(UIVisibilityMode.HIDDEN)
                                    .SetCursor(MouseCursor.SizeNWSE)
                                    .SetAnchor(new Vector2(1.0f, 1.0f), new Vector2(1.0f, 1.0f))
                                    .SetSize(16, 16)
                                    .OnClick(() => { isBeingResized = true; })
                                    .OnReleased(() => { isBeingResized = false; })
                            )
                    )
                );
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

            if (eventName == UIEvent.DRAG_STOP)
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

        public override void UIUpdate(double deltaTime)
        {
            if (isBeingResized)
            {
                Point mousePos = Input.GetLogicalMousePosition();

                bounds.Width = mousePos.X - bounds.X;
                bounds.Height = mousePos.Y - bounds.Y;
            }

            base.UIUpdate(deltaTime);
        }
    }
}
