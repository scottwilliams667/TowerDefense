using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Systems
{
    public static class LevelManager
    {
        private static Level mLevel;
        private static List<Level> mLevels;
        private static SaveFile mSaveFile;
        private static Settings mSettings;

        public static void LoadLevels()
        {
            mLevels = AssetOps.Serializer.DeserializeLevelFile("Levels");
            mSaveFile = AssetOps.Serializer.DeserializeSaveFile();
            if (SaveFile.LevelCompleted < mLevels.Count)
            {
                mLevel = mLevels.ElementAt(SaveFile.LevelCompleted);
            }
            else
            {
                mLevel = mLevels.ElementAt(mLevels.Count - 1);
            }
            mSettings = AssetOps.Serializer.DeserializeSettings();

        }

        public static void Save()
        {
            Base bse = LayerManager.Layer.EntitySystem.GetEntitiesByType<Base>().FirstOrDefault();

            int baseHealth = (bse != null) ? bse.HealthComponent.Health : Level.BaseHealth;

            uint resources = 0;
            if (mSaveFile.Minerals.TryGetValue(mLevel.Number - 1, ref resources))
            {
                mSaveFile.Minerals[mLevel.Number - 1] = Resources.Minerals;
            }
            else
            {
                mSaveFile.Minerals.Add(Resources.Minerals);
            }


            mSaveFile.LevelCompleted = mLevel.Number;

            mSaveFile.BaseHealth = baseHealth;

            mSaveFile.UnitsDestroyedOverCampaign += Resources.UnitsDestroyedOverLevel;

            mSaveFile.ResourcesSpentOverCampaign += Resources.ResourcesSpentOverLevel;

           AssetOps.Serializer.SerializeSaveFile();
        }

        public static void LoadLevel(int levelNumber)
        {
            if (levelNumber > mLevels.Count || levelNumber < 1)
                levelNumber = 1;

            mLevel = mLevels.ElementAt(levelNumber - 1);
            LoadLevel();

        }

        public static void LoadLevel()
        {
            if (!LayerManager.HasLayer(LayerType.Game))
                LayerManager.Push(LayerType.Game);
            if (!LayerManager.HasLayer(LayerType.Hud))
                LayerManager.Push(LayerType.Hud, true);


            LayerManager.SetLayer(LayerType.Game);


            SystemsManager.Instance.CreateBaseGameObjects();

            List<TileCoord> deadTiles = AssetOps.Serializer.DeserializeMapDataFile(mLevel.MapNameLeft);

            
            TileMap.LoadTiles(deadTiles, mLevel.SpawnTiles, mLevel.ResourceTiles);
            TileMap.Base = mLevel.Base;
            Spawner.LoadUnits(mLevel.Units, mLevel.SpawnTiles, mLevel.TimeBetweenSpawns);

            LoadResourcePiles_(mLevel.ResourceTiles);

            if (mLevel.Number == 1)
                Resources.Minerals = mLevel.Minerals;
            else
            {
                //weve already loaded the next leve
                //get the minerals from "2" levels ago
                Resources.Minerals = mSaveFile.Minerals[mLevel.Number - 2];
            }

            Resources.ResourcesSpentOverCampaign = mSaveFile.ResourcesSpentOverCampaign;
            Resources.UnitsDestroyedOverCampaign = mSaveFile.UnitsDestroyedOverCampaign;
            Resources.UnitsDestroyedOverLevel = 0;
            Resources.ResourcesSpentOverLevel = 0;

            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;
            DateTime dateTime = DateTime.Now;

            int hours = mLevel.Time / 3600; //hour of day

            infoBar.SetTimeOfDay(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                hours, 0,0));
            
        }

        public static Level Level
        {
            get { return mLevel;}
            set { mLevel = value; }
        }

        public static List<Level> Levels
        {
            get { return mLevels;}
        }

        public static SaveFile SaveFile
        {
            get { return mSaveFile; }
            set { mSaveFile = value; }
        }

        public static Settings Settings
        {
            get { return mSettings; }
            set { mSettings = value; }
        }

        //move this somewhere else
        private static void LoadResourcePiles_(List<ResourceTile> resourceTiles)
        {
            foreach (ResourceTile tile in resourceTiles)
            {
                if (tile.ResourceType == ResourceType.Gold)
                {
                    ResourcePile pile = new GoldPile("tiles", new Rectangle(32,97, TileMap.TileWidth-1, TileMap.TileHeight),  tile.X * TileMap.TileWidth + Tuner.MapStartPosX, tile.Y * TileMap.TileHeight + Tuner.MapStartPosY, new GoldData(tile.Amount, 5));
                    TileNode tileNode = TileMap.GetTileNode(tile.X, tile.Y);
                    tileNode.Object = pile;
                }
            }
        }

    }
}
