using System;
using System.Collections.Generic;
using System.Text;

namespace Tilt.Shared.Structures
{
    public static class Tuner
    {
        public static int MapStartPosX;
        public static int MapStartPosY;

        public static int BulletTowerDamage = 1;
        public static int BulletTowerFieldOfView = 256;
        public static int BulletTowerPriceToBuy = 250;
        public static int BulletTowerPriceToSell = 125;
        public static int BulletTowerShotsPerFire = 1;
        public static int BulletTowerHealth = 5;
        public static float BulletTowerFireRate = 2.0f;
        public static int BulletTowerCooldownSeconds = 5;
        public static int BulletTowerAmmoCapacity = 5;

        public static int HeavyTowerDamage = 1;
        public static int HeavyTowerFieldOfView = 256;
        public static int HeavyTowerPriceToBuy = 250;
        public static int HeavyTowerPriceToSell = 125;
        public static int HeavyTowerShotsPerFire = 1;
        public static int HeavyTowerHealth = 5;
        public static float HeavyTowerFireRate = 2.0f;
        public static int HeavyTowerCooldownSeconds = 5;
        public static int HeavyTowerAmmoCapacity = 5;

        public static int LaserTowerDamage = 1;
        public static int LaserTowerFieldOfView = 256;
        public static int LaserTowerPriceToBuy = 250;
        public static int LaserTowerPriceToSell = 125;
        public static int LaserTowerShotsPerFire = 1;
        public static int LaserTowerHealth = 10;
        public static float LaserTowerFireRate = 2.0f;
        public static int LaserTowerCooldownSeconds = 5;
        public static int LaserTowerAmmoCapacity = 5;

        public static int ShotgunTowerDamage = 1;
        public static int ShotgunowerFieldOfView = 256;
        public static int ShotgunTowerPriceToBuy = 250;
        public static int ShotgunTowerPriceToSell = 125;
        public static int ShotgunTowerShotsPerFire = 6;
        public static int ShotgunTowerHealth = 10;
        public static float ShotgunTowerFireRate = 2.0f;
        public static int ShotgunTowerCooldownSeconds = 5;
        public static int ShotgunTowerAmmoCapacity = 5;

        public static int NuclearTowerDamage = 1;
        public static int NuclearTowerFieldOfView = 256;
        public static int NuclearTowerPriceToBuy = 250;
        public static int NuclearTowerPriceToSell = 125;
        public static int NuclearTowerShotsPerFire = 1;
        public static int NuclearTowerHealth = 10;
        public static float NuclearTowerFireRate = 4.0f;
        public static int NuclearTowerCooldownSeconds = 5;
        public static int NuclearTowerAmmoCapacity = 5;

        public static int RocketTowerDamage = 1;
        public static int RocketTowerFieldOfView = 256;
        public static int RocketTowerPriceToBuy = 250;
        public static int RocketTowerPriceToSell = 125;
        public static int RocketTowerShotsPerFire = 1;
        public static int RocketTowerHealth = 10;
        public static float RocketTowerFireRate = 2.0f;
        public static int RocketTowerCooldownSeconds = 5;
        public static int RocketTowerAmmoCapacity = 5;

        public static float AmmoCapacityAddOnIncrease = 1;
        public static int AmmoCapacityAddOnFieldOfView = 256;
        public static int AmmoCapacityAddOnHealth = 10;

        public static float CooldownAddOnIncrease = 0.3f;
        public static int CooldownAddOnFieldOfView = 256;
        public static int CooldownAddOnHealth = 10;

        public static float RefineryAddOnIncrease = 2;
        public static int RefineryAddOnFieldOfView = 48;
        public static int RefineryAddOnHealth = 10;

        public static float RangeBoosterAddOnIncrease = 1.5f;
        public static int RangeBoosterAddOnFieldOfView = 256;
        public static int RangeBoosterAddOnHealth = 10;

        public static int BarricadeHealth = 10;

        public static int RefineryNumUnitsPerSecond = 2;
        public static int RefineryHealth = 10;

        public static int ShieldGeneratorHealthBoost = 2;
        public static int ShieldGeneratorFieldOfView = 256;
        public static int ShieldGeneratorHealth = 10;

        public static float UnitBasicDamageTimeout = 0.25f;
        public static float UnitHeavyDamageTimeout = 0.25f;
        public static float UnitHeavySlowDamageTimeout = 0.25f;
        public static float UnitFastDamageTimeout = 0.25f;
        public static float UnitLightFastDamageTimeout = 0.25f;
        public static float UnitFastWaitDamageTimeout = 0.25f;

        public static float UnitBasicAttackTimeout = 0.5f;
        public static float UnitHeavyAttackTimeout = 0.5f;
        public static float UnitHeavySlowAttackTimeout = 0.5f;
        public static float UnitFastAttackTimeout = 0.5f;
        public static float UnitLightFastAttackimeout = 0.5f;
        public static float UnitFastWaitAttackTimeout = 0.5f;

        public static int UnitBasicDamage = 1;
        public static int UnitHeavyDamage = 1;
        public static int UnitHeavySlowDamage = 1;
        public static int UnitFastDamage = 1;
        public static int UnitLightFastDamage = 1;
        public static int UnitFastWaitDamage = 1;

        public static int UnitBasicHealth = 10;
        public static int UnitHeavyHealth = 10;
        public static int UnitHeavySlowHealth = 10;
        public static int UnitFastHealth = 10;
        public static int UnitLightFastHealth = 10;
        public static int UnitFastWaitHealth = 10;


        public static string BarricadeName = "BARRICADE";
        public static string BarricadeDescription = "USED TO BLOCK ENEMIES AND KEEP VITAL STRUCTURES ALIVE";
        public static string RefineryName = "REFINERY";
        public static string RefineryDescription = "HARVESTS MINERALS ON THE MAP";
        public static string ShieldGeneratorName = "SHIELD GENERATOR";
        public static string ShieldGeneratorDescription = "PROVIDES BUILDINGS WITH SHIELDS";
        public static string CooldownAddOnName = "COOLDOWN ADD-ON";
        public static string CooldownAddOnDescription = "ALLOWS TOWERS TO COOLDOWN FASTER";
        public static string RangeBoosterAddOnName = "RANGE BOOSTER";
        public static string RangeBoosterAddOnDescription = "BOOSTS VISION OF TOWERS";
        public static string RefineryAddOnName = "REFINERY ADD-ON";
        public static string RefineryAddOnDescription = "ALLOWS REFINERIES TO HARVEST MINERALS MORE EFFICIENTLY";
        public static string AmmoAddOnName = "AMMO ADD-ON";
        public static string AmmoAddOnDescription = "EQUIPS TOWERS WITH EXTRA AMMO";
        public static string BulletTowerName = "BULLET TOWER";
        public static string BulletTowerDescription = "A BASIC BLASTER";
        public static string HeavyTowerName = "HEAVY TOWER";
        public static string HeavyTowerDescription = "DOUBLE BARRELED BLASTER";
        public static string LaserTowerName = "LASER TOWER";
        public static string LaserTowerDescription = "FIRES A FREAKIN LASER, YO";
        public static string NuclearTowerName = "NUCLEAR TOWER";
        public static string NuclearTowerDescription = "SECRETES NUCLEAR SLUDGE IN AN AOE ATTACK";
        public static string RocketTowerName = "ROCKET TOWER";
        public static string RocketTowerDescription = "FIRES ROCKETS THAT ARE CAPABLE OF DOING MAJOR DAMAGE";
        public static string ShotgunTowerName = "SHOTGUN TOWER";
        public static string ShotgunTowerDescription = "6 SHOT BURST FIRE";

        public static int BarricadePriceToBuy = 250;
        public static int RefineryPriceToBuy = 250;
        public static int ShieldGeneratorPriceToBuy = 250;
        public static int CooldownAddOnPriceToBuy = 250;
        public static int RangeBoosterAddOnPriceToBuy = 250;
        public static int RefineryAddOnPriceToBuy = 250;
        public static int AmmoCapacityPriceToBuy = 1000;

        public static string OperatingNormallyDescription = "OPERATING NORMALLY";
        public static string DamageDetectedDescription = "DAMAGED DETECTED";
        public static string CriticallyDamagedDescription = "CRITICALLY DAMAGED";

        public static string SFXButtonClick = "sfx_ui_buttonclick";
        public static string SFXButtonClickSlide = "sfx_ui_buttonclick_slide";
        public static string SFXPanelSlide = "sfx_ui_panelslide";
        public static string SFXUndo = "sfx_ui_undo";
        public static string SFXCancel = "sfx_ui_cancel";
        public static string SFXUISell = "sfx_ui_sell";
        public static string SFXUIDialogueText = "sfx_ui_dialogue_text";
        public static string SFXUILevelComplete = "sfx_ui_level_complete";
        public static string SFXUILogoDrop = "sfx_ui_logo_drop";
        public static string SFXObjectBuildAll = "sfx_object_buildall";
        public static string SFXPlaceTower = "sfx_place_tower";
    }
}
