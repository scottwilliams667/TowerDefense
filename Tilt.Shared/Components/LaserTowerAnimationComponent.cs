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
using Tilt.Shared.Entities;

namespace Tilt.Shared.Components
{
    public class LaserTowerFireState : AnimationState
    {
        private Texture2D mProjectiles;

        private Rectangle mMuzzleRectangle;
        private Rectangle mProjectileSourceRectangle;
        private Rectangle mProjectileCurrentRectangle;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        private float mMuzzleInterval = 0.5f;
        private bool mCharging = true;
        private bool mFired = false;

        public LaserTowerFireState(string towerTexturePath, string projectilesTexturePath, Rectangle towerSourceRectangle, Rectangle projectileSourceRectangle,
            Rectangle muzzleRectangle, float interval, int rows, int columns, Entity owner) 
            : base(towerTexturePath, towerSourceRectangle, interval, rows, columns, owner)
        {
            CurrentColumnIndex = 0;
            CurrentRowIndex = 0;
            mMuzzleRectangle = muzzleRectangle;
            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * TileMap.TileWidth),
                SourceRectangle.Y + (CurrentRowIndex * TileMap.TileHeight), TileMap.TileWidth, TileMap.TileHeight);

            mProjectileSourceRectangle = projectileSourceRectangle;
            mProjectileCurrentRectangle = projectileSourceRectangle;
            mProjectiles = AssetOps.LoadSharedAsset<Texture2D>(projectilesTexturePath);
        }

        public override void Update()
        {
            Tower tower  = Owner as Tower;
            TowerData towerData = tower.Data as TowerData;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            LaserTowerAnimationComponent animationComponent = tower.AnimationComponent as LaserTowerAnimationComponent;
            if (animationComponent.TowerState != TowerState.Firing)
                return;

            Draw_();

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(mCharging)
            {
                if (CurrentTime <= 0.0f)
                {
                    CurrentRowIndex++;
                    CurrentTime = Interval;

                    mProjectileCurrentRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * mProjectileSourceRectangle.Width),
                        mProjectileSourceRectangle.Y + (CurrentRowIndex * mProjectileSourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);

                }

                if(CurrentRowIndex >= Rows)
                {
                    mCharging = false;
                    CurrentRowIndex = Rows - 1;
                    mProjectileCurrentRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * mProjectileSourceRectangle.Width),
                        mProjectileSourceRectangle.Y + (CurrentRowIndex * mProjectileSourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);
                    CurrentTime = mMuzzleInterval;

                    if (!mFired)
                    {
                        Projectile projectile = ProjectileFactory.Make(towerData.BulletType, 0,
                            (int)tower.CannonPositionComponent.X - 5,
                            (int)tower.CannonPositionComponent.Y + 5,
                            tower.CannonPositionComponent.Rotation, tower.Id);

                        if (projectile != null)
                        {
                            ProjectileData projectileData = projectile.Data;
                            projectileData.Damage = towerData.Damage;
                        }

                        tower.AmmoCapacityComponent.Ammo--;
                        mFired = true;
                    }

                }
            }
            else
            {
                if(CurrentTime <= 0.0f)
                {
                    CurrentRowIndex--;
                    CurrentTime = Interval;

                    mProjectileCurrentRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * mProjectileSourceRectangle.Width),
                       mProjectileSourceRectangle.Y + (CurrentRowIndex * mProjectileSourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);
                }

                if(CurrentRowIndex <= 0)
                {
                    animationComponent.TowerState = TowerState.Idle;
                    mCharging = true;
                    mFired = false;
                    CurrentRowIndex = 0;
                    CurrentTime = Interval;
                    mProjectileSourceRectangle = new Rectangle(mProjectileSourceRectangle.X + (CurrentColumnIndex * mProjectileSourceRectangle.Width),
                       mProjectileSourceRectangle.Y + (CurrentRowIndex * mProjectileSourceRectangle.Height), mProjectileSourceRectangle.Width, mProjectileSourceRectangle.Height);
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


            spriteBatch.Draw(mProjectiles, new Vector2(basePosition.X - 2, basePosition.Y + TileMap.TileHeight / 8), mProjectileCurrentRectangle, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

            if (SystemsManager.Instance.IsPaused)
                return;

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




        }





    }

    public class LaserTowerIdleState : AnimationState
    {
        private Effect mTransparentEffect;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        public LaserTowerIdleState(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            CurrentRowIndex = 0;
            CurrentColumnIndex = 0;
            CurrentRectangle = sourceRectangle;

            // mTransparentEffect = AssetOps.LoadSharedAsset<Effect>("TransparentEffect");
        }

        public override void Update()
        {

            Tower tower = Owner as Tower;
            LaserTowerAnimationComponent animationComponent = tower.AnimationComponent as LaserTowerAnimationComponent;
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

    //fix the hierarchy on this -- make members protected in TowerAnimationComponent.
    public class LaserTowerAnimationComponent : TowerAnimationComponentBase
    {
        public LaserTowerAnimationComponent(string towerTexturePath, string projectilesTexturePath, Rectangle towerSourceRectangle, Rectangle projectileSourceRectangle, 
            Rectangle muzzleRectangle, float idleInterval, float fireInterval, int idleRows, int idleColumns, int fireRows, int fireColumns, Entity owner) : 
            base(owner)
        {
            mTowerState = TowerState.Idle;
            mIdleState = new LaserTowerIdleState(towerTexturePath, towerSourceRectangle, idleInterval, idleRows, idleColumns, owner);
            mFireState = new LaserTowerFireState(towerTexturePath, projectilesTexturePath, towerSourceRectangle, projectileSourceRectangle, 
                muzzleRectangle, fireInterval, fireRows, fireColumns, owner);
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
            base.Register();
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
            mFireState.UnRegister();
            mIdleState.UnRegister();
            base.UnRegister();
        }

        public TowerState TowerState
        {
            get { return mTowerState; }
            set { mTowerState = value; }
        }

        public override void Update()
        {
            if(mTowerState == TowerState.Idle)
            {
                mState = mIdleState;
            }
            if(mTowerState == TowerState.Firing)
            {
                mState = mFireState;
            }

            mState.Update();
        }
    }
}
