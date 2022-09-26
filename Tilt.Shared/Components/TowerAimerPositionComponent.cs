using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class TowerAimerPositionComponent : PositionComponent
    {
        private float mRotation;
        private float mCurrentTime;

        private bool mBurstMode;
        private float mBurstTime;

        private int mShotsPerFire;
        private int mShotsFired = 0;

        public TowerAimerPositionComponent(int x, int y, bool burstMode, Entity owner) : base(x,y,owner)
        {
            Tower tower = Owner as Tower;
            mCurrentTime = 0.0f;

            mBurstMode = burstMode;

            mBurstTime = mCurrentTime / (tower.Data as TowerData).ShotsPerFire;
            mShotsPerFire = (tower.Data as TowerData).ShotsPerFire;
        }

        public float Rotation
        {
            get { return mRotation; }
        }

        public bool BurstMode
        {
            get { return mBurstMode; }
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;

            if (!tower.Enabled || SystemsManager.Instance.IsPaused)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            mCurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            TowerData towerData = tower.Data as TowerData;
            CooldownComponent cooldownComponent = tower.CooldownComponent;
            AmmoCapacityComponent ammoComponent = tower.AmmoCapacityComponent;
            AudioComponent audioComponent = tower.AudioComponent;
            Vector2 towerOrigin = new Vector2(mPosition.X + TileMap.TileWidth / 2, mPosition.Y + TileMap.TileHeight / 2);

            //tower can only be in one cell
            int currentCell = tower.BoundsCollisionComponent.Cells.FirstOrDefault();

            List<int> cells = CollisionHelper.GetSurroundingCells(currentCell);

            List<CollisionComponent> nearbyComponents = new List<CollisionComponent>();
                
            foreach(int cell in cells)
                nearbyComponents.AddRange(CollisionHelper.GetNearby(cell));
                
            List<CollisionComponent> entities = nearbyComponents.Where(e => e.Owner is Unit).ToList();
            List<Unit> units = entities.Select(e => e.Owner as Unit).ToList();
            List<UnitPositionComponent> unitPositions = units.Select(c => c.PositionComponent).ToList();

            UnitPositionComponent closestUnit = null;
            float distance = float.MaxValue;

            foreach (UnitPositionComponent unitPosition in unitPositions)
            {
                Vector2 unitOrigin = new Vector2(unitPosition.X + TileMap.TileWidth / 2, unitPosition.Y + TileMap.TileHeight / 2);
                float dist = Vector2.Distance(towerOrigin, unitOrigin);
                if (dist < distance)
                {
                    distance = dist;
                    closestUnit = unitPosition;
                }
            }

            if (distance > towerData.FieldOfView)
            {
                return;
            }
            if (closestUnit != null)
            {
                Vector2 unitOrigin = new Vector2(closestUnit.X + TileMap.TileWidth / 2, closestUnit.Y + TileMap.TileHeight / 2);
                mRotation = (float)GeometryOps.AngleBetweenTwoVectors(unitOrigin, towerOrigin);


                if (mCurrentTime <= 0.0f && !cooldownComponent.IsCooling)
                {
                    tower.AnimationComponent.TowerState = TowerState.Firing;
                    tower.AnimationComponent.TargettedEntityId = closestUnit.Owner.Id;
                    tower.AudioComponent.Play();

                    mShotsFired++;

                    if (mBurstMode && mShotsFired < mShotsPerFire)
                    {
                        mCurrentTime = mBurstTime;
                    }
                    else
                    {
                        mCurrentTime = towerData.FireRate;
                        mShotsFired = 0;
                    }
                }
            }


        }
    }
}
