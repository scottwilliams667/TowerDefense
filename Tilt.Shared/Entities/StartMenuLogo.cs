using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;
using Tilt.Shared.Structures;

namespace Tilt.Shared.Entities
{
    public class StartMenuLogo : Entity
    {
        private StartMenuLogoRenderComponent mRenderComponent;
        private PositionComponent mPositionComponent;
        private SimpleAudioComponent mAudioComponent;

        public StartMenuLogo(string texturePath, int x, int y)
        {
            mRenderComponent = new StartMenuLogoRenderComponent(texturePath, this);
            mPositionComponent = new PositionComponent(x,y,this);
            mAudioComponent = new SimpleAudioComponent(Tuner.SFXUILogoDrop, this);
        }

        public StartMenuLogoRenderComponent RenderComponent
        {
            get { return mRenderComponent;}
            set { mRenderComponent = value; }
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; ;}
            set { mPositionComponent = value; }
        }

        public SimpleAudioComponent AudioComponent
        {
            get { return mAudioComponent; }
            set { mAudioComponent = value; }
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            mPositionComponent.UnRegister();
            mAudioComponent.UnRegister();
        }
    }

    public class StartMenuLogoRenderComponent : UIRenderComponent
    {
        private float kStartScale = 2.0f;
        private float mStartScale = 2.0f;
        private float mEndScale = 1.0f;
        private float mScaleIncrement = 0.09f;
        private bool mPlayedSoundEffect = false;

        public StartMenuLogoRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
        }

        public void ResetScale()
        {
            mStartScale = kStartScale;
            mPlayedSoundEffect = false;
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            StartMenuLogo startMenuLogo = Owner as StartMenuLogo;
            PositionComponent positionComponent = startMenuLogo.PositionComponent;
            AudioComponent audioComponent = startMenuLogo.AudioComponent;
            
            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(mTexture.Width / 2, mTexture.Height / 2), null, Color.White, 0.0f, new Vector2(mTexture.Width / 2, mTexture.Height / 2), 
                1.0f * mStartScale, SpriteEffects.None, 0.25f);

            mStartScale = (mStartScale > mEndScale) ? mStartScale - mScaleIncrement : mEndScale;

            if(mStartScale == mEndScale && !mPlayedSoundEffect)
            {
                mPlayedSoundEffect = true;
                audioComponent.Play();
            }


        }
    }
}
