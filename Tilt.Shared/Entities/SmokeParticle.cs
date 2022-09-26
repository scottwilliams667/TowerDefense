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
    public class SmokeParticle : Entity
    {
        private SmokeParticleAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;

         public SmokeParticle(int x, int y, string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns)
        {
            mAnimationComponent = new SmokeParticleAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mPositionComponent = new PositionComponent(x, y, this);
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
    }

    public class SmokeParticleAnimationComponent : AnimationComponent
    {
        private Vector2 mPosition;
        private float mLayerDepth;
        private Random mRandom = new Random();
        public SmokeParticleAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            CurrentRowIndex = mRandom.Next(0, 2);
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
            CurrentTime = interval;

            mLayerDepth = (float)(mRandom.NextDouble() * (0.10 - 0.05) + 0.05);
        }

        public override void Update()
        {
            SmokeParticle fireParticle = Owner as SmokeParticle;
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            PositionComponent positionComponent = fireParticle.PositionComponent;

            if (mPosition == Vector2.Zero)
            {
                mPosition = positionComponent.Position;
            }

            spriteBatch.Draw(mTexture, mPosition, CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.35f);

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
