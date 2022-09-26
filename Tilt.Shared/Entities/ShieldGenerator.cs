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
    public class ShieldGenerator 
        : Entity
        , IPlaceable
        , ICollideable
        , IBuildable
        , IHealable
        , IDamageable
    {
        private IData mData;
        private bool mIsEnabled;
        private bool mShowSmokeDamage;
        private ShieldGeneratorAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;
        private ShieldGeneratorAddOnComponent mAddOnComponent;
        private BoundsCollisionComponent mBoundsCollisionComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private FieldOfViewRenderComponent mFieldOfViewRenderComponent;
       // private FireParticleAnimationComponent mFireParticleAnimationComponent;

        public ShieldGenerator(string texturePath, string fireTexturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, IData data)
        {
            mAnimationComponent = new ShieldGeneratorAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mPositionComponent = new PositionComponent(x,y,this, new Vector2(x+ TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            mBoundsCollisionComponent = new ShieldGeneratorCollisionComponent(new Rectangle(x,y,TileMap.TileWidth, TileMap.TileHeight), this);
            mHealthComponent = new ResistantHealthComponent((data as ShieldGeneratorData).Health, this);
            mAddOnComponent = new ShieldGeneratorAddOnComponent((data as ShieldGeneratorData).FieldOfView, this);
            mFieldOfViewRenderComponent = new FieldOfViewRenderComponent(32, Color.Red, this);
            mData = data;
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mPositionComponent.UnRegister();
            mBoundsCollisionComponent.UnRegister();
            mHealthComponent.UnRegister();
            mAddOnComponent.UnRegister();
            mData = null;
            base.UnRegister();
        }


        public ObjectType ObjectType { get { return ObjectType.ShieldGenerator; } }
        public IData Data { get { return mData; } set { mData = value; } }
        public PositionComponent PositionComponent 
        { 
            get {return mPositionComponent; } 
            set { mPositionComponent = value;} 
        }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get {return mBoundsCollisionComponent;}
            set { mBoundsCollisionComponent = value; }
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

    public class ShieldGeneratorAnimationComponent : AnimationComponent
    {
        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;

        public ShieldGeneratorAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            ShieldGenerator shieldGenerator = Owner as ShieldGenerator;
            PositionComponent positionComponent = shieldGenerator.PositionComponent;

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);
            if (tile != null && tile.Type == TileType.Placed)
            {
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }
            else
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

            if (SystemsManager.Instance.IsPaused)
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

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;

            if (shieldGenerator.ShowSmokeDamage)
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

    public class ShieldGeneratorAddOnComponent : EventComponent
    {
        private float mFieldOfView;
        public ShieldGeneratorAddOnComponent(float fieldOfView, Entity owner, bool register = true) : base(owner, register)
        {
            mFieldOfView = fieldOfView;
        }

        public override void Register()
        {
            EventSystem.SubScribe(EventType.TowerAdded, OnTowerAdded_);
            base.Register();
        }

        public override void UnRegister()
        {
            EventSystem.UnSubScribe(EventType.TowerAdded, OnTowerAdded_);
            QueryTowers_(false);
            base.UnRegister();
        }


        private void QueryTowers_(bool addIncrease)
        {
            ShieldGenerator addOn = Owner as ShieldGenerator;
            CollisionComponent collisionComponent = addOn.BoundsCollisionComponent;
            PositionComponent positionComponent = addOn.PositionComponent;

            if (collisionComponent.Cells == null ||
                collisionComponent.Cells.Count == 0)
                return;

            List<int> surroundingCells = CollisionHelper.GetSurroundingCells(collisionComponent.Cells.First());
            foreach (int cell in surroundingCells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);
                foreach (CollisionComponent component in nearbyComponents)
                {
                    if (!(component.Owner is IShieldable))
                        continue;

                    IShieldable shieldable = component.Owner as IShieldable;
                    IPlaceable placeable = component.Owner as IPlaceable;

                    if (shieldable == null || placeable == null)
                        return;

                    if (Vector2.Distance(positionComponent.Origin, placeable.PositionComponent.Origin) < mFieldOfView)
                    {
                        shieldable.ShieldRenderComponent.IsEnabled = addIncrease;
                    }
                }
            }
        }

        private void OnTowerAdded_(object sender, IGameEventArgs e)
        {
            ShieldGenerator sg = Owner as ShieldGenerator;
            PositionComponent positionComponent = sg.PositionComponent;
            CollisionComponent collisionComponent = sg.BoundsCollisionComponent;
            Vector2 origin = positionComponent.Origin;

            if (sender is List<IPlaceable>)
            {
                List<IPlaceable> objects = sender as List<IPlaceable>;
                if (objects.Any(o => o == Owner))
                {
                    QueryTowers_(true);
                }
                else
                {
                    foreach (IPlaceable obj in objects)
                    {
                        if (Vector2.Distance(origin, obj.PositionComponent.Origin) < mFieldOfView && obj is Tower)
                        {
                            IShieldable shieldable = obj as IShieldable;
                            shieldable.ShieldRenderComponent.IsEnabled = true;
                        }
                    }
                }

            }
            else
            {
                IPlaceable placeable = sender as IPlaceable;
                if (Vector2.Distance(origin, placeable.PositionComponent.Origin) < mFieldOfView && placeable is Tower)
                {
                    IShieldable shieldable = placeable as IShieldable;
                    shieldable.ShieldRenderComponent.IsEnabled = true;
                }
            }


        }
    }

    public class ShieldGeneratorCollisionComponent : BoundsCollisionComponent
    {
        public ShieldGeneratorCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            ShieldGenerator shieldGenerator = Owner as ShieldGenerator;
            PositionComponent positionComponent = shieldGenerator.PositionComponent;

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

                    HealthComponent healthComponent = shieldGenerator.HealthComponent;

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

    public class ShieldRenderComponent : RenderComponent
    {
        private bool mIsEnabled;
        private bool mPreviousIsEnabled;
        private bool mIsDamaged;

        private const float mEndScale = 1.0f;
        private const float mStartScale = 0.0f;
        private float mScale = 0.0f;
        private float mScaleInterval = 0.09f;

        private float mStretchScale = 1.0f; //draw image larger for base
        private int mPadding;

        private Texture2D mDamageTexture;
        private const float kDamageDrawTime = 0.75f;
        private float mDamageDrawTime;
        private float mDamageAlpha = 1.0f;




        public ShieldRenderComponent(string texturePath, string damageTexture, Entity owner, float stretchScale  = 1.0f,  int padding = 0,  bool register = true) : base(texturePath, owner, register)
        {
            mDamageTexture = AssetOps.LoadSharedAsset<Texture2D>(damageTexture);
            //HACK - scale up for base and add extra padding
            mStretchScale = stretchScale;
            mPadding = padding;
        }

        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set
            {
                if (mIsEnabled == value)
                    return;

                if(value)
                {
                    mScale = mStartScale;
                }

                mIsEnabled = value;
            }
        }

        public bool IsDamaged
        {
            get { return mIsDamaged; }
            set
            {
                mIsDamaged = value;

                if(value)
                {
                    mDamageDrawTime = kDamageDrawTime;
                }
            }
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            PositionComponent positionComponent = (Owner as IPlaceable).PositionComponent;

            if (mIsEnabled)
            {


                mScale = (mScale >= mEndScale) ? 1.0f : mScale + mScaleInterval;



                if(mIsDamaged)
                {
                    mDamageDrawTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    mDamageAlpha = Math.Max(0.3f, mDamageDrawTime / kDamageDrawTime);

                    spriteBatch.Draw(mDamageTexture, positionComponent.Position - new Vector2(24, 0) + new Vector2(mTexture.Width / 2, mTexture.Height / 2) + new Vector2(mPadding, mPadding), null, Color.White * mDamageAlpha, 0.0f, new Vector2(mTexture.Width / 2, mTexture.Height / 2), 1.0f * mScale * mStretchScale, SpriteEffects.None, 0.7f);

                    if (mDamageDrawTime <= 0.0f)
                    {
                        mIsDamaged = false;
                    }
                }
                else
                {
                    mDamageDrawTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    mDamageDrawTime = Math.Min(mDamageDrawTime, kDamageDrawTime);
                    mDamageAlpha = Math.Min(1.0f, mDamageDrawTime / kDamageDrawTime);

                    spriteBatch.Draw(mTexture, positionComponent.Position - new Vector2(24, 0) + new Vector2(mTexture.Width / 2, mTexture.Height / 2) + new Vector2(mPadding, mPadding), null, Color.White * mDamageAlpha, 0.0f, new Vector2(mTexture.Width / 2, mTexture.Height / 2), 1.0f * mScale * mStretchScale, SpriteEffects.None, 0.7f);    
                }


                
            }
            else if(!mIsEnabled && mScale > 0.0f)
            {
                mScale = (mScale <= mStartScale) ? 0.0f : mScale - mScaleInterval;
                spriteBatch.Draw(mTexture, positionComponent.Position - new Vector2(24, 0) + new Vector2(mTexture.Width / 2, mTexture.Height / 2) + new Vector2(mPadding, mPadding), null, Color.White, 0.0f, new Vector2(mTexture.Width / 2, mTexture.Height / 2), 1.0f * mScale * mStretchScale, SpriteEffects.None, 0.7f);
            }
                
        }
    }
}
