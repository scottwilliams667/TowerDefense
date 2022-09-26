using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class CameraPositionComponent : PositionComponent
    {
        private Viewport mViewport;
        private Matrix mMatrix;
        private float mZoom;
        private bool mZoomLocked;

        private Camera2D mCamera;

        private const int kSpeed = 1150;

        private const float kFlickSpeedScale = 0.5f;
        private const float kKineticSpeedScale = 0.95f;
        private const int kDragSpeed = 120;
        private const float kMaxZoom = 3.0f;
        private const float kMinZoom = 1.0f;
        private Vector2 mDelta;
        private bool mIsFlick;
        private bool mIsZoom;

        private bool mShake;
        private Vector2 mShakeDirection = Vector2.Zero;
        private int mShakeSpeed = 150;
        private const int kShakeSpeed = 150;
        private const float kShakeScale = 0.7f;
        private float mShakeDuration = 0.1f;
        private const float kInterval = 0.1f;
        private int mSign = 1;


        public CameraPositionComponent(int x, int y, Viewport viewport, Entity owner) : base(x, y, owner, Vector2.Zero)
        {
            mZoom = 1.5f;
            mViewport = viewport;
            mPosition = Vector2.Zero;
            mMatrix = Matrix.CreateTranslation(0,0,0);

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            GameWindow gameWindow = ServiceLocator.GetService<GameWindow>();
            

            BoxingViewportAdapter adapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, viewport.Width, viewport.Height);

            mCamera = new Camera2D(adapter);
            mCamera.Position = new Vector2(0,0);

            mCamera.Origin = new Vector2(viewport.Width / 2, viewport.Height / 2);


        }

        public Matrix Matrix
        {
            get { return mMatrix; }
            set { mMatrix = value; }
        }

        public float Zoom
        {
            get { return mZoom; }
            set { mZoom = value; }
        }

        public bool ZoomLocked
        {
            get { return mZoomLocked; }
            set { mZoomLocked = value; }
        }

        public Viewport Viewport
        {
            get { return mViewport; }
        }

        public new Vector2 Position
        {
            get { return mCamera.Position; }
        }

        public bool Shake
        {
            get { return mShake; }
            set
            {
                mShake = value;

                if (value)
                {
                    mShakeDirection = Vector2.Zero;
                    mShakeDuration = kInterval;
                    mShakeSpeed = kShakeSpeed;
                    mSign = 1;
                }
            }
        }

        public override void Update()
        {
#if WIN32
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                mCamera.Move(new Vector2(0, -1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                mCamera.Move(new Vector2(0, 1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                mCamera.Move( new Vector2(-1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                mCamera.Move(new Vector2(1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                mCamera.ZoomIn(0.05f);
            }
            if (keyboardState.IsKeyDown(Keys.X))
            {
                mCamera.ZoomOut(0.05f);
            }

            if (mCamera.Zoom > 3.0f) mCamera.Zoom = 3.0f;
            if (mCamera.Zoom < 1.0f) mCamera.Zoom = 1.0f;

            Layer layer = LayerManager.GetLayerOfEntity(Owner);

            if(mCamera.BoundingRectangle.X < 0.0f)
            {
                mCamera.Move(new Vector2(-mCamera.BoundingRectangle.X, 0));
            }

            if(mCamera.BoundingRectangle.Y < 0.0f)
            {
                mCamera.Move(new Vector2(0, -mCamera.BoundingRectangle.Y));
            }

            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            SlidingPanel slidingPanel = null;
            if(towerSelectLayer != null)
            {
                slidingPanel = towerSelectLayer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            }

            if(slidingPanel != null && (slidingPanel.PositionComponent as SlidingPanelPositionComponent).IsOpen)
            {
                SlidingPanelPositionComponent slidingPanelPositionComponent = slidingPanel.PositionComponent as SlidingPanelPositionComponent;

                if(mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4) + Viewport.Width / 3)
                {
                    mCamera.Move(new Vector2(Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4) - mCamera.BoundingRectangle.Width - mCamera.BoundingRectangle.X + Viewport.Width / 3 , 0));
                }
            }
            else
            {
                if(mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4))
                {
                    mCamera.Move(new Vector2(Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4) - mCamera.BoundingRectangle.Width - mCamera.BoundingRectangle.X, 0));
                }

                if (mCamera.BoundingRectangle.Y + mCamera.BoundingRectangle.Height > Viewport.Height + Math.Max(TileMap.Height / 2, Viewport.Height / 4))
                {
                    mCamera.Move(new Vector2(0, Viewport.Height + Math.Max(TileMap.Height / 2, Viewport.Height / 4) - mCamera.BoundingRectangle.Height - mCamera.BoundingRectangle.Y));
                }
            }

            layer.Matrix = mCamera.GetViewMatrix();
#else


            if (LayerManager.Layer.Type == LayerType.GameMenuOverlay)
                return;



            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            Camera camera = Owner as Camera;
            CameraPositionComponent positionComponent = camera.PositionComponent;

            TouchCollection touchCollection = TouchOps.TouchCollection;
            GestureCollection gestureCollection = TouchOps.GestureCollection;

            if (gestureCollection == null)
                return;

            //handle all drag/flick gestures and move the camera
            foreach (TouchLocation location in touchCollection)
            {
                if (location.State == TouchLocationState.Pressed &&
                    mIsFlick &&
                    mDelta != Vector2.Zero)
                {
                    mDelta = Vector2.Zero;
                    mIsFlick = false;
                    mIsZoom = false;
                }
            }

            foreach (GestureSample gesture in gestureCollection)
            {
                switch (gesture.GestureType)
                {
                    case GestureType.Flick:
                        {
                            mDelta = -gesture.Delta * kFlickSpeedScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            //positionComponent.Position += mDelta;
                            mCamera.Move(mDelta);
                            mIsFlick = true;
                            mIsZoom = false;
                        }
                        break;
                    case GestureType.HorizontalDrag:
                        {
                            Vector2 position2 = gesture.Position;
                            Vector2 oldPosition = gesture.Position - gesture.Delta;
                            Vector2 delta = position2 - oldPosition;
                            mDelta = new Vector2(delta.X, 0) * kDragSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            mDelta = -mDelta;

                            mCamera.Move(mDelta);
                            mIsFlick = false;
                            mIsZoom = false;
                        }
                        break;
                    case GestureType.VerticalDrag:
                        {
                            Vector2 position = gesture.Position;
                            Vector2 oldPosition = gesture.Position - gesture.Delta;
                            Vector2 delta = position - oldPosition;
                            mDelta = new Vector2(0, delta.Y) * kDragSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            mDelta = -mDelta;
                            mCamera.Move(mDelta);
                            mIsFlick = false;
                            mIsZoom = false;
                        }
                        break;
                    case GestureType.FreeDrag:
                        {
                            mDelta = -gesture.Delta;
                            mCamera.Move( mDelta * kDragSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            mIsFlick = false;
                            mIsZoom = false;
                        }
                        break;
                    case GestureType.Tap:
                    case GestureType.Hold:
                        {
                            mDelta = Vector2.Zero;
                            mIsFlick = false;
                            mIsZoom = false;
                        }
                        break;
                    case GestureType.Pinch:
                        {
                            if (positionComponent.ZoomLocked)
                                break;

                            Vector2 oldPosition = gesture.Position - gesture.Delta;
                            Vector2 oldPosition2 = gesture.Position2 - gesture.Delta2;
                            float distance = Vector2.Distance(gesture.Position, gesture.Position2);
                            float oldDistance = Vector2.Distance(oldPosition, oldPosition2);
                            float zoom;
                            if (distance.Equals(0) || oldDistance.Equals(0))
                                zoom = kMinZoom;
                            else
                                zoom = distance / oldDistance;
                            //positionComponent.Zoom *= zoom;

                            mCamera.Zoom *= zoom;

                            if (mCamera.Zoom > 3.0f) mCamera.Zoom = 3.0f;
                            if (mCamera.Zoom < 1.0f) mCamera.Zoom = 1.0f;

                            if (positionComponent.Zoom > kMaxZoom) positionComponent.Zoom = kMaxZoom;
                            if (positionComponent.Zoom < kMinZoom) positionComponent.Zoom = kMinZoom;
                            mIsFlick = false;
                            mIsZoom = true;
                        }
                        break;
                }
            }

            //kinetic scrolling - gradually slow down flick
            if (mDelta != Vector2.Zero &&
                !TouchPanel.IsGestureAvailable &&
                mIsFlick)
            {
                mDelta = mDelta * kKineticSpeedScale;
                mCamera.Move(mDelta);

                if ((int)mDelta.X == 0 && (int)mDelta.Y == 0)
                {
                    mDelta = Vector2.Zero;
                    mIsFlick = false;
                }
            }

            Layer layer = LayerManager.GetLayerOfEntity(Owner);

            if (mCamera.BoundingRectangle.X < 0.0f)
            {
                mCamera.Move(new Vector2(-mCamera.BoundingRectangle.X, 0));
            }

            if (mCamera.BoundingRectangle.Y < 0.0f)
            {
                mCamera.Move(new Vector2(0, -mCamera.BoundingRectangle.Y));
            }

            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            SlidingPanel slidingPanel = null;
            if (towerSelectLayer != null)
            {
                slidingPanel = towerSelectLayer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            }

            // if the build panel is open we have to extended the camera's position more.
            if (slidingPanel != null && (slidingPanel.PositionComponent as SlidingPanelPositionComponent).IsOpen)
            {
                if (mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4) + Viewport.Width / 3)
                {
                    mCamera.Move(new Vector2(Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 4) - mCamera.BoundingRectangle.Width - mCamera.BoundingRectangle.X + Viewport.Width / 3, 0));
                }
            }
            else
            {
                if (mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 16))
                {
                    mCamera.Move(new Vector2(Viewport.Width + Math.Max(TileMap.Width / 2, Viewport.Width / 16) - mCamera.BoundingRectangle.Width - mCamera.BoundingRectangle.X, 0));
                }

                if (mCamera.BoundingRectangle.Y + mCamera.BoundingRectangle.Height > Viewport.Height + Math.Max(TileMap.Height / 2, Viewport.Height / 4))
                {
                    mCamera.Move(new Vector2(0, Viewport.Height + Math.Max(TileMap.Height / 2, Viewport.Height / 4) - mCamera.BoundingRectangle.Height - mCamera.BoundingRectangle.Y));
                }
            }



            if (Shake)
            {
                //generate shake direction
                if (mShakeDirection == Vector2.Zero)
                {
                    Random random = new Random();
                    mShakeDirection = new Vector2((float)(random.NextDouble() * 2.0 - 1.0), (float)(random.NextDouble() * 2.0 - 1.0));
                }

                mCamera.Move(mShakeDirection * mShakeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

                mShakeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mShakeDuration <= 0.0f)
                {
                    mShakeDirection = -mShakeDirection;
                    mShakeDuration = kInterval;

                    if (mSign < 0)
                    {
                        mShakeSpeed = (int)(mShakeSpeed * kShakeScale);
                    }

                    mSign = -mSign;
                }

                if (mShakeSpeed <= 10)
                {
                    mShakeDirection = Vector2.Zero;
                    mShakeDuration = kInterval;
                    mShakeSpeed = kShakeSpeed;
                    mShake = false;
                }

            }







            layer.Matrix = mCamera.GetViewMatrix();

            if (mDelta != Vector2.Zero || mIsFlick || mIsZoom)
                EventSystem.EnqueueEvent(EventType.MapScrolled);

#endif


        }
    }
}
