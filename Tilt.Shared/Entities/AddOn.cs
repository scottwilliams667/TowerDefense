using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Entities
{
    public enum AddOnType
    {
        None,
        Damage,
        RangeBooster,
        Cooldown,
        AmmoCapacity,
        Refinery
    }

    public class AddOn
        : Entity
        , IPlaceable
        , IBuildable
        , ICollideable
        , IHealable
        , IShieldable
        , IDamageable
    {
        private AddOnType mType;
        private IData mData;
        private bool mIsEnabled;
        private bool mShowSmokeDamage;
        private BoundsCollisionComponent mCollisionComponent;
        private PositionComponent mPositionComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private ShieldRenderComponent mShieldRenderComponent;
        private FieldOfViewRenderComponent mFieldOfViewRenderComponent;

        public AddOn(AddOnType type)
        {
            mType = type;
            mFieldOfViewRenderComponent = new FieldOfViewRenderComponent(32, Color.Red, this);
        }

        public override void UnRegister()
        {
            //mFieldOfViewShader.UnRegister();
            base.UnRegister();
        }

        public AddOnType Type { get { return mType; } }

        public ObjectType ObjectType { get { return ObjectType.AddOn; } }
        public IData Data { get { return mData; } set { mData = value; } }
        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
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

        public bool Enabled { get { return mIsEnabled; } set { mIsEnabled = value; } }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get { return mCollisionComponent; }
            set { mCollisionComponent = value; }
        }

        public ShieldRenderComponent ShieldRenderComponent
        {
            get { return mShieldRenderComponent; }
            set { mShieldRenderComponent = value; }
        }

        public bool ShowSmokeDamage
        {
            get { return mShowSmokeDamage; }
            set { mShowSmokeDamage = value; }
        }
    }

    public class AddOnCollisionComponent : BoundsCollisionComponent
    {
        public AddOnCollisionComponent(Rectangle bounds, Entity owner)
            : base(bounds, owner)
        {
        }

        public override void Update()
        {
            AddOn addOn = Owner as AddOn;
            PositionComponent positionComponent = addOn.PositionComponent;

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

                    HealthComponent healthComponent = addOn.HealthComponent;

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
