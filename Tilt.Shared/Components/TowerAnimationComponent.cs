using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Components
{
    public enum TowerState
    {
        Idle,
        Firing
    }

    public class TowerIdleState : AnimationState
    {
        private Effect mTransparentEffect;
        
        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;

        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        public TowerIdleState(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            CurrentRowIndex = 0;
            CurrentColumnIndex = 0;
            CurrentRectangle = sourceRectangle;

            //mTransparentEffect = AssetOps.LoadAsset<Effect>("TransparentEffect");
        }

        public override void Update()
        {

            Tower tower = Owner as Tower;
            TowerAnimationComponent animationComponent = tower.AnimationComponent as TowerAnimationComponent;
            if (animationComponent.TowerState != TowerState.Idle)
                return;

            Draw();
        }

        private void Draw()
        {
            Tower tower = Owner as Tower;
            PositionComponent basePosition = tower.PositionComponent;
            TowerAimerPositionComponent cannonPosition = tower.CannonPositionComponent;
            CooldownComponent cooldownComponent = tower.CooldownComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            TileNode tile = TileMap.GetTileForPosition(basePosition.X, basePosition.Y);

            if (tile != null && tile.Type == TileType.Placed)
            {
                spriteBatch.Draw(mTexture, new Vector2(basePosition.X, basePosition.Y), CurrentRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }
            else
                spriteBatch.Draw(mTexture, new Vector2(basePosition.X, basePosition.Y), CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

            if (SystemsManager.Instance.IsPaused)
                return;

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;

            if (tower.ShowSmokeDamage)
            {
                mCurrentSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                mSmokeSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mCurrentSpawnParticleTimer <= 0.0f)
                {
                    mCurrentSpawnParticleTimer = kCurrentSpawnParticleTimer;

                    FireParticle fireParticle = new FireParticle(
                        (int)basePosition.Position.X,
                        (int)basePosition.Position.Y,
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

    public class TowerFireState : AnimationState
    {
        private Random mMuzzleFlashRandom = new Random();

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;

        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        private bool mMuzzleShown;
        private bool mFired;
        private bool mBurstMode;
        private bool mDisableMuzzleFire = false;

        private int mShotsFired = 0;

        private Texture2D mProjectiles;

        private Rectangle mMuzzleRectangle;
        private Rectangle mProjectileSourceRectangle;
        private Rectangle mProjectileCurrentRectangle;

        public TowerFireState(string towerTexturePath, string projectilesTexturePath, Rectangle towerSourceRectangle, Rectangle projectileSourceRectangle, Rectangle muzzleSourceRectangle, 
            float interval, int rows, int columns, Entity owner) 
            : base(towerTexturePath, towerSourceRectangle, interval, rows, columns, owner)
        {
            CurrentColumnIndex = 0;
            CurrentRowIndex = 0;
            mMuzzleRectangle = muzzleSourceRectangle;
            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * TileMap.TileWidth),
            SourceRectangle.Y + (CurrentRowIndex * TileMap.TileHeight), TileMap.TileWidth, TileMap.TileHeight);
            mProjectileSourceRectangle = projectileSourceRectangle;
            mProjectileCurrentRectangle = projectileSourceRectangle;
            //mBurstMode = burstMode;
            mProjectiles = AssetOps.LoadSharedAsset<Texture2D>(projectilesTexturePath);

        }

        public bool DisableMuzzleFire
        {
            get { return mDisableMuzzleFire; }
            set { mDisableMuzzleFire = value; }
        }
        
        public override void Update()
        {
            Tower tower = Owner as Tower;
            TowerData towerData = tower.Data as TowerData;

            TowerAnimationComponent animationComponent = tower.AnimationComponent as TowerAnimationComponent;
            if (animationComponent.TowerState != TowerState.Firing)
                return;

            Draw_();

            if (SystemsManager.Instance.IsPaused)
                return;


            if (mMuzzleShown && !mFired)
            {
                for (int i = 0; i < towerData.ShotsPerFire; i++)
                {
                    Projectile projectile = ProjectileFactory.Make(towerData.BulletType, 0,
                        (int)tower.CannonPositionComponent.X,
                        (int)tower.CannonPositionComponent.Y,
                        tower.CannonPositionComponent.Rotation, animationComponent.TargettedEntityId);

                    if (projectile != null)
                    {
                        ProjectileData projectileData = projectile.Data;
                        projectileData.Damage = towerData.Damage;
                    }

                    tower.AmmoCapacityComponent.Ammo--;
                    mFired = true;

                    //only fire one shot if in burst mode.
                    //allow the TowerAimerPositionComponent to handle the burst again. (not ideal)
                    TowerAimerPositionComponent aimerPositionComponent = tower.CannonPositionComponent;
                    if(aimerPositionComponent.BurstMode)
                    {
                        break;
                    }

                }
            }

        }

        private void Draw_()
        {
            Tower tower = Owner as Tower;
            PositionComponent basePosition = tower.PositionComponent;
            TowerAimerPositionComponent cannonPosition = tower.CannonPositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            spriteBatch.Draw(mTexture, new Vector2(basePosition.X, basePosition.Y), CurrentRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);
            
            int randomIndex = -1;
            if (!mMuzzleShown) //50% of time attempt to show muzzle
            {
                randomIndex = mMuzzleFlashRandom.Next(0, 2);

                if (randomIndex == 0)
                {
                    float layerDepth = (mDisableMuzzleFire) ? 0.0f : 1.0f; //hide muzzle fire behind everything if its disabled by AOE towers
                    spriteBatch.Draw(mProjectiles, new Vector2(basePosition.X, basePosition.Y), mMuzzleRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, layerDepth);
                }
                else
                {
                    randomIndex = -1;
                }

                mMuzzleShown = true; //dont let the muzzle flash after first frame if failed to land within 50%

            }
            
            if(mMuzzleShown && randomIndex == -1)
            {
                float layerDepth = (mDisableMuzzleFire) ? 0.0f : 1.0f;
                spriteBatch.Draw(mProjectiles, new Vector2(basePosition.X, basePosition.Y), mProjectileCurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, layerDepth);

                CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (CurrentTime <= 0.0f)
                {
                    CurrentColumnIndex++;
                    CurrentTime = Interval;
                }

                mProjectileCurrentRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width),
                    mProjectileSourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);
            }


            if(CurrentColumnIndex >= Columns)
            {
                TowerAnimationComponent animationComponent = tower.AnimationComponent as TowerAnimationComponent;
                animationComponent.TowerState = TowerState.Idle;
                mMuzzleShown = false;
                mFired = false;
                CurrentColumnIndex = 0;
                mProjectileCurrentRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width),
                    mProjectileSourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);

            }

            if (SystemsManager.Instance.IsPaused)
                return;

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;

            if (tower.ShowSmokeDamage)
            {
                mCurrentSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                mSmokeSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mCurrentSpawnParticleTimer <= 0.0f)
                {
                    mCurrentSpawnParticleTimer = kCurrentSpawnParticleTimer;

                    FireParticle fireParticle = new FireParticle(
                        (int)basePosition.Position.X,
                        (int)basePosition.Position.Y,
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

    public class TowerAnimationComponent : TowerAnimationComponentBase
    {
        private ulong mTargettedEntityId;

        public TowerAnimationComponent(string towerTexturePath, string projectilesTexturePath, Rectangle towerSourceRectangle, Rectangle projectileSourceRectangle,
            Rectangle muzzleRectangle, float idleInterval, float fireInterval, 
            int idleRows, int idleColumns, int fireRows, int fireColumns, Entity owner) : base(owner)
        {
            mTowerState = TowerState.Idle;
            mIdleState = new TowerIdleState(towerTexturePath, towerSourceRectangle, idleInterval, idleRows, idleColumns, owner);
            mFireState = new TowerFireState(towerTexturePath, projectilesTexturePath, towerSourceRectangle, projectileSourceRectangle, muzzleRectangle, fireInterval, fireRows, fireColumns, owner);
            mState = mIdleState;

        }

        public TowerFireState FireState
        {
            get { return mFireState as TowerFireState; }
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
        }


        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
            mFireState.UnRegister();
            mIdleState.UnRegister();
            base.UnRegister();
        }

        public override void Update()
        {
            if (mTowerState == TowerState.Idle)
            {
                mState = mIdleState;
            }
            if (mTowerState == TowerState.Firing)
            {
                mState = mFireState;
            }
            mState.Update();
            
        }
    }
}
