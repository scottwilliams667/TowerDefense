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
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Components
{
    public class UnitAnimationComponent : AnimationComponent
    {
        private bool mIsDamaged;
        private bool mIsAttacking;

        private float mAttackTime;
        private float mAttackTextureInterval = 0.2f;
        private bool mShowAttackTexture = true;

        private Texture2D mAttackTexture;
        private Texture2D mDamageTexture;
        private Vector2 mLastDirection;
        private EntityState mEntityState;
        private float mDamageAlpha;
        private float kDamageAlphaHurtIncrement = 0.03f;
        private float kDamageAlphaDeathIncrement = 0.04f;

        private ulong mAttackEntityId;
        


        public UnitAnimationComponent(string texturePath, string damageTexturePath, string attackTexturePath, float attackTime, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            mEntityState = EntityState.Walking;
            mDamageTexture = AssetOps.LoadSharedAsset<Texture2D>(damageTexturePath);
            mAttackTexture = AssetOps.LoadSharedAsset<Texture2D>(attackTexturePath);
            mAttackTime = attackTime;
        }

        public EntityState EntityState
        {
            get { return mEntityState; }
            set { mEntityState = value; }
        }

        public bool IsDamaged
        {
            get { return mIsDamaged; }
            set { mIsDamaged = value; }
        }

        public bool IsAttacking
        {
            get { return mIsAttacking; }
            set { mIsAttacking = value; }
        }

        public ulong AttackEntityId
        {
            get { return mAttackEntityId; }
            set { mAttackEntityId = value; }
        }

        public override void Update()
        {
            Unit unit = Owner as Unit;
            UnitPositionComponent position = unit.PositionComponent;
            HealthComponent healthComponent = unit.HealthComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            SpriteEffects spriteEffects = (position.Direction.X == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!SystemsManager.Instance.IsPaused)
            {

                if (EntityState == EntityState.Idle)
                {
                    CurrentTime = 0.0f;
                    CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex*SourceRectangle.Width,
                        SourceRectangle.Y + CurrentRowIndex*SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);
                }

                


                CurrentTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (CurrentTime > Interval && EntityState == EntityState.Walking)
                {
                    CurrentTime = 0.0f;

                    CurrentColumnIndex++;

                    if(CurrentColumnIndex >= Columns)
                    {
                        CurrentColumnIndex = 0;
                    }


                    CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex*SourceRectangle.Width,
                        SourceRectangle.Y + CurrentRowIndex*SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

                }

                if (!mIsDamaged)
                    mDamageAlpha = 1.0f;
                else if (mIsDamaged && healthComponent.Health > 0)
                    mDamageAlpha -= kDamageAlphaHurtIncrement;
                else if (mIsDamaged && healthComponent.Health <= 0)
                    mDamageAlpha -= kDamageAlphaDeathIncrement;

                if(mIsAttacking)
                {
                    mAttackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    mShowAttackTexture = mAttackTime % mAttackTextureInterval > mAttackTextureInterval / 2;
                }

                
            }

            
            

            if(!mIsDamaged && !mIsAttacking)
                spriteBatch.Draw(mTexture, position.Position, CurrentRectangle, Color.White, 0, Vector2.Zero, 1.0f, spriteEffects, 0.2f);
            else if(mIsAttacking && mShowAttackTexture)
                spriteBatch.Draw(mAttackTexture, position.Position, CurrentRectangle, Color.White, 0, Vector2.Zero, 1.0f, spriteEffects, 0.2f);
            else if(mIsAttacking && !mShowAttackTexture)
                spriteBatch.Draw(mTexture, position.Position, CurrentRectangle, Color.White, 0, Vector2.Zero, 1.0f, spriteEffects, 0.2f);
            else
                spriteBatch.Draw(mDamageTexture, position.Position, CurrentRectangle, Color.White * mDamageAlpha, 0, Vector2.Zero, 1.0f, spriteEffects, 0.2f);



            if (mAttackTime <= 0.0f && mIsAttacking)
            {
                unit.UnRegister();
                EventSystem.EnqueueEvent(EventType.UnitDestroyed, unit, null);
                
                AreaOfEffectParticle particle = new AreaOfEffectParticle(
                    (int)unit.PositionComponent.Position.X + TileMap.TileWidth * 3/4,
                    (int)unit.PositionComponent.Position.Y + TileMap.TileHeight * 3/4,
                    "particles_strip7",
                    new Rectangle(96, 0, 32, 32), 1, 4, 0.16f);

                Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
                IHealable entity = (IHealable)gameLayer.EntitySystem.GetEntityById(mAttackEntityId);

                if (entity == null)
                    return;

                HealthComponent entityHealthComponent = entity.HealthComponent;
                entityHealthComponent.Health -= unit.Data.Damage;
                
                if(entityHealthComponent.HealthPercentage < 0.8f)
                {
                    IDamageable damageable = entity as IDamageable;
                    if(damageable != null)
                    {
                        damageable.ShowSmokeDamage = true;
                    }
                }

                if (entityHealthComponent.Health <= 0)
                {
                    IPlaceable placeable = entity as IPlaceable;
                    Entity e = entity as Entity;
                    e.UnRegister();
                    EventSystem.EnqueueEvent(EventType.TowerDestroyed, entity, null);

                    EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                    {
                        SoundEffect = "sfx_tower_explosion",
                        Play = true
                    });
                    
                    ExplosionParticle explosionParticle = new ExplosionParticle(
                        (int)placeable.PositionComponent.Position.X,
                        (int)placeable.PositionComponent.Position.Y,
                        "Explosion 1",
                        new Rectangle(0,0,32,32),
                        0.08f,
                        2, 4);

                    ExplosionParticle explosionParticle2 = new ExplosionParticle(
                        (int)placeable.PositionComponent.Position.X,
                        (int)placeable.PositionComponent.Position.Y,
                        "Explosion 2",
                        new Rectangle(0, 0, 32, 32),
                        0.08f,
                        2, 4);

                    ExplosionParticle explosionParticle3 = new ExplosionParticle(
                        (int)placeable.PositionComponent.Position.X,
                        (int)placeable.PositionComponent.Position.Y,
                        "Explosion 1",
                        new Rectangle(0, 0, 32, 32),
                        0.08f,
                        2, 4);

                    ExplosionParticle explosionParticle4 = new ExplosionParticle(
                        (int)placeable.PositionComponent.Position.X,
                        (int)placeable.PositionComponent.Position.Y,
                        "Explosion 2",
                        new Rectangle(0, 0, 32, 32),
                        0.08f,
                        2, 4);
                    ExplosionParticle explosionParticle5 = new ExplosionParticle(
                        (int)placeable.PositionComponent.Position.X,
                        (int)placeable.PositionComponent.Position.Y,
                        "Explosion 1",
                        new Rectangle(0, 0, 32, 32),
                        0.08f,
                        2, 4);

                    
                    explosionParticle2.AnimationComponent.CurrentColumnIndex = 5;
                    explosionParticle2.AnimationComponent.CurrentRowIndex = -1;
                    explosionParticle2.AnimationComponent.CurrentTime = 0.16f;

                    explosionParticle3.AnimationComponent.CurrentColumnIndex = 5;
                    explosionParticle3.AnimationComponent.CurrentRowIndex = -1;
                    explosionParticle3.AnimationComponent.CurrentTime = 0.33f;

                    explosionParticle4.AnimationComponent.CurrentColumnIndex = 5;
                    explosionParticle4.AnimationComponent.CurrentRowIndex = -1;
                    explosionParticle4.AnimationComponent.CurrentTime = 0.5f;

                    explosionParticle5.AnimationComponent.CurrentColumnIndex = 5;
                    explosionParticle5.AnimationComponent.CurrentRowIndex = -1;
                    explosionParticle5.AnimationComponent.CurrentTime = 0.66f;

                }


                IShieldable shieldable = entity as IShieldable;

                if (shieldable == null)
                    return;

                if(shieldable.ShieldRenderComponent.IsEnabled)
                {
                    shieldable.ShieldRenderComponent.IsDamaged = true;
                }
                else
                {
                    //show regular damage
                }


            }
            else if (mDamageAlpha <= 0.0f && healthComponent.Health <= 0)
            {
                unit.UnRegister();
                EventSystem.EnqueueEvent(EventType.UnitDestroyed, unit, null);
            }



        }
    }
}
