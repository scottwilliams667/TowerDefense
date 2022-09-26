using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Components
{
    public class BarricadeAnimationComponent : AnimationComponent
    {
        private Effect mTransparentEffect;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        public BarricadeAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner, bool register = true) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Barricade barricade = Owner as Barricade;
            PositionComponent positionComponent = barricade.PositionComponent;

            CurrentColumnIndex++;
            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
            }


            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width), 
                SourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), 
                SourceRectangle.Width, SourceRectangle.Height);

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);

            if (tile != null && tile.Type == TileType.Placed || tile.Type == TileType.Occupied)
            {
                Layer layer = LayerManager.GetLayerOfEntity(barricade);
                if (tile != null && tile.Type == TileType.Placed)
                {
                    spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
                }
                else
                    spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }

            if (SystemsManager.Instance.IsPaused)
                return;

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;


            if (barricade.ShowSmokeDamage)
            {
                mCurrentSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                mSmokeSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mCurrentSpawnParticleTimer <= 0.0f)
                {
                    mCurrentSpawnParticleTimer = kCurrentSpawnParticleTimer;

                    FireParticle fireParticle = new FireParticle(
                        (int)positionComponent.Position.X,
                        (int)positionComponent.Position.Y,
                        "Fire",
                        new Rectangle(0, 0, 32, 32),
                        0.13f,
                        1, 4);

                    if (mSmokeSpawnParticleTimer <= 0.0f)
                    {
                        mSmokeSpawnParticleTimer = kSmokeSpawnParticleTimer;

                        SmokeParticle smokeParticle = new SmokeParticle(
                                (int)fireParticle.PositionComponent.Position.X,
                                (int)fireParticle.PositionComponent.Position.Y - 4,
                           "Simple Black Smoke",
                           new Rectangle(0, 0, 32, 32),
                           0.2f,
                           1, 4);
                    }
                }
            }

            LayerManager.Layer = currentLayer;
        }
    }
}
