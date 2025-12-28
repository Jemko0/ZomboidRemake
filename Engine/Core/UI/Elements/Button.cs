using Iso.Engine.Core.Assets;
using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.UI.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI.Elements
{
    public class Button : SingleChildElement
    {
        public event Action onButtonClicked;
        public event Action onButtonReleased;

        public Image buttonImage = new Image()
                                        .FillParent()
                                        .SetBrush(AssetHelper.globalContentManager.Load<Texture2D>("ui/ui_button_default"))
                                        .SetDrawMode(ImageDrawMode.Box)
                                        .SetSliceSize(8);

        public Image buttonImageHovered = new Image()
                                        .FillParent()
                                        .SetBrush(AssetHelper.globalContentManager.Load<Texture2D>("ui/ui_button_default_hovered"))
                                        .SetDrawMode(ImageDrawMode.Box)
                                        .SetSliceSize(8);

        public Image buttonImagePressed = new Image()
                                        .FillParent()
                                        .SetBrush(AssetHelper.globalContentManager.Load<Texture2D>("ui/ui_button_default_pressed"))
                                        .SetDrawMode(ImageDrawMode.Box)
                                        .SetSliceSize(8);

        public Image currentImage = null;

        private bool allowRightClick = false;
        public bool isPressed = false;

        public Button AllowRightClick()
        {
            allowRightClick = true;
            return this;
        }

        public Button OnClick(Action action)
        {
            onButtonClicked += action;
            return this;
        }

        public Button OnReleased(Action action)
        {
            onButtonReleased += action;
            return this;
        }

        private void OnButtonClicked()
        {
            isPressed = true;
            onButtonClicked?.Invoke();
        }

        private void OnButtonReleased()
        {
            isPressed = false;
            onButtonReleased?.Invoke();
        }

        public override void OnDestroy()
        {
            buttonImage.SetBrush(null);
            buttonImageHovered.SetBrush(null);
            buttonImagePressed.SetBrush(null);

            onButtonClicked = null;
            onButtonReleased = null;
        }

        protected override bool OnEventReceived(UIEvent eventName, Dictionary<string, object> data)
        {
            if(eventName == UIEvent.MOUSE_LMBCLICK || (eventName == UIEvent.MOUSE_RMBCLICK && allowRightClick))
            {
                OnButtonClicked();
                return true;
            }

            if(eventName == UIEvent.MOUSE_LMBRELEASE || (eventName == UIEvent.MOUSE_RMBRELEASE && allowRightClick))
            {
                OnButtonReleased();
                return true;
            }

            return false;
        }

        public override void UIUpdate(double deltaTime)
        {
            currentImage = buttonImage;

            if(IsHovered())
            {
                currentImage = buttonImageHovered;
            }

            if (IsPressed())
            {
                currentImage = buttonImagePressed;
            }

            base.UIUpdate(deltaTime);
        }
        protected override void UIRender(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext, Rectangle actualRect)
        {
            if (currentImage == null)
            {
                return;
            }

            currentImage.Render(ref gdm, ref sb, ref renderContext, actualRect);
            base.UIRender(ref gdm, ref sb, ref renderContext, actualRect);
        }
    }
}
