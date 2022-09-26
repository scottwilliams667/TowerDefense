using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Entities
{
    public class FireParticle : Entity
    {
        private FireParticleAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;

        public FireParticle(int x, int y, string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns)
        {
            mPositionComponent = new PositionComponent(x, y, this);
            mAnimationComponent = new FireParticleAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mPositionComponent.UnRegister();
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent;}
            set { mPositionComponent = value; }
        }

        public FireParticleAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }
    }



    public class FireParticleAnimationComponent : AnimationComponent
    {
        private Vector2 mPosition = Vector2.Zero;
        private float mLayerDepth;
        private Random mRandom = new Random();
        public FireParticleAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {

            CurrentRowIndex = mRandom.Next(0, 2);
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
            CurrentTime = interval;
            mLayerDepth = (float)(mRandom.NextDouble() * (0.10 - 0.05) + 0.05);

            FireParticle fireParticle = Owner as FireParticle;
            PositionComponent positionComponent = fireParticle.PositionComponent;

            positionComponent.Position = new Vector2(positionComponent.Position.X + mRandom.Next(-4, 4), positionComponent.Position.Y + mRandom.Next(-4, 4));
            mPosition = positionComponent.Position;
        }

        public override void Update()
        {
            
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            

            if (mPosition == Vector2.Zero)
            {
            }

            spriteBatch.Draw(mTexture, mPosition, CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.4f + mLayerDepth);

            if (SystemsManager.Instance.IsPaused)
                return;


            mPosition -= new Vector2(0, 0.5f);
            
            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }
            if (CurrentColumnIndex >= Columns)
            {
                Owner.UnRegister();
            }

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

        }


    }

}
