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
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public enum TowerType
    {
        None,
        Laser,
        Fire,
        Bullet,
        Heavy,
        Shotgun,
        Nuclear,
        Rocket
    }


    public class Tower 
        : Entity
        , IPlaceable
        , IBuildable
        , ICollideable
        , IHealable
        , IShieldable
        , IDamageable
    {
        private TowerAnimationComponentBase mAnimationComponent;
        private BoundsCollisionComponent mBoundsCollisionComponent;
        private PositionComponent mPositionComponent;
        private TowerAimerPositionComponent mCannonPositionComponent;
        private SimpleAudioComponent mAudioComponent;
        private CooldownComponent mCooldownComponent;
        private CooldownRenderComponent mCooldownRenderComponent;
        private HealthComponent mHealthComponent;
        private HealthRenderComponent mHealthRenderComponent;
        private ShieldRenderComponent mShieldRenderComponent;
        private AmmoCapacityComponent mAmmoCapacityComponent;
        private AmmoCapacityRenderComponent mAmmoCapacityRenderComponent;
        private FieldOfViewShaderComponent mFieldOfViewShader;
        private FieldOfViewRenderComponent mFieldOfViewRenderComponent;
        private IData mData;
        private TowerType mType;
        private bool mEnabled;
        private bool mShowSmokeDamage;

        public Tower(TowerType towerType, int x, int y, string soundEffect, TowerData towerData)
        {
            mData = towerData;
            mPositionComponent = new PositionComponent(x,y,this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            mHealthComponent = new ResistantHealthComponent(towerData.Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            mFieldOfViewRenderComponent = new FieldOfViewRenderComponent(32, Color.Red, this);
            mAudioComponent = new SimpleAudioComponent(soundEffect, this);
            mType = towerType;
            
        }

        public override void UnRegister()
        {
            mPositionComponent.UnRegister();
            mHealthComponent.UnRegister();
            mShieldRenderComponent.UnRegister();
            mCooldownRenderComponent.UnRegister();
            mFieldOfViewRenderComponent.UnRegister();
            mAudioComponent.UnRegister();
            base.UnRegister();
        }

        public TowerType Type { get { return mType; } }

        public ObjectType ObjectType { get { return ObjectType.Tower;} }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public TowerAimerPositionComponent CannonPositionComponent
        {
            get { return mCannonPositionComponent; }
            set { mCannonPositionComponent = value; }
        }

        public BoundsCollisionComponent BoundsCollisionComponent
        {
            get { return mBoundsCollisionComponent; }
            set { mBoundsCollisionComponent = value; }
        }

        public TowerAnimationComponentBase AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }
        
        public SimpleAudioComponent AudioComponent
        {
            get { return mAudioComponent; }
            set { mAudioComponent = value; }
        }

        public CooldownComponent CooldownComponent
        {
            get { return mCooldownComponent; }
            set { mCooldownComponent = value; }
        }

        public CooldownRenderComponent CooldownRenderComponent
        {
            get { return mCooldownRenderComponent;}
            set { mCooldownRenderComponent = value; }
        }

        public HealthComponent HealthComponent
        {
            get { return mHealthComponent; }
            set { mHealthComponent = value; }
        }

        public HealthRenderComponent HealthRenderComponent
        {
            get { return mHealthRenderComponent;}
            set { mHealthRenderComponent = value; }
        }

        public ShieldRenderComponent ShieldRenderComponent
        {
            get { return mShieldRenderComponent; }
            set { mShieldRenderComponent = value; }
        }

        public AmmoCapacityComponent AmmoCapacityComponent
        {
            get { return mAmmoCapacityComponent; }
            set { mAmmoCapacityComponent = value; }
        }

        public AmmoCapacityRenderComponent AmmoCapacityRenderComponent
        {
            get { return mAmmoCapacityRenderComponent; }
            set { mAmmoCapacityRenderComponent = value; }
        }

        public IData Data
        {
            get { return mData;}
            set { mData = value; }
        }

        public bool Enabled
        {
            get { return mEnabled;}
            set { mEnabled = value; }
        }

        public bool ShowSmokeDamage
        {
            get { return mShowSmokeDamage; }
            set { mShowSmokeDamage = value; }
        }

    }

    public class FireTower : Tower
    {
        private FieldOfViewShaderComponent mShaderComponent;

        public FireTower(TowerType towerType, string spriteSheet, int x, int y, string soundEffect, TowerData towerData)
            : base(towerType, x, y, soundEffect, towerData)
        {
           // AnimationComponent = new TowerAnimationComponent(spriteSheet, new Rectangle(0, 0, 32, 32), 
           //     0, 0, 1, 1, 1, 1, this);
            BoundsCollisionComponent =
                new BoundsCollisionComponent(
                    new Rectangle(x, y, TileMap.TileWidth,
                        TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            mShaderComponent = new FieldOfViewShaderComponent("ShadowEffect", towerData.FieldOfView, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            mShaderComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class BulletTower : Tower
    {
        private ShaderComponent mShaderComponent;
        public BulletTower(TowerType towerType, string towerTexturePath, string projectilesTexturePath, int x, int y, string soundEffect, TowerData towerData) 
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new TowerAnimationComponent(towerTexturePath, projectilesTexturePath,
                new Rectangle(34, 0, 30, 32), new Rectangle(32, 96, 32, 32), new Rectangle(0, 0, 32, 32), 1.0f, 0.16f, 1, 1, 1, 3, this);
            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x,y, TileMap.TileWidth, TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(34,0,30,32),  this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class HeavyTower : Tower
    {
        public HeavyTower(TowerType towerType, string texturePath, string projectilesPath, int x, int y, string soundEffect, TowerData towerData) 
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new TowerAnimationComponent(texturePath, projectilesPath, new Rectangle(62,0,32,32), 
                new Rectangle(32,96,32,32), new Rectangle(0,0,32,32), 1.0f, 0.3f, 1, 1, 1, 3, this);
            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, true, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(62, 0, 32, 32), this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class ShotgunTower : Tower
    {
        public ShotgunTower(TowerType towerType, string texturePath, string projectilesTexturePath, int x, int y, string soundEffect, TowerData towerData) 
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new TowerAnimationComponent(texturePath, projectilesTexturePath, 
                new Rectangle(32, 32, 32, 32), new Rectangle(32, 96, 32, 32), new Rectangle(0, 0, 32, 32), 1.0f, 0.16f, 1, 1, 1, 3, this);
            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x, y, TileMap.TileWidth , TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(32, 32, 32, 32), this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class LaserTower : Tower
    {
        public LaserTower(TowerType towerType, string texturePath, string projectilesTexturePath, int x, int y, string soundEffect, TowerData towerData)
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new LaserTowerAnimationComponent(texturePath, projectilesTexturePath, 
                new Rectangle(0,32, 32, 32), new Rectangle(26, 10, 14, 16), Rectangle.Empty, 1.0f, 0.16f, 1,1,4,1, this);

            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(0, 32, 32, 32), this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class NuclearTower : Tower
    {
        public NuclearTower(TowerType towerType, string texturePath, string projectilesTexturePath, int x, int y, string soundEffect, TowerData towerData)
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new TowerAnimationComponent(texturePath, projectilesTexturePath, new Rectangle(0, 0, 32, 32), new Rectangle(32,96,32,32),  Rectangle.Empty, 
                1.0f, 0.3f, 1, 1, 1, 4, this);
            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(0, 0, 32, 32), this);            
            (AnimationComponent as TowerAnimationComponent).FireState.DisableMuzzleFire = true;
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class RocketTower : Tower
    {
        public RocketTower(TowerType towerType, string texturePath, string projectilesTexturePath, int x, int y, string soundEffect, TowerData towerData)
            : base(towerType, x, y, soundEffect, towerData)
        {
            AnimationComponent = new TowerAnimationComponent(texturePath, projectilesTexturePath, new Rectangle(64, 32, 32, 32), new Rectangle(32,96,32,32) , new Rectangle(0,0,32,32), 
                1.0f, 0.3f, 1, 1, 1, 3, this);
            BoundsCollisionComponent = new TowerCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            CannonPositionComponent = new TowerAimerPositionComponent(x, y, false, this);
            CooldownComponent = new CooldownComponent(towerData.Cooldown, this);
            AmmoCapacityComponent = new AmmoCapacityComponent(towerData.AmmoCapacity, this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
            CooldownRenderComponent = new CooldownRenderComponent("towersreload", new Rectangle(64, 32, 32, 32), this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            CannonPositionComponent.UnRegister();
            CooldownComponent.UnRegister();
            CooldownRenderComponent.UnRegister();
            AmmoCapacityComponent.UnRegister();
            base.UnRegister();
        }
    }

}
