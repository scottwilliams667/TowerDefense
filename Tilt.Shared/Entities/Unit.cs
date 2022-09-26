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
using Tilt.Shared.Components;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Entities
{
    public enum EntityState
    {
        Idle,
        Walking
    }

    [Flags]
    public enum UnitType
    {
        Basic,
        Heavy,
        HeavySlow,
        Fast,
        LightFast,
        FastWait
    }

    public class Unit 
        : Entity
    {
        private UnitAnimationComponent mRenderComponent;
        private UnitPositionComponent mPositionComponent;
        private UnitCollisionComponent mBoundsCollisionComponent;
        private UnitDamageTimerComponent mDamageTimerComponent;
        private HealthComponent mHealthComponent;
        private BuffComponent mBuffComponent;
        private UnitData mData;

        public Unit()
        {
        }

        public override void UnRegister()
        {

            mRenderComponent.UnRegister();
            mPositionComponent.UnRegister();
            mBoundsCollisionComponent.UnRegister();
            mBuffComponent.UnRegister();

            base.UnRegister();
        }

        public UnitAnimationComponent RenderComponent
        {
            get { return mRenderComponent; }
            set { mRenderComponent = value; }
        }

        public UnitPositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public UnitCollisionComponent BoundsCollisionComponent
        {
            get { return mBoundsCollisionComponent;}
            set { mBoundsCollisionComponent = value; }
        }

        public UnitDamageTimerComponent DamageTimerComponent
        {
            get { return mDamageTimerComponent; }
            set { mDamageTimerComponent = value; }
        }

        public HealthComponent HealthComponent
        {
            get { return mHealthComponent;}
            set { mHealthComponent = value; }
        }
        
        public BuffComponent BuffComponent
        {
            get { return mBuffComponent;}
            set { mBuffComponent = value; }
        }

        public UnitData Data
        {
            get { return mData; }
            set { mData = value; }
        }
    }

    public class UnitBasic : Unit
    {
        public UnitBasic(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitBasicAttackTimeout, new Rectangle(0, 0, 48, 48), 0.5f, 1, 1, this);
            PositionComponent = new UnitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitBasicDamageTimeout, this);
            Data = unitData;
        }
    }

    public class UnitHeavy : Unit
    {
        public UnitHeavy(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitHeavyAttackTimeout, new Rectangle(0, 0, 48, 48), 0.33f, 1, 4, this);
            PositionComponent = new UnitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitHeavyDamageTimeout, this);
            Data = unitData;
        }
    }

    public class UnitHeavySlow : Unit
    {
        public UnitHeavySlow(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitHeavySlowAttackTimeout, new Rectangle(0, 0, 48, 48), 0.5f, 1, 4, this);
            PositionComponent = new UnitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitHeavySlowDamageTimeout, this);
            Data = unitData;
        }
    }

    public class UnitLightFast : Unit
    {
        public UnitLightFast(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitLightFastAttackimeout, new Rectangle(145, 0, 48, 48), 0.5f, 1, 1, this);
            PositionComponent = new UnitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitLightFastDamageTimeout, this);
            Data = unitData;
        }
    }

    public class UnitFast : Unit
    {
        public UnitFast(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitFastAttackTimeout, new Rectangle(192, 0, 48, 48), 0.5f, 1, 1, this);
            PositionComponent = new UnitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitFastDamageTimeout, this);
            Data = unitData;
        }
    }

    public class UnitFastWait : Unit
    {
        public UnitFastWait(int x, int y, TileCoord endTileCoords, string texturePath, string damageTexturePath, string attackTexturePath, int speed, UnitData unitData)
        {
            RenderComponent = new UnitAnimationComponent(texturePath, damageTexturePath, attackTexturePath, Tuner.UnitFastWaitAttackTimeout, new Rectangle(240, 0, 48, 48), 0.5f, 1, 1, this);
            PositionComponent = new UnitFastWaitPositionComponent(x, y, endTileCoords, speed, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new UnitCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new HealthComponent(unitData.Health, this);
            BuffComponent = new BuffComponent(this);
            DamageTimerComponent = new UnitDamageTimerComponent(Tuner.UnitFastWaitDamageTimeout, this);
            Data = unitData;
        }
    }
}
