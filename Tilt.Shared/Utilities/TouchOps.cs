using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Utilities
{
    public static class TouchOps
    {
        private static TouchCollection mTouchCollection;
        private static GestureCollection mGestureCollection;

        public static void Initialize()
        {
            mTouchCollection = new TouchCollection();
            mGestureCollection = new GestureCollection();
        }

        public static void Update()
        {
            mGestureCollection = new GestureCollection();
            mTouchCollection = TouchPanel.GetState();

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                mGestureCollection.Add(gesture);
            }

        }

        public static GestureCollection GestureCollection
        {
            get { return mGestureCollection; }
        }

        public static TouchCollection TouchCollection
        {
            get { return mTouchCollection; }
        }

        public static Vector2 GetPosition()
        {
            return mGestureCollection.FirstOrDefault(g => g.Position != Vector2.Zero).Position;
        }

        public static void ClearTouch()
        {
            mGestureCollection.Clear();
        }

        public static bool ContainsPoint(Rectangle bounds)
        {
            return mGestureCollection.Any(gesture => bounds.Contains(gesture.Position));
        }

        public static bool IsFlick()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.Flick);
        }

        public static bool IsTap()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.Tap);
        }

        public static bool IsDoubleTap()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.DoubleTap);
        }

        public static bool IsHold()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.Hold);
        }

        public static bool IsPinch()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.Pinch);
        }

        public static bool IsPinchComplete()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.PinchComplete);
        }

        public static bool IsFreeDrag()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.FreeDrag);
        }

        public static bool IsHorizontalDrag()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.HorizontalDrag);
        }

        public static bool IsVerticalDrag()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.VerticalDrag);
        }

        public static bool IsDragComplete()
        {
            return mGestureCollection.Any(g => g.GestureType == GestureType.DragComplete);
        }
    }

    public class GestureCollection : Collection<GestureSample>
    {
    }
}
