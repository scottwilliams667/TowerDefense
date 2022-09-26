using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class PauseMenuPanel : UIElement
    {
        private PanelState mPanelState;
        private PauseMenuPanelRenderComponent mRenderComponent;

        public PauseMenuPanel(int x, int y, int xDest, int yDest, string texturePath, PanelState selectedState, Action action = null, bool register = true, string name = null) :
            base(x,y, register, name)
        {
            mPanelState = selectedState;
            mRenderComponent = new PauseMenuPanelRenderComponent(texturePath, this);
            PositionComponent = new PauseMenuPanelPositionComponent(x, y, xDest, yDest, action, this);
            
        }

        public PanelState PanelState
        {
            get { return mPanelState; }
        }

        public void Reset()
        {
            PauseMenuPanelPositionComponent positionComponent = PositionComponent as PauseMenuPanelPositionComponent;
            positionComponent.Reset();
        }
    }

    public class PauseMenuPanelRenderComponent : UIRenderComponent
    {
        public PauseMenuPanelRenderComponent(string texturePath, Entity owner) : base(texturePath, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            PauseMenuPanel panel = Owner as PauseMenuPanel;
            PauseMenuPanelPositionComponent positionComponent = panel.PositionComponent as PauseMenuPanelPositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
        }
    }

    public class PauseMenuPanelPositionComponent : PositionComponent
    {
        private Vector2 mDestination;
        private Vector2 mOriginalPosition;
        private Vector2 mDirection = Vector2.Zero;
        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        private int mSpeed = 1120;
        private int kInitialSpeed = 1120;
        private float mSpeedScale = 0.925f;

        private Action mAction;

        private GraphicsDevice mGraphicsDevice;

        public PauseMenuPanelPositionComponent(int x, int y, int xDest, int yDest, Action action, Entity owner)
            : base(x,y, owner)
        {
            mOriginalPosition = mPosition;
            mDestination = new Vector2(xDest, yDest);
            mDirection = new Vector2(xDest - x, yDest - y);
            mDirection.Normalize();
            mAction = action;
            mGraphicsDevice = ServiceLocator.GetService<GraphicsDevice>();

            mSpeed = mGraphicsDevice.Viewport.Width;
            kInitialSpeed = mGraphicsDevice.Viewport.Width;
        }

        public bool IsSlidingIn
        {
            get { return mIsSlidingIn; }
            set
            {
                if (mIsSlidingIn == value) return;

                mIsSlidingIn = value;
                mDirection = new Vector2(mDestination.X - X, mDestination.Y - Y);
                mDirection.Normalize();

                if (float.IsNaN(mDirection.X) && float.IsNaN(mDirection.Y))
                {
                    mDirection = Vector2.Zero;
                    mIsSlidingOut = false;
                }

            }
        }

        public bool IsSlidingOut
        {
            get { return mIsSlidingOut; }
            set
            {
                if (mIsSlidingOut == value) return;

                mIsSlidingOut = value;
                mDirection = new Vector2(mOriginalPosition.X - X, mOriginalPosition.Y - Y);
                mDirection.Normalize();

                if (float.IsNaN(mDirection.X) && float.IsNaN(mDirection.Y))
                {
                    mDirection = Vector2.Zero;
                    mIsSlidingOut = false;
                }
            }
        }

        public void Reset()
        {
            PauseMenuPanel pauseMenuPanel = Owner as PauseMenuPanel;
            mIsSlidingIn = false;
            mIsSlidingOut = false;

            UIOps.ResetPanelStatePositions(pauseMenuPanel.PanelState, mPosition, mOriginalPosition);
            mPosition = mOriginalPosition;
        }

        public override void Update()
        {
            PauseMenuPanel pauseMenuPanel = Owner as PauseMenuPanel;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if (mIsSlidingIn)
            {



                if (mPosition.X - mDestination.X < mGraphicsDevice.Viewport.Width * 17/100)
                    mSpeed = mSpeed > mGraphicsDevice.Viewport.Width * 13/100 ? (int)(mSpeed * mSpeedScale) : mSpeed;
                
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X - xOffset.X < mDestination.X)
                    xOffset.X = mDestination.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in pauseMenuPanel.PanelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);

                        button.AnimationComponent.IsEnabled = false;
                    }
                }
            }

            if (mIsSlidingOut)
            {
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X > mOriginalPosition.X)
                    xOffset.X = mOriginalPosition.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in pauseMenuPanel.PanelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);

                        button.AnimationComponent.IsEnabled = false;
                    }
                }
            }

            if (mPosition == mDestination && mIsSlidingIn)
            {
                mIsSlidingIn = false;
                mSpeed = kInitialSpeed;

                foreach(UIElement element in pauseMenuPanel.PanelState.Elements)
                {
                    if(element is Button)
                    {
                        Button button = element as Button;
                        button.AnimationComponent.IsEnabled = true;
                    }
                }
            }
            if (mPosition == mOriginalPosition && mIsSlidingOut)
            {
                mIsSlidingOut = false;
                if (mAction != null)
                    mAction();
            }

        }
        
    }
}
