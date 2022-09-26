using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Structures
{

    /* 
     *  The Spawner is responsible for spawning waves of enemies. The spawner
     *  takes in a list of creature types , spawn tiles, and time between spawns
     *  It will randomly generate a index into the list of spawn tiles, chose that
     *  tile and call UnitFactory.Make which spawns the creature on that tile
     * 
     * The "Spawn" property tells the Spawner when it is appropriate to spawn
     * a new creature. When the Game Layer is done being
     * 
     */
    public static class Spawner
    {
        private static Queue<Mob> mMobQueue = new Queue<Mob>(); 
        private static Queue<UnitType> mQueue = new Queue<UnitType>();
        private static Mob mCurrentMob;
        private static List<TileCoord> mSpawnTiles;
        private static float mTimeBetweenSpawns;
        private static float mCurrentTime;

        // used to signify when we've built a refinery.
        private static bool mIsSpawing;

        static Spawner()
        {
            EventSystem.SubScribe(EventType.UnitDestroyed, OnUnitDestroyed_);
        }

        public static void LoadUnits(List<List<UnitType>> units, List<TileCoord> spawnTiles, float timeBetweenSpawns )
        {
            mIsSpawing = false;

            mMobQueue.Clear();
            mQueue.Clear();


            foreach (List<UnitType> creatureList in units)
            {
                Mob mob = new Mob(creatureList);
                mMobQueue.Enqueue(mob);
            }

            mSpawnTiles = spawnTiles;
            mTimeBetweenSpawns = timeBetweenSpawns;
            mCurrentTime = 0.0f;
            mCurrentMob = mMobQueue.Dequeue();

        }

        public static void Update()
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            
            if (SystemsManager.Instance.IsPaused || gameLayer == null)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            mCurrentTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (mCurrentTime > mTimeBetweenSpawns)
            {
                Dequeue_();
                mCurrentTime = 0.0f;
            }

        }

        private static void  Dequeue_()
        {
            Random random = new Random();
            int index = random.Next(0, mSpawnTiles.Count);
            TileCoord coord = mSpawnTiles.ElementAt(index);
            if (!mCurrentMob.IsEmpty())
                mCurrentMob.Spawn(coord);


        }

        private static void OnUnitDestroyed_(object sender, IGameEventArgs e)
        {
            if (mMobQueue.Count == 0 && mCurrentMob.IsEmpty() && 
                LayerManager.Layer.EntitySystem.Entities.Count(entity => entity is Unit) == 0)
            {
                List<TileNode> tiles = TileMap.Tiles.OfType<TileNode>().ToList();
                List<IPlaceable> objectsToRefund = tiles.Where(t => t.HasObject).Select(t => t.Object).ToList();

                Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
                Base bse = gameLayer.EntitySystem.GetEntitiesByType<Base>().FirstOrDefault();
                
                if(bse.HealthComponent.Health > 0)
                { 
                    int towerCost = objectsToRefund.Where(tower => tower.Data is ISellable).Select(tower => tower.Data as ISellable).Sum(tower => tower.PriceToSell);

                    EventSystem.EnqueueEvent(EventType.LevelRecap, null, new LevelRecapArgs() {TowerRefund =  towerCost});
                }
            }
            
            else if (mCurrentMob.IsEmpty() && mMobQueue.Count > 0 &&
                LayerManager.Layer.EntitySystem.Entities.Count(entity => entity is Unit) == 0)
            {
                mCurrentMob = mMobQueue.Dequeue();
                mCurrentTime = 0.0f;
            }
            

        }


    }
}
