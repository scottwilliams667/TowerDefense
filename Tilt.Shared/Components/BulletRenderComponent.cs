using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Components
{
    public class BulletRenderComponent : RenderComponent
    {
        private Rectangle mSourceRectangle;
        private float mRotation1 = 0.0f;
        private float mRotation2 = 0.785f;
        private float mRotation3 = 1.570f;
        private float mSpawnTrails = 0.3f;
        private const int kTrailOffset = 10;
        private const float kRotationDecrement = 0.36f;

        public BulletRenderComponent(string texturePath, Rectangle sourceRectangle, Entity owner) : base(texturePath, owner)
        {
            mSourceRectangle = sourceRectangle;
        }

        public override void Update()
        {
            Bullet bullet = Owner as Bullet;
            BulletPositionComponent positionComponent = bullet.PositionComponent as BulletPositionComponent;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            Vector2 offset = new Vector2(kTrailOffset * positionComponent.Direction.X, kTrailOffset * positionComponent.Direction.Y);
            Vector2 offset2 = new Vector2(bullet.PositionComponent.Position.X - offset.X, bullet.PositionComponent.Position.Y - offset.Y);
            Vector2 offset3 = new Vector2(offset2.X - 2 * offset.X, offset2.Y - 2 * offset.Y);

            mSpawnTrails -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Draw(mTexture, bullet.PositionComponent.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), mSourceRectangle, Color.White, mRotation1, new Vector2(mSourceRectangle.Width / 2, mSourceRectangle.Height / 2), 1.0f, SpriteEffects.None, 0.6f);

            if (mSpawnTrails <= 0.0f)
            {
                spriteBatch.Draw(mTexture, offset2 + new Vector2(TileMap.TileWidth / 2 , TileMap.TileHeight / 2), new Rectangle(mSourceRectangle.X + 32, mSourceRectangle.Y, mSourceRectangle.Width, mSourceRectangle.Height), Color.White, 
                    mRotation2, new Vector2(mSourceRectangle.Width / 2, mSourceRectangle.Height / 2), 1.0f, SpriteEffects.None, 0.59f);

                spriteBatch.Draw(mTexture, offset3 + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), new Rectangle(mSourceRectangle.X + 64, mSourceRectangle.Y, mSourceRectangle.Width, mSourceRectangle.Height), Color.White, 
                    mRotation3, new Vector2(mSourceRectangle.Width / 2, mSourceRectangle.Height / 2), 1.0f, SpriteEffects.None, 0.58f);

                mSpawnTrails = 0.0f;
            }
            if (SystemsManager.Instance.IsPaused)
                return;

            mRotation1 -= kRotationDecrement;
            mRotation2 -= kRotationDecrement;
            mRotation3 -= kRotationDecrement;
        }
    }

}
