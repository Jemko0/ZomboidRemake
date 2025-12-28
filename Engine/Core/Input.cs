using Iso.Engine.Core.Interfaces;
using Iso.Engine.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Iso.Engine.Core
{
    public class MouseEventArgs
    {
        /// <summary>
        /// on mouse moved, this will be how much the mouse moved
        /// </summary>
        public Vector2 delta;

        public float wheelDelta;

        /// <summary>
        /// The mouse position relative to the viewport
        /// </summary>
        public Vector2 position;

        public MouseEventArgs(Vector2 delta, Vector2 position, float wheelDelta = 0.0f)
        {
            this.delta = delta;
            this.position = position;
            this.wheelDelta = wheelDelta;
        }
    }

    public class Input : IIsoUpdateable
    {
        public Input() { }

        public delegate void MouseMoveDelegate(MouseEventArgs e);
        public delegate void MouseClickDelegate(MouseEventArgs e);
        public delegate void MouseWheelDelegate(MouseEventArgs e);

        public event MouseMoveDelegate onMouseMove;
        public event MouseWheelDelegate onMouseWheel;

        public event MouseClickDelegate onMouseLeftClick;
        public event MouseClickDelegate onMouseRightClick;
        public event MouseClickDelegate onMouseMiddleClick;

        public event MouseClickDelegate onMouseLeftRelease;
        public event MouseClickDelegate onMouseRightRelease;
        public event MouseClickDelegate onMouseMiddleRelease;

        private Vector2 mousePositionLastFrame;
        private float mouseWheelLastFrame;

        private ButtonState lmbLastFrame = Mouse.GetState().LeftButton;
        private ButtonState mmbLastFrame = Mouse.GetState().MiddleButton;
        private ButtonState rmbLastFrame = Mouse.GetState().RightButton;

        public void Update(double deltaTime)
        {
            UpdateMouse();
            UpdateMappings();
        }

        public void UpdateMappings()
        {
            Mappings.Update();
        }

        public static Point GetRawMousePosition()
        {
            return Mouse.GetState().Position;
        }

        public static Point GetLogicalMousePosition()
        {
            return UIRenderer.GetLogicalMouse(Mouse.GetState().Position);
        }

        public void UpdateMouse()
        {
            Vector2 mousePositionThisFrame = Mouse.GetState().Position.ToVector2();
            float mouseWheelThisFrame = Mouse.GetState().ScrollWheelValue;

            ButtonState lmbThisFrame = Mouse.GetState().LeftButton;
            ButtonState mmbThisFrame = Mouse.GetState().MiddleButton;
            ButtonState rmbThisFrame = Mouse.GetState().RightButton;

            if (mousePositionLastFrame != mousePositionThisFrame)
            {
                Vector2 delta = mousePositionLastFrame - mousePositionThisFrame;
                onMouseMove?.Invoke(new MouseEventArgs(delta, mousePositionThisFrame));

                mousePositionLastFrame = mousePositionThisFrame;
            }

            if (mouseWheelLastFrame != mouseWheelThisFrame)
            {
                onMouseWheel?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, mouseWheelLastFrame - mouseWheelThisFrame));

                mouseWheelLastFrame = mouseWheelThisFrame;
            }

            if (lmbLastFrame != lmbThisFrame)
            {
                if (lmbThisFrame == ButtonState.Pressed)
                {
                    onMouseLeftClick?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }
                else
                {
                    onMouseLeftRelease?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }

                lmbLastFrame = lmbThisFrame;
            }

            if (rmbLastFrame != rmbThisFrame)
            {
                if (rmbThisFrame == ButtonState.Pressed)
                {
                    onMouseRightClick?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }
                else
                {
                    onMouseRightRelease?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }

                rmbLastFrame = rmbThisFrame;
            }

            if (mmbLastFrame != mmbThisFrame)
            {
                if (mmbThisFrame == ButtonState.Pressed)
                {
                    onMouseMiddleClick?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }
                else
                {
                    onMouseMiddleRelease?.Invoke(new MouseEventArgs(Vector2.Zero, mousePositionThisFrame, 0.0f));
                }

                mmbLastFrame = mmbThisFrame;
            }
        }
    }

    public class Mappings
    {
        public static Dictionary<string, ActionMapping> actionMappings = new Dictionary<string, ActionMapping>();
        public static Collection<Keys> invokePressedArray;
        public static Collection<Keys> invokeReleasedArray;
        public static void Update()
        {
            foreach (var mapping in actionMappings.Values)
            {
                if (Keyboard.GetState().GetPressedKeys().Contains(mapping.keyboardKey) && !invokePressedArray.Contains(mapping.keyboardKey))
                {
                    invokePressedArray.Add(mapping.keyboardKey);
                    invokeReleasedArray.Remove(mapping.keyboardKey);

                    mapping.InvokePressed(new ActionMappingArgs(mapping.keyboardKey));
                }
                if (!Keyboard.GetState().GetPressedKeys().Contains(mapping.keyboardKey) && !invokeReleasedArray.Contains(mapping.keyboardKey))
                {
                    invokePressedArray.Remove(mapping.keyboardKey);
                    invokeReleasedArray.Add(mapping.keyboardKey);

                    mapping.InvokeReleased(new ActionMappingArgs(mapping.keyboardKey));
                }
            }
        }

        public static void InitializeMappings()
        {
            invokePressedArray = new Collection<Keys>();
            invokeReleasedArray = new Collection<Keys>();

            actionMappings.Add("move_forward", new ActionMapping(Keys.W));
            actionMappings.Add("move_backward", new ActionMapping(Keys.S));
            actionMappings.Add("move_left", new ActionMapping(Keys.A));
            actionMappings.Add("move_right", new ActionMapping(Keys.D));
        }

        /// <summary>
        /// replaces a mapping that already exists, does not check for non existing ones so it will crash when u use an invalid mapping name
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="newKey"></param>
        public static void RebindMapping(string mapping, Keys newKey)
        {
            actionMappings[mapping].keyboardKey = newKey;
        }


        /// <summary>
        /// loads mappings from file
        /// </summary>
        /// <param name="file"></param>
        public static void LoadMappings(string file)
        {
            throw new NotImplementedException("lolol");
        }

        public static bool IsMappingHeld(string mapping)
        {
            return Keyboard.GetState().GetPressedKeys().Contains(actionMappings[mapping].keyboardKey);
        }
    }

    public delegate void ActionMappingPress(ActionMappingArgs e);
    public delegate void ActionMappingRelease(ActionMappingArgs e);

    public class ActionMappingArgs
    {
        public Keys key;

        public ActionMappingArgs(Keys key)
        {
            this.key = key;
        }
    }

    public class ActionMapping
    {
        public Keys keyboardKey;
        public event ActionMappingPress onActionMappingPressed;
        public event ActionMappingRelease onActionMappingReleased;
        public ActionMapping(Keys key)
        {
            keyboardKey = key;
        }

        public void InvokePressed(ActionMappingArgs args)
        {
            if (onActionMappingPressed == null)
            {
                return;
            }
            onActionMappingPressed.Invoke(args);
        }

        public void InvokeReleased(ActionMappingArgs args)
        {
            if (onActionMappingReleased == null)
            {
                return;
            }
            onActionMappingReleased.Invoke(args);
        }
    }
}
