using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;
using Tilt.Shared.Structures;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class TowerSelectListPanelState : PanelState
    {
        public int ButtonHeight
        {
            get 
            { 
                return Elements.Where(e => e is Button)
                                .Cast<Button>().First()
                                .AnimationComponent.SourceRectangle.Height; 
            }
        }

        public int ButtonWidth
        {
            get { return Elements.Where(e => e is Button)
                                .Cast<Button>().First()
                                .AnimationComponent.SourceRectangle.Width; }
        }

        public int Count
        {
            get { return Elements.Count; }
        }
    }

    public class TowerSelectList : UIElement
    {
        private TowerSelectListRenderComponent mRenderComponent;
        private TowerSelectListTouchComponent mTouchComponent;
        private TowerSelectListPanelState mPanelState;

        public TowerSelectList(int x, int y, int xDest, int yDest, int viewportHeight) : base(x,y)
        {
            PositionComponent = new TowerSelectListPositionComponent(x, y, xDest, yDest, this);
            mRenderComponent = new TowerSelectListRenderComponent("buildlistpanel", this);
            mTouchComponent  = new TowerSelectListTouchComponent(new Rectangle(x * 2/3, viewportHeight / 5, x  / 3, viewportHeight * 4/5), this);

            Texture2D bulletTowerTexture = AssetOps.LoadAsset<Texture2D>("pistoltowerbutton");

            int offset = viewportHeight / 5;
            int height = bulletTowerTexture.Height;

            mPanelState = new TowerSelectListPanelState()
            {
                Elements = new List<UIElement>()
                {
                    new TowerSelectButton(x, offset, "refinerybutton",
                        1, 1, MenuManager.SelectBuildObject, ObjectType.Refinery, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + height, "pistoltowerbutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Bullet, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (2 * height), "heavytowerbutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Heavy, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (3 * height), "shotguntowerbutton", 1,
                        1, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Shotgun, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (4 * height), "lasertowerbutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Laser, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (5 * height), "nucleartowerbutton", 1,
                        1, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Nuclear, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (6 * height), "rockettowerbutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Rocket, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (7 * height), "coolingaddonbutton", 1,
                        1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.Cooldown, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (8 * height), "ammoaddonbutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.AmmoCapacity, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (9 * height), "refineryaddonbutton", 1,
                        1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.Refinery, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (10 * height), "rangeboosteraddonbutton",
                        1, 1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.RangeBooster, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (11 * height), "barricadebutton", 1, 1,
                        MenuManager.SelectBuildObject, ObjectType.Barricade, Tuner.SFXButtonClick),
                    new TowerSelectButton(x, offset + (12 * height), "shieldgeneratorbutton",
                        1, 1, MenuManager.SelectBuildObject, ObjectType.ShieldGenerator, Tuner.SFXButtonClick)
                    


                }
            };
        }

        public TowerSelectListTouchComponent TouchComponent
        {
            get {return mTouchComponent;}
        }

        public TowerSelectListRenderComponent RenderComponent
        {
            get { return mRenderComponent;}
        }

        public TowerSelectListPanelState PanelState
        {
            get { return mPanelState; }
        }

        public void Reset()
        {
            TowerSelectListPositionComponent positionComponent = PositionComponent as TowerSelectListPositionComponent;
            positionComponent.Reset();
        }

    }

    public class TowerSelectListTouchComponent : TouchAreaComponent
    {    
        public TowerSelectListTouchComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
#if WINDOWS
            if(MouseOps.ContainsPoint(mBounds))
            {
                
                if(MouseOps.IsMouseScrolling())
                {
                    TowerSelectList list = Owner as TowerSelectList;
                    GameTime gameTime = ServiceLocator.GetService<GameTime>();

                    PositionComponent positionComponent = list.PositionComponent;
                    TouchAreaComponent touchComponent = list.TouchComponent;

                    int direction = MouseOps.GetScrollWheelDelta();

                    Vector2 offset = new Vector2(0, direction * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    Button topButton = list.PanelState.Elements.First() as Button;
                    Button bottomButton = list.PanelState.Elements.Last() as Button;

                    if (topButton.PositionComponent.Position.Y + offset.Y >= mBounds.Y)
                        offset.Y = mBounds.Y - topButton.PositionComponent.Position.Y;

                    if (bottomButton.PositionComponent.Position.Y +
                        bottomButton.AnimationComponent.SourceRectangle.Height +
                        offset.Y <= mBounds.Y + mBounds.Height)
                        offset.Y = (mBounds.Height + mBounds.Y) - (bottomButton.PositionComponent.Y +
                                                                   bottomButton.AnimationComponent
                                                                       .SourceRectangle.Height);

                    foreach (Button button in list.PanelState.Elements)
                    {
                        button.PositionComponent.Position += offset;
                        button.TouchComponent.Bounds = new Rectangle(
                            (int)button.PositionComponent.X,
                            (int)button.PositionComponent.Y,
                            button.TouchComponent.Bounds.Width,
                            button.TouchComponent.Bounds.Height);
                    }


                }




            }
#else 
            if (TouchOps.ContainsPoint(mBounds))
            {

                if (TouchOps.IsVerticalDrag())
                {
                    TowerSelectList list = Owner as TowerSelectList;

                    GameTime gameTime = ServiceLocator.GetService<GameTime>();
                    PositionComponent positionComponent = list.PositionComponent;
                    TouchAreaComponent touchComponent = list.TouchComponent;

                    Vector2 position = Vector2.Zero;

                    foreach (GestureSample gesture in TouchOps.GestureCollection)
                    {
                        switch (gesture.GestureType)
                        {
                            case GestureType.VerticalDrag:
                            {
                                position = gesture.Position;
                                Vector2 oldPosition = gesture.Position - gesture.Delta;
                                Vector2 delta = position - oldPosition;

                                Vector2 offset = (float) gameTime.ElapsedGameTime.TotalSeconds *
                                                 150 * delta;

                                Button topButton = list.PanelState.Elements.First() as Button;
                                Button bottomButton = list.PanelState.Elements.Last() as Button;


                                if (topButton.PositionComponent.Position.Y + offset.Y >= mBounds.Y)
                                    offset.Y = mBounds.Y - topButton.PositionComponent.Position.Y;

                                if (bottomButton.PositionComponent.Position.Y +
                                    bottomButton.AnimationComponent.SourceRectangle.Height +
                                    offset.Y <= mBounds.Y + mBounds.Height)
                                    offset.Y = (mBounds.Height + mBounds.Y) - (bottomButton.PositionComponent.Y +
                                                                               bottomButton.AnimationComponent
                                                                                   .SourceRectangle.Height);

                                foreach (Button button in list.PanelState.Elements)
                                {
                                    button.PositionComponent.Position += offset;
                                    button.TouchComponent.Bounds = new Rectangle(
                                        (int) button.PositionComponent.X,
                                        (int) button.PositionComponent.Y,
                                        button.TouchComponent.Bounds.Width,
                                        button.TouchComponent.Bounds.Height);
                                }

                            }
                                break;
                        }
                    }
                }

                // if we are not doing a tap, clear the touch 
                // gestures so we dont pass them to a game layer, which
                // will scroll the map and the dialog at the same time

                if(!TouchOps.IsTap() && !TouchOps.IsDoubleTap())
                    TouchOps.ClearTouch();

            }
#endif
        }
    }

    public class TowerSelectListRenderComponent : UIRenderComponent
    {
        private bool mIsVisible = true;
        public TowerSelectListRenderComponent(string texturePath, Entity owner)
            : base(texturePath, owner)
        {
        }

        public bool IsVisible
        {
            get { return mIsVisible;}
            set { mIsVisible = value; }
        }

        public override void Update()
        {
 	        SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            TowerSelectList list = Owner as TowerSelectList;
            PositionComponent positionComponent = list.PositionComponent;
            TowerSelectListTouchComponent touchComponent = list.TouchComponent;

            if (mIsVisible)
            {
                spriteBatch.Draw(mTexture, positionComponent.Position,
                    new Rectangle(0, 0, mTexture.Width, (int) (mTexture.Height * 0.21f)),
                    Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);

                spriteBatch.Draw(mTexture, positionComponent.Position,
                    new Rectangle(0, (int)(mTexture.Height * 0.2f), mTexture.Width, mTexture.Height),
                    Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.12f);
            }

            Rectangle bounds = touchComponent.Bounds;

            if (!mIsVisible)
            {
                foreach (Button button in list.PanelState.Elements)
                {
                    button.AnimationComponent.IsVisible = false;
                    button.AnimationComponent.IsEnabled = false;
                }
                return;
            }

            foreach(Button button in list.PanelState.Elements)
            {
                if(bounds.Intersects(button.TouchComponent.Bounds))
                {
                    button.AnimationComponent.IsVisible = true;
                    button.AnimationComponent.IsEnabled = true;
                }
                else
                {
                    button.AnimationComponent.IsVisible = false;
                    button.AnimationComponent.IsEnabled = false;
                }
            }

        }
    }


    public class TowerSelectListPositionComponent : PositionComponent
    {
        private Vector2 mDestination;
        private Vector2 mOriginalPosition;
        private Vector2 mDirection = Vector2.Zero;
        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        private int kInitialSpeed;
        private int mSpeed;
        private float mSpeedScale = 0.925f;

        private GraphicsDevice mGraphicsDevice;

        public TowerSelectListPositionComponent(int x, int y, int xDest, int yDest, Entity owner, Vector2 origin = new Vector2()) : base(x, y, owner, origin)
        {
            mOriginalPosition = Position;
            mDestination = new Vector2(xDest, yDest);
            mDirection = new Vector2(xDest - x, yDest - y);
            mDirection.Normalize();

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
            }
        }

        public void Reset()
        {
            TowerSelectList towerSelectList = Owner as TowerSelectList;

            mIsSlidingIn = false;
            mIsSlidingOut = false;
            
            UIOps.ResetPanelStatePositions(towerSelectList.PanelState, mPosition, mOriginalPosition);

            mPosition = mOriginalPosition;
        }

        public override void Update()
        {
            TowerSelectList towerSelectList = Owner as TowerSelectList;
            TouchAreaComponent touchComponent = towerSelectList.TouchComponent;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if (mIsSlidingIn)
            {

                if (mPosition.X - mDestination.X < mGraphicsDevice.Viewport.Width * 17/100)
                    mSpeed = mSpeed > mGraphicsDevice.Viewport.Width * 13/100 ? (int)(mSpeed * mSpeedScale) : mSpeed;

                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X - xOffset.X < mDestination.X)
                    xOffset.X = mDestination.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in towerSelectList.PanelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                    }
                }
            }

            if (mIsSlidingOut)
            {
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X > mOriginalPosition.X)
                    xOffset.X = mOriginalPosition.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in towerSelectList.PanelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                    }
                }
            }

            if (mPosition == mDestination && mIsSlidingIn)
            {
                mIsSlidingIn = false;
                mSpeed = kInitialSpeed;
            }
            if (mPosition == mOriginalPosition && mIsSlidingOut)
            {
                mIsSlidingOut = false;
            }
        }
    }
}
