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

namespace Tilt.EntityComponent.Entities
{
    public class Base 
        : Entity
        , IPlaceable
        , ICollideable
        , IHealable
        , IShieldable
    {
        private PositionComponent mPositionComponent;
        private BoundsCollisionComponent mBoundsCollisionComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private ShieldRenderComponent mShieldRenderComponent;
        private BaseAnimationComponent mAnimationComponent;
        private SimpleAudioComponent mAlarmAudioComponent;
        private IData mData;

        public Base(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, int baseHealth, IData data)
        {
            mPositionComponent = new PositionComponent(x,y, this, new Vector2(x + TileMap.TileWidth, y + TileMap.TileHeight));
            mBoundsCollisionComponent = new BaseCollisionComponent(new Rectangle(x, y, 2 * TileMap.TileWidth, 2*TileMap.TileHeight), this);
            mAnimationComponent = new BaseAnimationComponent(texturePath, sourceRectangle, interval, rows,columns,this);
            mHealthComponent = new HealthComponent(baseHealth, this);
            //mHealthRenderComponent = new BaseHealthRenderComponent("fillbarshadow", "fillbar", this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this, 2.0f, 16);
            mAlarmAudioComponent = new SimpleAudioComponent("sfx_alarm_loop2", this);
            mData = data;
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mBoundsCollisionComponent.UnRegister();
            mHealthComponent.UnRegister();
            mPositionComponent.UnRegister();
        }

        public ObjectType ObjectType { get { return ObjectType.Base; } }

        public IData Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent;}
            set { mPositionComponent = value; }
        }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get { return mBoundsCollisionComponent; }
            set { mBoundsCollisionComponent = value; }
        }

        public BaseAnimationComponent BaseAnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

       public HealthComponent HealthComponent
        {
            get { return mHealthComponent; }
            set { mHealthComponent = value; }
        }

        public ShieldRenderComponent ShieldRenderComponent
        {
            get { return mShieldRenderComponent; }
            set { mShieldRenderComponent = value; }
        }


        public HealthRenderComponent HealthRenderComponent
        {
            get { return mHealthRenderComponent; }
            set { mHealthRenderComponent = value; }
        }

        public SimpleAudioComponent AlarmAudioComponent
        {
            get { return mAlarmAudioComponent;}
            set { mAlarmAudioComponent = value; }
        }
    }

    public class BaseAnimationComponent : AnimationComponent
    {
        public BaseAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner, bool register = true) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            Base bse = Owner as Base;

            if (bse == null)
                return;

            PositionComponent positionComponent = bse.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

            if (SystemsManager.Instance.IsPaused)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentTime <= 0.0f)
            {
                CurrentTime = Interval;
                CurrentColumnIndex++;
            }


            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
                CurrentRectangle = SourceRectangle;
            }


            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width), 
                SourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), 
                SourceRectangle.Width, SourceRectangle.Height);

            

        }
    }

    public class BaseCollisionComponent : BoundsCollisionComponent
    {
        private static readonly int kDistance = TileMap.TileWidth;

        public BaseCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Base bse = Owner as Base;

            if (bse == null)
                return;


            foreach (int cell in Cells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);

                foreach (CollisionComponent component in nearbyComponents)
                {
                    if (component == this || !(component.Owner is Unit))
                        continue;
                    
                    Unit unit = component.Owner as Unit;
                    UnitCollisionComponent unitCollision = unit.BoundsCollisionComponent;
                    UnitAnimationComponent animationComponent = unit.RenderComponent;
                    Vector2 unitOrigin = new Vector2(unitCollision.Bounds.X + unitCollision.Bounds.Width / 2, unitCollision.Bounds.Y + unitCollision.Bounds.Height / 2) ;
                    Vector2 baseOrigin  = new Vector2(mBounds.X + mBounds.Width / 2, mBounds.Y + mBounds.Height / 2);

                    if (GeometryOps.IsWithinDistance(baseOrigin, unitOrigin, kDistance))
                    {
                        HealthComponent healthComponent = bse.HealthComponent;
                        AudioComponent alarmAudioComponent = bse.AlarmAudioComponent;
                        healthComponent.Health--;
                        alarmAudioComponent.Play();

                        animationComponent.IsAttacking = true;
                        animationComponent.EntityState = EntityState.Idle;
                        animationComponent.AttackEntityId = Owner.Id;
                        unit.PositionComponent.Speed = 0;


                        EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                        {
                            Play = true,
                            SoundEffect = "sfx_enemy_attack"
                        });


                        Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
                        Camera camera = gameLayer.EntitySystem.GetEntitiesByType<Camera>().FirstOrDefault() as Camera;
                       // CameraTouchComponent touchComponent = camera.TouchComponent;

                        //touchComponent.Shake = true;
                        CameraPositionComponent cameraPositionComponent = camera.PositionComponent;
                        cameraPositionComponent.Shake = true;

                        if (healthComponent.Health <= 0)
                        {
                            EventSystem.EnqueueEvent(EventType.GameOver);
                        }


                    }
                }
                

            }
        }

       
    }

    public class BaseHealthRenderComponent : HealthRenderComponent
    {
        public BaseHealthRenderComponent(string borderTexturePath, string fillTexturePath, Entity owner, bool register = true)
            : base(borderTexturePath, fillTexturePath, owner, register)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            Base bse = Owner as Base;
            float healthPercentage = bse.HealthComponent.HealthPercentage;
            PositionComponent positionComponent = bse.PositionComponent;
            Rectangle healthBarRectangle = new Rectangle(0, 0, (int)Math.Ceiling((mFillTexture.Width * healthPercentage)), mFillTexture.Height);

            spriteBatch.Draw(mFillTexture, new Vector2(positionComponent.X, positionComponent.Y - 20),
                healthBarRectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.34f);

            spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y - 20), null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.36f);

        }

    }
}
