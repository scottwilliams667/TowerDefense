using System;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Structures
{
    public static class ProjectileFactory
    {
        private static Random mRandom = new Random();

        public static Projectile Make(ProjectileType projectileType, int bulletLevel, int x, int y, float rotation, ulong entityId = 0)
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            Layer previousLayer = LayerManager.Layer;

            if (previousLayer != gameLayer)
                LayerManager.Layer = gameLayer;


            Projectile bullet = null;
            switch (projectileType)
            {
                case ProjectileType.Bullet:
                {
                    bullet = new Bullet("projectiles", x, y, new Rectangle(32, 96, 32, 32), rotation, new ProjectileData(450, ProjectileType.Bullet));
                }
                break;
                case ProjectileType.Heavy:
                {
                    bullet = new Bullet("projectiles", x, y, new Rectangle(32, 96, 32, 32), rotation, new ProjectileData(450, ProjectileType.Heavy));
                }
                break;
                case ProjectileType.Shotgun:
                {
                    //generate random offset for the shotgun to similate shotgun spray
                    double rand = mRandom.NextDouble();
                    double randomRotation = rand * (0.52 - -0.52) + -0.52;
                    rotation = rotation + (float)randomRotation;

                    bullet = new Bullet("projectiles", x, y, new Rectangle(32, 96, 32, 32), rotation, new ProjectileData(450, ProjectileType.Bullet));
                }
                break;
                case ProjectileType.Laser:
                {
                    bullet = new Laser("projectile_laser",x,y, new Rectangle(49,14,32,5), 0.0f, 4,1, rotation, new ProjectileData(450, ProjectileType.Laser));
                    
                }
                break;
                case ProjectileType.Sludge:
                {
                    bullet = new Sludge("projectiles",x,y,rotation, new ProjectileData(150, ProjectileType.Sludge));
                }
                break;
                case ProjectileType.Rocket:
                {
                    bullet = new Rocket("projectiles", x, y, new Rectangle(32, 160, 32, 24), 1, 3, 0.16f,  rotation, new ProjectileData(300, ProjectileType.Rocket), entityId);
                }
                break;

            }

            if(previousLayer != gameLayer)
                LayerManager.Layer = previousLayer;

            return bullet;
        }
    }

    public static class UnitFactory
    {
        public static Unit Make(UnitType unitType , int x, int y, float rotation, string texturePath)
        {
            
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            Layer previousLayer = LayerManager.Layer;

            if (previousLayer != gameLayer)
                LayerManager.Layer = gameLayer;


            Unit unit = null;
            switch (unitType)
            {
                case UnitType.Basic:
                    unit = new UnitBasic(x,y, TileMap.Base, "enemies_strip6", "enemies_strip6_damage", "enemies_strip6_attack", 30, new UnitData(Tuner.UnitBasicDamage, Tuner.UnitBasicHealth));
                    break;
                case UnitType.Heavy:
                    unit = new UnitHeavy(x, y, TileMap.Base, "roller0_strip4", "roller0_strip4_damage", "roller0_strip4_attack", 25, new UnitData(Tuner.UnitHeavyDamage, Tuner.UnitHeavyHealth));
                    break;
                case UnitType.HeavySlow:
                    unit = new UnitHeavySlow(x, y, TileMap.Base, "roller1_strip4", "roller1_strip4_damage", "roller1_strip4_attack", 20, new UnitData(Tuner.UnitHeavySlowDamage, Tuner.UnitHeavySlowHealth));
                    break;
                case UnitType.Fast:
                    unit = new UnitFast(x, y, TileMap.Base, "enemies_strip6", "enemies_strip6_damage", "enemies_strip6_attack", 75, new UnitData(Tuner.UnitFastDamage, Tuner.UnitFastHealth));
                    break;
                case UnitType.LightFast:
                    unit = new UnitLightFast(x, y, TileMap.Base, "enemies_strip6", "enemies_strip6_damage", "enemies_strip6_attack", 100, new UnitData(Tuner.UnitLightFastDamage, Tuner.UnitLightFastHealth));
                    break;
                case UnitType.FastWait:
                    unit = new UnitFastWait(x, y, TileMap.Base, "enemies_strip6", "enemies_strip6_damage", "enemies_strip6_attack", 120, new UnitData(Tuner.UnitFastWaitDamage, Tuner.UnitFastWaitHealth));
                    break;
            }

            if (previousLayer != gameLayer)
                LayerManager.Layer = previousLayer;

            return unit;
        }

    }

    public static class ObjectFactory
    {
        public static IData GetDataForObject(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Barricade:
                    return new BarricadeData(
                        Tuner.BarricadeName,
                        Tuner.BarricadeDescription,
                        Tuner.BarricadeHealth,
                        Tuner.BarricadePriceToBuy);
                case ObjectType.Refinery:
                    return new RefineryData(
                        Tuner.RefineryName,
                        Tuner.RefineryDescription
                        , Tuner.RefineryNumUnitsPerSecond
                        , Tuner.RefineryHealth
                        , Tuner.RefineryPriceToBuy);
                case ObjectType.ShieldGenerator:
                    return new ShieldGeneratorData(
                        Tuner.ShieldGeneratorName,
                        Tuner.ShieldGeneratorDescription,
                        Tuner.ShieldGeneratorHealthBoost, 
                        Tuner.ShieldGeneratorFieldOfView, 
                        Tuner.ShieldGeneratorHealth,
                        Tuner.ShieldGeneratorPriceToBuy);
            }
            return null;
        }

        public static AddOnData GetDataForAddOn(AddOnType addOnType)
        {
            switch (addOnType)
            {
                case AddOnType.Cooldown:
                    return new AddOnData(
                        Tuner.CooldownAddOnName,
                        Tuner.CooldownAddOnDescription,
                        Tuner.CooldownAddOnIncrease,
                        Tuner.CooldownAddOnFieldOfView, 
                        Tuner.CooldownAddOnHealth,
                        Tuner.CooldownAddOnPriceToBuy);
                case AddOnType.RangeBooster:
                    return new AddOnData(
                        Tuner.RangeBoosterAddOnName,
                        Tuner.RangeBoosterAddOnDescription,
                        Tuner.CooldownAddOnIncrease,
                        Tuner.CooldownAddOnFieldOfView,
                        Tuner.CooldownAddOnHealth,
                        Tuner.RangeBoosterAddOnPriceToBuy);
                case AddOnType.Refinery:
                    return new AddOnData(
                        Tuner.RefineryAddOnName,
                        Tuner.RefineryAddOnDescription,
                    Tuner.RefineryAddOnIncrease,
                    Tuner.RefineryAddOnFieldOfView,
                    Tuner.RefineryAddOnHealth,
                    Tuner.RefineryAddOnPriceToBuy);
                case AddOnType.AmmoCapacity:
                    return new AddOnData(
                        Tuner.AmmoAddOnName,
                        Tuner.AmmoAddOnDescription,
                        Tuner.AmmoCapacityAddOnIncrease,
                        Tuner.AmmoCapacityAddOnFieldOfView,
                        Tuner.AmmoCapacityAddOnHealth,
                        Tuner.AmmoCapacityPriceToBuy);
            }
            return null;
        }

        public static TowerData GetDataForTower(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.Bullet:
                    return new TowerData(
                        Tuner.BulletTowerName,
                        Tuner.BulletTowerDescription,
                        Tuner.BulletTowerDamage,
                        Tuner.BulletTowerFieldOfView,
                        Tuner.BulletTowerFireRate,
                        ProjectileType.Bullet, 
                        Tuner.BulletTowerPriceToBuy,
                        Tuner.BulletTowerPriceToSell,
                        Tuner.BulletTowerShotsPerFire,
                        Tuner.BulletTowerHealth,
                        Tuner.BulletTowerCooldownSeconds,
                        Tuner.BulletTowerAmmoCapacity);
                case TowerType.Heavy:
                    return new TowerData(
                        Tuner.HeavyTowerName,
                        Tuner.HeavyTowerDescription,
                        Tuner.HeavyTowerDamage,
                        Tuner.HeavyTowerFieldOfView,
                        Tuner.HeavyTowerFireRate,
                        ProjectileType.Bullet,
                        Tuner.HeavyTowerPriceToBuy,
                        Tuner.HeavyTowerPriceToSell,
                        Tuner.HeavyTowerShotsPerFire,
                        Tuner.HeavyTowerHealth,
                        Tuner.HeavyTowerCooldownSeconds,
                        Tuner.HeavyTowerAmmoCapacity);
                case TowerType.Laser:
                    return new TowerData(
                        Tuner.LaserTowerName,
                        Tuner.LaserTowerDescription,
                        Tuner.LaserTowerDamage,
                        Tuner.LaserTowerFieldOfView,
                        Tuner.LaserTowerFireRate,
                        ProjectileType.Laser,
                        Tuner.LaserTowerPriceToBuy,
                        Tuner.LaserTowerPriceToSell,
                        Tuner.LaserTowerShotsPerFire,
                        Tuner.LaserTowerHealth,
                        Tuner.LaserTowerCooldownSeconds,
                        Tuner.LaserTowerAmmoCapacity);
                case TowerType.Nuclear:
                    return new TowerData(
                        Tuner.NuclearTowerName,
                        Tuner.NuclearTowerDescription,
                        Tuner.NuclearTowerDamage,
                        Tuner.NuclearTowerFieldOfView,
                        Tuner.NuclearTowerFireRate,
                        ProjectileType.Sludge,
                        Tuner.NuclearTowerPriceToBuy,
                        Tuner.NuclearTowerPriceToSell,
                        Tuner.NuclearTowerShotsPerFire,
                        Tuner.NuclearTowerHealth,
                        Tuner.NuclearTowerCooldownSeconds,
                        Tuner.NuclearTowerAmmoCapacity);
                case TowerType.Rocket:
                    return new TowerData(
                        Tuner.RocketTowerName,
                        Tuner.RocketTowerDescription,
                        Tuner.RocketTowerDamage,
                        Tuner.RocketTowerFieldOfView,
                        Tuner.RocketTowerFireRate,
                        ProjectileType.Rocket,
                        Tuner.RocketTowerPriceToBuy,
                        Tuner.RocketTowerPriceToSell,
                        Tuner.RocketTowerShotsPerFire,
                        Tuner.RocketTowerHealth,
                        Tuner.RocketTowerCooldownSeconds,
                        Tuner.RocketTowerAmmoCapacity);
                case TowerType.Shotgun:
                    return new TowerData(
                        Tuner.ShotgunTowerName,
                        Tuner.ShotgunTowerDescription,
                        Tuner.ShotgunTowerDamage,
                        Tuner.ShotgunowerFieldOfView,
                        Tuner.ShotgunTowerFireRate,
                        ProjectileType.Shotgun,
                        Tuner.ShotgunTowerPriceToBuy,
                        Tuner.ShotgunTowerPriceToSell,
                        Tuner.ShotgunTowerShotsPerFire,
                        Tuner.ShotgunTowerHealth,
                        Tuner.ShotgunTowerCooldownSeconds,
                        Tuner.ShotgunTowerAmmoCapacity);
            }
            return null;
        }

        public static IPlaceable Make(TowerSynchronizer towerSynchronizer, int x, int y)
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            Layer previousLayer = LayerManager.Layer;

            if (previousLayer != gameLayer)
                LayerManager.Layer = gameLayer;

            IPlaceable obj = null;

            switch (towerSynchronizer.Type)
            {
                case ObjectType.Barricade:
                    obj = new Barricade("buildings", x, y, new Rectangle(0, 32, TileMap.TileWidth, TileMap.TileHeight), 0.0f, 1, 1);
                    break;
                case ObjectType.Refinery:
                    obj = new Refinery("buildings", "refinerywave",  x, y, new Rectangle(32,32, TileMap.TileWidth, TileMap.TileHeight), new Rectangle(48, 0, 16,32), new Rectangle(0,0,16,32), 1, 3, 0.33f,  GetDataForObject(ObjectType.Refinery));
                    break;
                case ObjectType.ShieldGenerator:
                    obj = new ShieldGenerator("buildings", "Fire", x, y, new Rectangle(0, 0, TileMap.TileWidth, TileMap.TileHeight), 0.5f, 1, 1, GetDataForObject(ObjectType.ShieldGenerator));
                    break;
            }

            if (towerSynchronizer.Type == ObjectType.Tower)
            {

                switch (towerSynchronizer.TowerType)
                {
                    case TowerType.Bullet:
                        obj = new BulletTower(TowerType.Bullet, "towers", "projectiles", x, y, "sfx_shoot_pistol", GetDataForTower(TowerType.Bullet));
                        break;
                    case TowerType.Heavy:
                        obj = new HeavyTower(TowerType.Heavy, "towers", "projectiles", x, y, "sfx_shoot_heavy", GetDataForTower(TowerType.Heavy));
                        break;
                    case TowerType.Shotgun:
                        obj = new ShotgunTower(TowerType.Shotgun, "towers", "projectiles", x, y, "sfx_shoot_shotgun", GetDataForTower(TowerType.Shotgun));
                        break;
                    case TowerType.Laser:
                        obj = new LaserTower(TowerType.Laser, "towers", "projectile_laser", x, y, "sfx_shoot_laser", GetDataForTower(TowerType.Laser));
                        break;
                    case TowerType.Nuclear:
                        obj = new NuclearTower(TowerType.Nuclear, "towers", "projectiles", x, y, "sfx_shoot_nuclear", GetDataForTower(TowerType.Nuclear));
                        break;
                    case TowerType.Rocket:
                        obj = new RocketTower(TowerType.Rocket, "towers", "projectiles", x, y, "sfx_shoot_rocket", GetDataForTower(TowerType.Rocket));
                        break;
                }
            }

            if (towerSynchronizer.Type == ObjectType.AddOn)
            {

                switch (towerSynchronizer.AddOnType)
                {
                    case AddOnType.Cooldown:
                        obj = new CooldownAddOn("addons", x, y,
                            new Rectangle(0, 0, TileMap.TileWidth, TileMap.TileWidth), 0.3f, 1, 1,
                            GetDataForAddOn(AddOnType.Cooldown));
                        break;
                    case AddOnType.AmmoCapacity:
                        obj = new AmmoAddOn("buildings_strip6", x, y, 
                            new Rectangle(320, 0, TileMap.TileWidth, TileMap.TileHeight), 0.3f, 1, 2, 
                            GetDataForAddOn(AddOnType.AmmoCapacity));
                        break;
                    case AddOnType.RangeBooster:
                        obj = new RangeBoosterAddOn("buildings_strip6", x, y,
                            new Rectangle(256, 0, TileMap.TileWidth, TileMap.TileHeight), 2.0f, 1, 2,
                            GetDataForAddOn(AddOnType.RangeBooster));
                        break;
                    case AddOnType.Refinery:
                        obj = new RefineryAddOn("buildings_strip6", x, y,
                            new Rectangle(0, 0, TileMap.TileWidth, TileMap.TileHeight), 0.5f, 2, 2,
                            GetDataForAddOn(AddOnType.Refinery));
                        break;
                }
            }

            if (previousLayer != gameLayer)
                LayerManager.Layer = previousLayer;

            return obj;
        }
    }

    public static class BuffFactory
    {
        public static Buff GenerateBuffForEntity(ProjectileType type, Entity entity)
        {
            if (entity is Unit)
            {
                if (type == ProjectileType.Fire)
                {
                    return new UnitFireBuff(1, 5, ProjectileType.Fire, (uint)entity.Id);
                }
            }
            return null;
        }

    }

}
