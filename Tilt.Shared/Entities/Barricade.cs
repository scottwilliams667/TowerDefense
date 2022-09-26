using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Entities
{
    public enum ObjectType
    {
        None,
        Tower,
        Barricade,
        ResourcePile,
        Refinery,
        Base,
        AddOn,
        ShieldGenerator
    }

    public class Barricade 
        : Entity
        , IPlaceable
        , IBuildable
        , ICollideable
        , IHealable
        , IShieldable
        , IDamageable
    {
        private BarricadeAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;
        private BoundsCollisionComponent mBoundsCollisionComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private ShieldRenderComponent mShieldRenderComponent;
        private IData mData;
        private bool mIsEnabled;
        private bool mShowSmokeDamage;

        public Barricade(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns)
        {
            mData = new BarricadeData(
                Tuner.BarricadeName,
                Tuner.BarricadeDescription,
                Tuner.BarricadeHealth,
                Tuner.BarricadePriceToBuy);
            mAnimationComponent = new BarricadeAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mPositionComponent = new PositionComponent(x,y,this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            mBoundsCollisionComponent = new BarricadeCollisionComponent(new Rectangle(x,y, mAnimationComponent.Texture.Width, mAnimationComponent.Texture.Height), this);
            mHealthComponent = new ResistantHealthComponent((mData as BarricadeData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mPositionComponent.UnRegister();
            mBoundsCollisionComponent.UnRegister();
        }

        public ObjectType ObjectType
        {
            get { return ObjectType.Barricade;}
        }

        public IData Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public BarricadeAnimationComponent RenderComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get { return mBoundsCollisionComponent; }
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

        public ShieldRenderComponent ShieldRenderComponent
        {
            get { return mShieldRenderComponent; }
            set { mShieldRenderComponent = value; }
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

    public class BarricadeCollisionComponent : BoundsCollisionComponent
    {
        public BarricadeCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Barricade barricade = Owner as Barricade;
            PositionComponent positionComponent = barricade.PositionComponent;

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

                    HealthComponent healthComponent = barricade.HealthComponent;

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
