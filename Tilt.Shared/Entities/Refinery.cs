using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Systems;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Entities
{
    public class Refinery 
        : Entity
        , IPlaceable
        , IBuildable
        , ICollideable
        , IHealable
        , IShieldable
        , IDamageable
    {
        private PositionComponent mPositionComponent;
        private RefineryRenderComponent mRenderComponent;
        private BoundsCollisionComponent mBoundsCollisionComponent;
        private RefineryHarvestComponent mHarvestComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private ShieldRenderComponent mShieldRenderComponent;
        private SimpleAudioComponent mAudioComponent;
        private IData mData;
        private bool mIsEnabled;
        private bool mShowSmokeDamage;

        public Refinery(string texturePath, string waveTexturePath, int x, int y, Rectangle sourceRectangle, Rectangle waveRectangle, Rectangle addOnRectangle, int rows, int columns, float interval, IData data)
        {
            mRenderComponent = new RefineryRenderComponent(texturePath, waveTexturePath, sourceRectangle, waveRectangle, addOnRectangle, rows, columns, interval, this);
            mPositionComponent = new PositionComponent(x,y,this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            mBoundsCollisionComponent = new RefineryCollisionComponent(
                new Rectangle(x,y, TileMap.TileWidth, TileMap.TileHeight), this);
            mHarvestComponent = new RefineryHarvestComponent(this);
            mHealthComponent = new ResistantHealthComponent((data as RefineryData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            mAudioComponent = new SimpleAudioComponent("sfx_refinery_wave", this);
            mData = data;
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            mPositionComponent.UnRegister();
            mBoundsCollisionComponent.UnRegister();
            mHarvestComponent.UnRegister();
            mHealthComponent.UnRegister();
            mShieldRenderComponent.UnRegister();
            mAudioComponent.UnRegister();
            mData = null;
            base.UnRegister();
        }

        public ObjectType ObjectType { get { return ObjectType.Refinery; } }

        public IData Data
        {
            get { return mData; }
            set { mData = value; }
                
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public RefineryRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
            set { mRenderComponent = value; }
        }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get { return mBoundsCollisionComponent; }
            set { mBoundsCollisionComponent = value; }
        }

        public RefineryHarvestComponent HarvestComponent
        {
            get { return mHarvestComponent;}
            set { mHarvestComponent = value; }
        }

        public HealthComponent HealthComponent
        {
            get { return mHealthComponent; }
            set { mHealthComponent = value; }
        }

        public HealthRenderComponent HealthRenderComponent
        {
            get { return mHealthRenderComponent; }
            set { mHealthRenderComponent = value; }
        }

        public ShieldRenderComponent ShieldRenderComponent
        {
            get { return mShieldRenderComponent; }
            set { mShieldRenderComponent = value; }
        }

        public SimpleAudioComponent AudioComponent
        {
            get { return mAudioComponent;}
            set { mAudioComponent = value; }
        }

        public bool Enabled 
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; } 
        }

        public bool ShowSmokeDamage
        {
            get { return mShowSmokeDamage; }
            set { mShowSmokeDamage = value; }
        }

    }

    public class RefineryRenderComponent : AnimationComponent
    {
        private Rectangle mTowerRectangle;
        private Texture2D mWaveTexture;

        private Rectangle mAddOnRectangle;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;

        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;

        
        public RefineryRenderComponent(string texturePath, string waveTexturePath, Rectangle sourceRectangle, Rectangle waveRectangle, Rectangle addOnRectangle, int rows, int columns, float interval,  Entity owner, bool register = true)
            : base(texturePath, waveRectangle, interval, rows, columns, owner)
        {
            mTowerRectangle = sourceRectangle;
            mWaveTexture = AssetOps.LoadSharedAsset<Texture2D>(waveTexturePath);
            mAddOnRectangle = addOnRectangle;
        }

        public override void Update()
        {
            Refinery refinery = Owner as Refinery;
            PositionComponent positionComponent = refinery.PositionComponent;
            RefineryHarvestComponent harvestComponent = refinery.HarvestComponent;


            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);

            if (tile != null && tile.Type == TileType.Placed)
            {
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), mTowerRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }
            else
            {
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), mTowerRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

                ResourcePile resourcePile = harvestComponent.CurrentResourcePile;
                List<RefineryAddOn> refineryAddOns = harvestComponent.RefineryAddOns;

                if(resourcePile != null)
                {


                    Vector2 resourcePosition = resourcePile.PositionComponent.Position;

                    double angle = GeometryOps.AngleBetweenTwoVectors(refinery.PositionComponent.Position, resourcePosition);
                    double dist = Vector2.Distance(refinery.PositionComponent.Position, resourcePosition);

                    decimal sizeY = Decimal.Divide((decimal)dist, mWaveTexture.Height);

                    
                    spriteBatch.Draw(mWaveTexture, refinery.PositionComponent.Position + new Vector2(16, 16), CurrentRectangle, Color.White, (float)angle + (float)(Math.PI / 2), new Vector2(8, 0), new Vector2(1.0f, (float)sizeY), SpriteEffects.None, 0.29f);

                    if (refineryAddOns != null)
                    {

                        foreach (RefineryAddOn refineryAddOn in refineryAddOns)
                        {
                            double angle2 = GeometryOps.AngleBetweenTwoVectors(refinery.PositionComponent.Position, refineryAddOn.PositionComponent.Position);
                            double dist2 = Vector2.Distance(refinery.PositionComponent.Position, refineryAddOn.PositionComponent.Position);

                            decimal sizeY2 = Decimal.Divide((decimal)dist2, mWaveTexture.Height);

                            Rectangle rectangle = new Rectangle(
                                mAddOnRectangle.X + CurrentColumnIndex * mAddOnRectangle.Width,
                                mAddOnRectangle.Y + CurrentRowIndex * mAddOnRectangle.Height,
                                mAddOnRectangle.Width,
                                mAddOnRectangle.Height);

                            spriteBatch.Draw(mWaveTexture, refinery.PositionComponent.Position + new Vector2(16, 16), rectangle, Color.White, (float)angle2 + (float)(Math.PI / 2), new Vector2(8, 0), new Vector2(1.0f, (float)sizeY2), SpriteEffects.None, 0.29f);

                        }
                    }

                    if (SystemsManager.Instance.IsPaused)
                        return;

                    if(CurrentColumnIndex >= Columns)
                    {
                        CurrentColumnIndex = 0;
                        CurrentTime = Interval;
                    }

                    if(CurrentTime <= 0.0f)
                    {
                        CurrentColumnIndex++;
                        CurrentTime = Interval;
                    }

                    CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex * SourceRectangle.Width, SourceRectangle.Y + CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
                    
                }

                if (SystemsManager.Instance.IsPaused)
                    return;

                Layer currentLayer = LayerManager.Layer;
                Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

                LayerManager.Layer = gameLayer;


                if (refinery.ShowSmokeDamage)
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

            //spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), mSourceRectangle, Color.White,
            //    0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

        }
    }

    public class RefineryCollisionComponent : BoundsCollisionComponent
    {
        public RefineryCollisionComponent(Rectangle bounds, Entity owner)
            : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Refinery refinery = Owner as Refinery;
            PositionComponent positionComponent = refinery.PositionComponent;

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);

            if (tile.IsTowerPlaced)
                return;

            foreach (int cell in Cells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);

                foreach (CollisionComponent component in nearbyComponents)
                {
                    if (!(component.Owner is Unit))
                        continue;

                    Unit unit = component.Owner as Unit;
                    UnitPositionComponent unitPosition = unit.PositionComponent;
                    UnitAnimationComponent animationComponent = unit.RenderComponent;
                    UnitData unitData = unit.Data;

                    HealthComponent healthComponent = refinery.HealthComponent;

                    if (Vector2.Distance(positionComponent.Position, unitPosition.Position) < TileMap.TileWidth)
                    {
                        animationComponent.IsAttacking = true;
                        animationComponent.EntityState = EntityState.Idle;
                        animationComponent.AttackEntityId = Owner.Id;
                        unitPosition.Speed = 0;


                        EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                        {
                            Play = true,
                            SoundEffect = "sfx_enemy_attack"
                        });

                    }

                    if (healthComponent.Health <= 0)
                    {
                        EventSystem.EnqueueEvent(EventType.TowerDestroyed, Owner, null);
                    }
                }
            }

        }
    }
}
