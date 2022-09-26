using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Entities
{
    public class UnitSpawnPoint : Entity
    {
        private PositionComponent mPositionComponent;
        private RenderComponent mRenderComponent;

        public UnitSpawnPoint(int x, int y, Rectangle sourceRectangle, int rows, int columns, float interval, string texturePath)
        {
            mPositionComponent = new PositionComponent(x,y, this);
            mRenderComponent = new UnitSpawnPointRenderComponent(texturePath, sourceRectangle, rows, columns, interval, this);
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public RenderComponent RenderComponent
        {
            get { return mRenderComponent; }
            set { mRenderComponent = value; }
        }

    }

    public class UnitSpawnPointRenderComponent : AnimationComponent
    {
        private Texture2D mPulseTexture;
        public UnitSpawnPointRenderComponent(string texturePath, Rectangle sourceRectangle, int rows, int columns, float interval, Entity owner, bool register = true) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            mPulseTexture = AssetOps.LoadSharedAsset<Texture2D>("pulseanimation");
        }

        public override void Update()
        {
            UnitSpawnPoint unitSpawnPoint = Owner as UnitSpawnPoint;

            PositionComponent positionComponent = unitSpawnPoint.PositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            spriteBatch.Draw(mPulseTexture, positionComponent.Position - new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), CurrentRectangle, Color.White,
                0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.65f);

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.7f);

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }

            if (CurrentColumnIndex >= Columns)
                CurrentColumnIndex = 0;

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            

            
        }
    }
}
