using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Components
{


    public interface IAnimationSequence
    {
        void Start();
        void Stop();

        void Cancel();

        bool IsStarted { get; set; }
    }

    public class TowerSelectButtonAnimationComponent : ButtonAnimationComponent
    {
        private ObjectType mObjectType;
        private TowerType mTowerType;
        private AddOnType mAddOnType;
        private SpriteFont mDebugFont;

        public TowerSelectButtonAnimationComponent(string texturePath, float interval, int rows, int columns, Entity owner, 
            ObjectType objectType, TowerType towerType = TowerType.None, AddOnType addOnType = AddOnType.None)
            : base(texturePath, interval, rows, columns, owner)
        {
            IsVisible = true;
            IsEnabled = true;
            IsStarted = false;

            mObjectType = objectType;
            mTowerType = towerType;
            mAddOnType = addOnType;

            mDebugFont = AssetOps.LoadAsset<SpriteFont>("TowerSelectFont");
        }
        
        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            TowerSelectButton button = Owner as TowerSelectButton;
            PositionComponent positionComponent = button.PositionComponent;

            IData data = null;
            if (mObjectType == ObjectType.Tower)
            {
                data = ObjectFactory.GetDataForTower(mTowerType);
            }
            else if(mObjectType == ObjectType.AddOn)
            {
                data = ObjectFactory.GetDataForAddOn(mAddOnType);
            }
            else
            {
                data = ObjectFactory.GetDataForObject(mObjectType);
            }

            ISellable sellable = data as ISellable;

            Layer hudLayer = LayerManager.GetLayer(LayerType.TowerSelect);


            SlidingPanel slidingPanel = hudLayer.EntitySystem.GetEntitiesByType<SlidingPanel>().First();
            if (slidingPanel.PanelAction != PanelAction.TowerSelecting)
                return;


            Vector2 pricePosition = new Vector2(
                positionComponent.Position.X + mTexture.Width * 29/100,
                positionComponent.Position.Y + mTexture.Height *  60/100
                );

            if(sellable != null)
            {
                spriteBatch.DrawString(mDebugFont, "$" + sellable.PriceToBuy, pricePosition, Color.Black, 0.0f, 
                    new Vector2(mDebugFont.MeasureString("$" + sellable.PriceToBuy).X / 2, 0.0f),
                    new Vector2(0.85f, 0.85f) * Scale, SpriteEffects.None, 0.17f);
            }

            base.Update();
        }
    }

    public class ButtonAnimationComponent : UIAnimationComponent, IAnimationSequence
    {
        private bool mIsVisible;
        private bool mIsEnabled;
        private bool mIsStarted;
        private bool mShrink;
        private bool mCancel;
        private float mScale = 1.0f;
        private float kMinScale = 0.9f;
        private float kMaxScale = 1.0f;
        private float kIncrement = 0.06f;


        public ButtonAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            IsVisible = true;
            IsEnabled = true;
            IsStarted = false;
        }

        public ButtonAnimationComponent(string texturePath, float interval, int rows, int columns, Entity owner)
            : base(texturePath, Rectangle.Empty, interval, rows, columns, owner)
        {
            IsVisible = true;
            IsEnabled = true;
            IsStarted = false;
            SourceRectangle = new Rectangle(0, 0, mTexture.Width, mTexture.Height);
        }

        public bool IsVisible
        {
            get { return mIsVisible;}
            set { mIsVisible = value; }
        }

        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; }
        }

        public bool IsStarted
        {
            get { return mIsStarted; }
            set
            {
                mIsStarted = value;
            }
        }

        public bool Shrink
        {
            get { return mShrink;}
            set { mShrink = value; }
        }

        protected float MinScale
        {
            get { return kMinScale;}
        }

        protected float MaxScale
        {
            get { return kMaxScale;}
        }

        protected float Scale
        {
            get { return mScale;}
            set { mScale = value; }
        }

        protected float Increment
        {
            get { return kIncrement;}
        }

        protected bool Cancelled
        {
            get { return mCancel;}
            set { mCancel = value; }
        }

        public void Start()
        {
            IsStarted = true;
            mShrink = true;
            mScale = kMaxScale;
        }

        public void Stop()
        {
            IsStarted = false;
            mShrink = false;
        }

        public void Cancel()
        {
            mCancel = true;
            mShrink = false;
        }

        public bool IsMinScale()
        {
            return mScale == kMinScale;
        }

        public override void Update()
        {
            if (!mIsVisible)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Button button = Owner as Button;
            PositionComponent positionComponent = button.PositionComponent;

            //if not enabled, display the disabled icon
            if (!mIsEnabled)
            {
                CurrentRowIndex = 0;
                CurrentColumnIndex = 0;
            }
            //animation not started, show button icon
            if (!IsStarted && mIsEnabled)
            {
                CurrentColumnIndex = Columns - 1;
                CurrentRowIndex = Rows - 1;
            }

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            if (mIsEnabled)
            {
                //animation started and shrinking icon
                if (IsStarted && mShrink)
                {
                    if (mScale < kMinScale)
                    {

                        mScale = kMinScale;
                        mShrink = false;
                    }
                    else if (mScale > kMinScale)
                        mScale -= kIncrement;
                }
                //shrinking is done, resize back to full
                else if (IsStarted && !mShrink)
                {
                    //we clicked the button but dragged off of it
                    if (mCancel)
                    {
                        mScale += kIncrement;
                        if (mScale > kMaxScale)
                        {
                            IsStarted = false;
                            mCancel = false;
                            mScale = kMaxScale;
                        }
                    }
                    //resize back to full && launch the action
                    else if (mScale > kMaxScale)
                    {
                        mScale = kMaxScale;
                        IsStarted = false;
                        ActionComponent actionComponent = button.ActionComponent;
                        if (actionComponent != null)
                            actionComponent.Execute();

                    }
                    else if (mScale < kMaxScale)
                        mScale += kIncrement;

                }
            }

            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), CurrentRectangle, Color.White, 0.0f,
                new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), 1.0f * mScale, SpriteEffects.None, 0.15f);

        }

    }

    public class PauseButtonRenderComponent : ButtonAnimationComponent
    {
        public PauseButtonRenderComponent(string texturePath,int rows, int columns, Entity owner)
            : base(texturePath, Rectangle.Empty, 0.0f, rows, columns, owner)
        {
            SourceRectangle = new Rectangle(0,0, mTexture.Width / 2, mTexture.Height);
        }


        public override void Update()
        {
            if (!IsVisible)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Button button = Owner as Button;
            PositionComponent positionComponent = button.PositionComponent;

            CurrentColumnIndex = SystemsManager.Instance.IsPaused ? Columns - 1 : 0;
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            //animation started and shrinking icon
            if (IsStarted && Shrink)
            {
                if (Scale < MinScale)
                {

                    Scale = MinScale;
                    Shrink = false;
                }
                else if (Scale > MinScale)
                    Scale -= Increment;
            }
            //shrinking is done, resize back to full
            else if (IsStarted && !Shrink)
            {
                //we clicked the button but dragged off of it
                if (Cancelled)
                {
                    Scale += Increment;
                    if (Scale > MaxScale)
                    {
                        IsStarted = false;
                        Cancelled = false;
                        Scale = MaxScale;
                    }
                }
                //resize back to full && launch the action
                else if (Scale > MaxScale)
                {
                    Scale = MaxScale;
                    IsStarted = false;
                    ActionComponent actionComponent = button.ActionComponent;
                    if (actionComponent != null)
                        actionComponent.Execute();

                }
                else if (Scale < MaxScale)
                    Scale += Increment;

            }


            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), CurrentRectangle, Color.White, 0.0f,
                new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), 1.0f * Scale, SpriteEffects.None, 0.1f);

        }
    }

    public class SoundFXButtonRenderComponent : ButtonAnimationComponent
    {
        public SoundFXButtonRenderComponent(string texturePath, float interval, int rows, int columns, Entity owner) : base(texturePath, Rectangle.Empty, interval, rows, columns, owner)
        {
            SourceRectangle = new Rectangle(0, 0, mTexture.Width, mTexture.Height / 2);
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Button button = Owner as Button;
            PositionComponent positionComponent = button.PositionComponent;

            if (!IsVisible)
                return;

            CurrentRowIndex = AudioSystem.IsSFXMuted ? Rows - 1 : 0;
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            //animation started and shrinking icon
            if (IsStarted && Shrink)
            {
                if (Scale < MinScale)
                {

                    Scale = MinScale;
                    Shrink = false;
                }
                else if (Scale > MinScale)
                    Scale -= Increment;
            }
            //shrinking is done, resize back to full
            else if (IsStarted && !Shrink)
            {
                //we clicked the button but dragged off of it
                if (Cancelled)
                {
                    Scale += Increment;
                    if (Scale > MaxScale)
                    {
                        IsStarted = false;
                        Cancelled = false;
                        Scale = MaxScale;
                    }
                }
                //resize back to full && launch the action
                else if (Scale > MaxScale)
                {
                    Scale = MaxScale;
                    IsStarted = false;
                    ActionComponent actionComponent = button.ActionComponent;
                    if (actionComponent != null)
                        actionComponent.Execute();

                }
                else if (Scale < MaxScale)
                    Scale += Increment;

            }


            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), CurrentRectangle, Color.White, 0.0f,
                new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), 1.0f * Scale, SpriteEffects.None, 0.11f);
        }
    }

    public class MusicButtonRenderComponent : ButtonAnimationComponent
    {
        public MusicButtonRenderComponent(string texturePath, float interval, int rows, int columns, Entity owner) : base(texturePath, Rectangle.Empty, interval, rows, columns, owner)
        {
            SourceRectangle = new Rectangle(0,0, mTexture.Width, mTexture.Height / 2);
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Button button = Owner as Button;
            PositionComponent positionComponent = button.PositionComponent;

            CurrentRowIndex = AudioSystem.IsMusicMuted ? Rows - 1 : 0;
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            //animation started and shrinking icon
            if (IsStarted && Shrink)
            {
                if (Scale < MinScale)
                {

                    Scale = MinScale;
                    Shrink = false;
                }
                else if (Scale > MinScale)
                    Scale -= Increment;
            }
            //shrinking is done, resize back to full
            else if (IsStarted && !Shrink)
            {
                //we clicked the button but dragged off of it
                if (Cancelled)
                {
                    Scale += Increment;
                    if (Scale > MaxScale)
                    {
                        IsStarted = false;
                        Cancelled = false;
                        Scale = MaxScale;
                    }
                }
                //resize back to full && launch the action
                else if (Scale > MaxScale)
                {
                    Scale = MaxScale;
                    IsStarted = false;
                    ActionComponent actionComponent = button.ActionComponent;
                    if (actionComponent != null)
                        actionComponent.Execute();

                }
                else if (Scale < MaxScale)
                    Scale += Increment;

            }


            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), CurrentRectangle, Color.White, 0.0f,
                new Vector2(CurrentRectangle.Width / 2, CurrentRectangle.Height / 2), 1.0f * Scale, SpriteEffects.None, 0.11f);
        }
    }
}
