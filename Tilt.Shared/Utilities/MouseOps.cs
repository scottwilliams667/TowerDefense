using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Utilities
{
    public static class MouseOps
    {
        private static MouseState mPrevMouseState;
        private static MouseState mMouseState;


        public static void Update()
        {
            mPrevMouseState = mMouseState;
            mMouseState = Mouse.GetState();

        }


        public static bool ContainsPoint(Rectangle bounds)
        {
            return bounds.Contains(mMouseState.Position);
        }

        public static Vector2 GetPosition()
        {
            return GeometryOps.PointToVector2(mMouseState.Position);
        }

        public static void SetPosition(Vector2 position)
        {
            Point point = GeometryOps.Vector2ToPoint(position);
            Mouse.SetPosition(point.X, point.Y);
        }

        public static int GetScrollWheelValue()
        {
            return mMouseState.ScrollWheelValue;    
        }

        public static int GetScrollWheelDelta()
        {
            return mMouseState.ScrollWheelValue - mPrevMouseState.ScrollWheelValue;
        }

        public static bool IsMouseScrolling()
        {
            return mMouseState.ScrollWheelValue != mPrevMouseState.ScrollWheelValue;
        }

        public static bool IsClick()
        {
            return mMouseState.LeftButton == ButtonState.Released && mPrevMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsDrag()
        {
            return mMouseState.LeftButton == ButtonState.Pressed && mPrevMouseState.LeftButton == ButtonState.Pressed;
        }
    }
}
