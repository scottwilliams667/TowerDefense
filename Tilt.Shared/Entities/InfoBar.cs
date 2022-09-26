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
    public class InfoBar : UIElement
    {
        private InfoBarRenderComponent mRenderComponent;
        private InfoBarTimeComponent mTimeComponent;
        public InfoBar(string texturePath, int x, int y, string name) : base(x,y, name: name)
        {
            PositionComponent = new PositionComponent(x, y, this);
            mRenderComponent = new InfoBarRenderComponent(texturePath, this);
            mTimeComponent = new InfoBarTimeComponent(this);
        }

        public InfoBarTimeComponent TimeComponent
        {
            get { return mTimeComponent; }
        }

        public InfoBarRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
        }


        public void SetTimeOfDay(DateTime dateTime)
        {
            if(mTimeComponent != null)
                mTimeComponent.SetTimeOfDay(dateTime);
        }

        public DateTime GetTimeOfDay()
        {
            if (mTimeComponent == null)
                return DateTime.MinValue;

            return mTimeComponent.GetTimeOfDay();
        }

        public TimeSpan GetTimeElapsed()
        {
            if (mTimeComponent == null)
                return TimeSpan.Zero;

            return mTimeComponent.GetTimeElapsed();
        }

        public void PauseTime()
        {
            if (mTimeComponent == null)
                return;

            mTimeComponent.Pause();
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class InfoBarRenderComponent : UIRenderComponent
    {
        private SpriteFont mFont;

        private bool mIsAnimating;
        private bool mIncrementing;
        private bool mIsVisible = true;

        private int mOldValue;
        private int mNewValue;
        private float mRate;
        private float mAnimationTime = 0.5f;

        
        

        public InfoBarRenderComponent(string texturePath, Entity owner) : base(texturePath, owner) 
        {
            mFont = AssetOps.LoadAsset<SpriteFont>("InfoBarFont");
            EventSystem.SubScribe(EventType.MineralsChanged, OnMineralsChanged_);
        }

        public override void UnRegister()
        {
            EventSystem.UnSubScribe(EventType.MineralsChanged, OnMineralsChanged_);
            base.UnRegister();

        }

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; }
        }

        public override void Update()
        {
            if (!mIsVisible)
                return;

            InfoBar infoBar = Owner as InfoBar;
            InfoBarTimeComponent timeComponent = infoBar.TimeComponent;
            PositionComponent positionComponent = infoBar.PositionComponent;

            if (positionComponent == null)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            DateTime dateTime = timeComponent.GetTimeOfDay();

            string time = dateTime.ToString("HH:mm");

            

            spriteBatch.DrawString(mFont, LevelManager.Level.Name, new Vector2(positionComponent.Position.X + mTexture.Width / 16, positionComponent.Position.Y + mTexture.Height / 16),
                Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.085f);

            spriteBatch.DrawString(mFont, time, new Vector2(positionComponent.Position.X + mTexture.Width * 46/100, positionComponent.Position.Y + mTexture.Height / 16),
                Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.085f);

            if (mIsAnimating)
            {
                mOldValue += (int)mRate;

                if (mOldValue >= mNewValue && mIncrementing)
                    mIsAnimating = false;
                else if (mOldValue <= mNewValue && !mIncrementing)
                    mIsAnimating = false;

                spriteBatch.DrawString(mFont, string.Format("{0}", mOldValue),
                   new Vector2(positionComponent.Position.X + (mTexture.Width * 3 / 4), positionComponent.Position.Y + mTexture.Height / 16), Color.Black,
                   0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.085f);

            }
            else
            {

                spriteBatch.DrawString(mFont, string.Format("{0}", Resources.Minerals),
                    new Vector2(positionComponent.Position.X + (mTexture.Width * 3 / 4), positionComponent.Position.Y + mTexture.Height / 16), Color.Black,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.085f);
            }


            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.082f);

        }

        private void OnMineralsChanged_(object sender, IGameEventArgs e)
        {
            MineralsChangedArgs args = e as MineralsChangedArgs;

            //dont overwrite the old value if we get two animation calls
            if (!mIsAnimating)
            {
                mOldValue = (int)args.OldValue;
            }

            mIsAnimating = true;

            mNewValue = (int)args.NewValue;

            mIncrementing = (mNewValue > mOldValue);

            mRate = (mNewValue - mOldValue) / 15.0f;

        }

    }

    public class InfoBarTimeComponent : TimerComponent
    {
        private DateTime mDateTime = DateTime.MinValue;
        private DateTime mStartTime = DateTime.MinValue;
        private TimeSpan mTimeSpan = TimeSpan.Zero;
        private float mInterval = 1.0f;
        private bool mPaused = false;

        public InfoBarTimeComponent(Entity owner, bool register = true) : base(owner, register)
        {
            mTimeLeft = mInterval;
        }

        public void SetTimeOfDay(DateTime dateTime)
        {
            mTimeLeft = mInterval;
            mDateTime = dateTime;
            mStartTime = dateTime;
        }

        public DateTime GetTimeOfDay()
        {
            return mDateTime;
        }

        public TimeSpan GetTimeElapsed()
        {
            return mDateTime - mStartTime;
        }

        public void Pause()
        {
            mPaused = true;
        }

        public void Resume()
        {
            mPaused = false;
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused || mPaused)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            mTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (mTimeLeft <= 0.0f)
            {
                mTimeLeft = mInterval;
                mDateTime = mDateTime.AddMinutes(1);
            }
        }
    }
}
