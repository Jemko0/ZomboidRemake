using Iso.Engine.Core.Rendering.DataStructures;
using Iso.Engine.Core.Rendering.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Iso.Engine.Core.UI.Elements;
using Iso.Engine.Core.UI.Extensions;
using Iso.Engine.Core.Rendering;
using Iso.Engine.Core.Interfaces;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Iso.Engine.Core.UI
{
    public class UIRenderer : IIsoRenderable, IIsoUpdateable
    {
        public RenderTarget2D uiRenderTarget = null;
        public static Texture2D onePxWhite = null;
        public static Panel root = null!;

        private float frameCounter = 0;
        public float updateRate = 0.033f; //30 FPS

        public UIElement capturedElement = null;

        public UIRenderer()
        {
            root = new Panel()
                    .SetName("root")
                    .SetWidth(1920)
                    .SetHeight(1080)
                    .FillParent();

            RenderUtil.window.ClientSizeChanged += OnWindowResized;
            BindEvents();
        }

        private void BindEvents()
        {
            SceneManager.GetInputManager().onMouseLeftClick += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_LMBCLICK, e); };
            SceneManager.GetInputManager().onMouseRightClick += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_RMBCLICK, e); };
            SceneManager.GetInputManager().onMouseLeftRelease += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_LMBRELEASE, e); };
            SceneManager.GetInputManager().onMouseRightRelease += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_RMBRELEASE, e); };

            SceneManager.GetInputManager().onMouseMove += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_MOVE, e); };
            SceneManager.GetInputManager().onMouseWheel += (e) => { SendEventOnMouseLocation(UIEvent.MOUSE_WHEEL, e); };
        }

        private void SendEventOnMouseLocation(UIEvent eventName, MouseEventArgs e)
        {
            Point logicalMouse = GetLogicalMouse(e.position.ToPoint());

            UIElement target = capturedElement ?? GetElementAt(root, logicalMouse);

            if (target == null) return;

            if (eventName == UIEvent.MOUSE_LMBCLICK) capturedElement = target;
            if (eventName == UIEvent.MOUSE_LMBRELEASE) capturedElement = null;

            Dictionary<string, object?> data = new Dictionary<string, object?>();
            data.Add("mouseArguments", e);

            target.BubbleEvent(eventName, data);
        }

        public static Point GetLogicalMouse(Point physicalPos)
        {
            float dpi = RenderUtil.GetDPIScale();
            return new Point((int)(physicalPos.X / dpi), (int)(physicalPos.Y / dpi));
        }

        private UIElement GetElementAt(UIElement parent, Point mousePos)
        {
            if (parent == null) return null;

            if (parent.visibility == UIVisibilityMode.HIDDEN || parent.visibility == UIVisibilityMode.COLLAPSED) return null;

            if (parent.clipChildren && !parent.GetAbsolouteBounds().Contains(mousePos))
            {
                return null;
            }

            var children = parent.GetChildren();
            if (children != null)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    var hit = GetElementAt(children[i], mousePos);
                    if (hit != null) return hit;
                }
            }

            if (parent.visibility != UIVisibilityMode.VISIBLE) return null;

            if (parent.GetAbsolouteBounds().Contains(mousePos))
            {
                return parent;
            }

            return null;
        }

        private void OnWindowResized(object sender, System.EventArgs e)
        {
            int newWidth = RenderUtil.window.ClientBounds.Width;
            int newHeight = RenderUtil.window.ClientBounds.Height;

            newWidth = (int)(newWidth / RenderUtil.GetDPIScale());
            newHeight = (int)(newHeight / RenderUtil.GetDPIScale());

            if (uiRenderTarget.Width != newWidth || uiRenderTarget.Height != newHeight)
            {
                uiRenderTarget.Dispose();
                uiRenderTarget = new RenderTarget2D(IsoGame.graphics.GraphicsDevice, newWidth, newHeight);
            }
        }

        private static Queue<UIElement> pendingDestroyElements = new Queue<UIElement>();

        public static void MarkForDeletion(UIElement element)
        {
            pendingDestroyElements.Enqueue(element);
        }

        public static void ProcessDestruction()
        {
            while (pendingDestroyElements.Count > 0)
            {
                var element = pendingDestroyElements.Dequeue();

                element.OnDestroy();

                if (element.parent != null)
                {
                    if (element.parent is Panel panel)
                    {
                        panel.GetChildren().Remove(element);
                    }
                    else if (element.parent is SingleChildElement sce)
                    {
                        sce.SetChild(null);
                    }
                }

                element.parent = null;
            }
        }

        public void Add(params UIElement[] newElements)
        {
            if (newElements == null) return;
            if (newElements.Length == 0) return;

            for(int i  = 0; i < newElements.Length; i++)
            {
                root.Attach(newElements[i]);
            }
        }

        public static RasterizerState rasterizerState = new RasterizerState()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public void Render(ref GraphicsDeviceManager gdm, ref SpriteBatch sb, ref IsoRenderContext renderContext)
        {
            if(frameCounter > updateRate)
            {
                frameCounter = 0;
            }
            else
            {
                return;
            }

            var device = gdm.GraphicsDevice;

            device.SetRenderTarget(uiRenderTarget);
            device.Clear(Color.Transparent);


            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rasterizerState);

            Rectangle screenSpace = new Rectangle(0, 0, uiRenderTarget.Width, uiRenderTarget.Height);
            root.Render(ref gdm, ref sb, ref renderContext, screenSpace);

            sb.End();

            device.SetRenderTarget(null);
        }

        public void DrawToScreen(SpriteBatch sb, GraphicsDevice device)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(uiRenderTarget, device.Viewport.Bounds, Color.White);
            sb.End();
        }

        public void Update(double deltaTime)
        {
            frameCounter += (float)deltaTime;
            root.Update(deltaTime);

            ProcessDestruction();
        }
    }
}
