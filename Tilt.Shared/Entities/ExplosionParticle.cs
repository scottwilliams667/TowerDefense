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
    public class ExplosionParticle : Entity
    {
        private ExplosionParticleAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;

        public ExplosionParticle(int x, int y, string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns)
        {
            mPositionComponent = new PositionComponent(x, y, this);
            mAnimationComponent = new ExplosionParticleAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
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

        public ExplosionParticleAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }
    }

    public class ExplosionParticleAnimationComponent : AnimationComponent
    {
        private float mLayerDepth;
        private Random mRandom = new Random();
        private Vector2 mOrigin;
        public ExplosionParticleAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {;
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
            CurrentTime = interval;

            mLayerDepth = (float)(mRandom.NextDouble() * (0.10 - 0.05) + 0.05);

            ExplosionParticle explosionParticle = Owner as ExplosionParticle;
            PositionComponent positionComponent = explosionParticle.PositionComponent;
            mOrigin = positionComponent.Position;

        }

        public override void Update()
        {
            ExplosionParticle explosionParticle = Owner as ExplosionParticle;
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            PositionComponent positionComponent = explosionParticle.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f + mLayerDepth);

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
                positionComponent.Position = new Vector2(mOrigin.X + mRandom.Next(-8, 8), mOrigin.Y + mRandom.Next(-8,8));
                
                if(CurrentColumnIndex >= Columns)
                {
                    CurrentRowIndex++;
                    CurrentColumnIndex = 0;
                }

            }
            if (CurrentRowIndex >= Rows)
            {
                Owner.UnRegister();
            }

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
        }
    }
}
