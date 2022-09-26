using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Entities
{
    public class RefineryAddOn : AddOn
    {
        private RefineryAddOnAnimationComponent mRenderComponent;
        private RefineryAddOnWaveAnimationComponent mWaveAnimationComponent;

        public RefineryAddOn(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, IData data) : base(AddOnType.Refinery)
        {
            PositionComponent = new PositionComponent(x, y, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new AddOnCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            mRenderComponent = new RefineryAddOnAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            HealthComponent = new ResistantHealthComponent((data as AddOnData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            mWaveAnimationComponent = new RefineryAddOnWaveAnimationComponent("refinerywave", new Rectangle(0, 0, 16, 32), 0.33f, 1, 3, this);
            Data = data;
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            PositionComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            HealthComponent.UnRegister();
            ShieldRenderComponent.UnRegister();
            mWaveAnimationComponent.UnRegister();
            Data = null;

            base.UnRegister();
        }
    }

    public class RefineryAddOnAnimationComponent : AnimationComponent
    {
        private int kRadius = 120;
        private Effect mTransparentEffect;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        public RefineryAddOnAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner, bool register = true) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
           // mTransparentEffect = AssetOps.LoadAsset<Effect>("TransparentEffect");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            RefineryAddOn refineryAddOn = Owner as RefineryAddOn;
            PositionComponent positionComponent = refineryAddOn.PositionComponent;

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);

            if (tile != null && tile.Type == TileType.Placed)
            {
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }
            else
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

            if (SystemsManager.Instance.IsPaused)
                return;

            bool isRefining = IsNearbyRefineryWorking_();

            if (!isRefining)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentTime = Interval;
                CurrentColumnIndex++;
            }

            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
            }

            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width), SourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), SourceRectangle.Width, SourceRectangle.Height);

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;


            if (refineryAddOn.ShowSmokeDamage)
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


        private bool IsNearbyRefineryWorking_()
        {
            List<int> cells = CollisionHelper.GetCells((Owner as RefineryAddOn).BoundsCollisionComponent);

            foreach (int cell in cells)
            {
                List<CollisionComponent> components = CollisionHelper.GetNearby(cell);
                List<CollisionComponent> refineryComponents = components.Where(c => c.Owner is Refinery).ToList();

                foreach (CollisionComponent component in refineryComponents)
                {
                    Refinery refinery = component.Owner as Refinery;
                    if (Vector2.Distance((Owner as RefineryAddOn).PositionComponent.Origin, refinery.BoundsCollisionComponent.Origin) <= kRadius && refinery.HarvestComponent.IsRefining)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class RefineryAddOnWaveAnimationComponent : AnimationComponent
    {
        public RefineryAddOnWaveAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            RefineryAddOn refineryAddOn = Owner as RefineryAddOn;
            PositionComponent positionComponent = refineryAddOn.PositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);
        }
    }
}
