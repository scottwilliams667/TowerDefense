using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;

namespace Tilt.Shared.Components
{
    public class AreaOfEffectAnimationComponent
        : AnimationComponent
    {
        private float mScale = 1.0f;
        private float kScaleIncrement = 1.05f;
        private float kMaxScale = 2.0f;

        public AreaOfEffectAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            AreaOfEffectParticle particle = Owner as AreaOfEffectParticle;
            PositionComponent position = particle.PositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            

            spriteBatch.Draw(mTexture, position.Position, CurrentRectangle, Color.White, 0.0f, new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2),  mScale, SpriteEffects.None, 0.35f);

            if(SystemsManager.Instance.IsPaused)
                return;

            if (mScale >= kMaxScale)
            {
                particle.UnRegister();
                return;
            }

            //stretch the last frame out
            if(CurrentColumnIndex == Columns - 1)
            {
                mScale = mScale * kScaleIncrement;
                return;
            }

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }

            CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex * SourceRectangle.Width, SourceRectangle.Y + CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
        }
    }
}
