using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Structures;

namespace Tilt.Shared.Entities
{
    public class TowerSlamParticle : Entity
    {
        private PositionComponent mPositionComponent;
        private TowerSlamParticleAnimationComponent mAnimationComponent;
        public TowerSlamParticle(string texturePath, Rectangle sourceRectangle, int x, int y, int rows, int columns, float interval)
        {
            mAnimationComponent = new TowerSlamParticleAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mPositionComponent = new PositionComponent(x, y, this);
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public TowerSlamParticleAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mPositionComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class TowerSlamParticleAnimationComponent : AnimationComponent
    {
        public TowerSlamParticleAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            TowerSlamParticle particle = Owner as TowerSlamParticle;
            PositionComponent positionComponent = particle.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(-TileMap.TileWidth/2, TileMap.TileHeight/2), CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.4f);
            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(TileMap.TileWidth/2, TileMap.TileHeight/2), CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.4f);
            

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;

                if (CurrentColumnIndex >= Columns)
                {
                    particle.UnRegister();
                    return;
                }
            }

            CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex * SourceRectangle.Width,
                       SourceRectangle.Y + CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

        }
    }
}
