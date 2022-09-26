using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Components
{

    public interface IData
    {
    }

    public interface ISellable
    {
        int PriceToBuy { get; set; }

        int PriceToSell { get; set; }
    }

    public interface IFieldOfView
    {
        float FieldOfView { get; set; }
    }

    public class ResourcePileData 
        : IData
    {
        public ResourcePileData(int units, int pricePerUnit)
        {
            Units = units;
            PricePerUnit = pricePerUnit;
        }

        public int Units { get; set; }

        public int PricePerUnit { get; set; }
    }

    public class RefineryData 
        : IData
        , ISellable
    {
        public RefineryData(string name, string description, int unitsPerSec, int health, int price)
        {
            NumUnitsPerSecond = unitsPerSec;
            Health = health;
            Name = name;
            Description = description;
            PriceToBuy = price;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        ///determines the number of units that can be harvested from a 
        ///resource pile per second.
        public int NumUnitsPerSecond { get; set; }

        public int PriceToBuy { get; set; }
        public int PriceToSell { get; set; }

        public int Health { get; set; }
    }

    public class GoldData : ResourcePileData
    {
        public GoldData(int units, int pricePerUnit) : base(units, pricePerUnit)
        {
        }
        
    }

    public class BarricadeData 
        : IData
        , ISellable
    {
        public BarricadeData(string name, string description, int health, int price)
        {
            Health = health;
            Name = name;
            Description = description;
            PriceToBuy = price;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int PriceToBuy { get; set; }

        public int PriceToSell { get; set; }

        public int Health { get; set; }
    }

    public class AddOnData
        : IData
        , ISellable
        , IFieldOfView
    {
        public AddOnData(string addonName, string description,
            float increase, float fieldOfView, int health, int price)
        {
            AddOnName = addonName;
            Description = description;
            Increase = increase;
            FieldOfView = fieldOfView;
            Health = health;
            PriceToBuy = price;
        }

        public string AddOnName { get; set; }
        public string Description { get; set; }
        public float FieldOfView { get; set; }
        public float Increase { get; set; }
        public int PriceToBuy { get; set; }
        public int PriceToSell { get; set; }

        public int Health { get; set; }
    }

    public class ShieldGeneratorData
        : IData
        , ISellable
        , IFieldOfView
    {
        public ShieldGeneratorData(string name, string description, int healthBoost, float fieldOfView, int health, int price)
        {
            HealthBoost = healthBoost;
            FieldOfView = fieldOfView;
            Health = health;
            Name = name;
            Description = description;
            PriceToBuy = price;
        }

        public string Name { get; set; }

        public string Description { get; set; }


        public float FieldOfView { get; set; }
        public int HealthBoost { get; set; }
        public int PriceToBuy { get; set; }
        public int PriceToSell { get; set; }

        public int Health { get; set; }
    }

    public class TowerData 
        : IData
        , ISellable
        , IFieldOfView
    {
        public TowerData(string towerName
            , string description
            , float damage
            , float fieldOfView
            , float fireRate
            , ProjectileType bulletType
            , int priceToBuy
            , int priceToSell
            , int shotsPerFire
            , int health
            , int cooldown
            , int ammoCapacity)
        {
            Damage = damage;
            FieldOfView = fieldOfView;
            FireRate = fireRate;
            BulletType = bulletType;
            PriceToBuy = priceToBuy;
            PriceToSell = priceToSell;
            ShotsPerFire = shotsPerFire;
            FieldOfView = fieldOfView;
            Health = health;
            Cooldown = cooldown;
            AmmoCapacity = ammoCapacity;
            TowerName = towerName;
            Description = description;
        }

        public string TowerName { get; set; }
        public string Description { get; set; }
        public float Damage { get; set; }
        public float  FieldOfView { get; set; }
        public float FireRate { get; set; }
        public ProjectileType BulletType { get; set; }
        public int PriceToBuy { get; set; }
        public int PriceToSell { get; set; }
        public int ShotsPerFire { get; set; }
        public int Health { get; set; }
        public int AmmoCapacity { get; set; }
        public int Cooldown { get; set; }

    }


    public class ProjectileData
    {

        public ProjectileData(//int damage
             int destructDistance
            , ProjectileType type)
        {
           // Damage = damage;
            DestructDistance = destructDistance;
            Type = type;
        }

        public float Damage { get; set; }
        public int DestructDistance { get; set; }
        public ProjectileType Type { get; set; }
        
    }

    public class UnitData: IData
    {
        public UnitData(int damage, int health)
        {
            Damage = damage;
            Health = health;
        }

        public int Damage { get; set; }

        public int Health { get; set; }
    }

    public class BaseData : IData
    {
    }
}
