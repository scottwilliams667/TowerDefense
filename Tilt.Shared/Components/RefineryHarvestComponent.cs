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
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Components
{
    public class RefineryHarvestComponent : TimerComponent
    {
        private const float kOneSecond = 1.0f;
        private const int kRadius = 256;
        private float mTimer = kOneSecond;
        private bool mNoResources;
        private ResourcePile mCurrentResourcePile;
        private List<RefineryAddOn> mRefineryAddOns;



        public RefineryHarvestComponent(Entity owner, bool register = true) : base(owner, register)
        {
        }

        public bool IsTimerDepleted
        {
            get { return mTimer <= 0.0f; }
        }

        public bool IsRefining
        {
            get { return mCurrentResourcePile != null; }
        }

        public ResourcePile CurrentResourcePile
        {
            get { return mCurrentResourcePile; }
        }

        public List<RefineryAddOn> RefineryAddOns
        {
            get { return mRefineryAddOns; }
        }

        public override void Update()
        {
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Refinery refinery = Owner as Refinery;

            PositionComponent refineryPosition = refinery.PositionComponent;
            SimpleAudioComponent audioComponent = refinery.AudioComponent;

            if(SystemsManager.Instance.IsPaused)
            {
                //dont play refinery wave sfx if game is paused
                audioComponent.Played = true;
            }


            if (refinery == null || !refinery.Enabled || SystemsManager.Instance.IsPaused)
            {
                return;
            }

            
            TileCoord refineryTileCoord = GeometryOps.PositionToTileCoord(refineryPosition.Position);

            if (mCurrentResourcePile == null && !mNoResources)
            {
                ResourcePile resourcePile = FindResourcePiles_(refineryTileCoord);
                if (resourcePile != null)
                {
                    mCurrentResourcePile = resourcePile;
                    mTimer = kOneSecond;

                    if (!audioComponent.Played)
                    {
                        audioComponent.Play();
                    }

                }
                else
                {
                    //found no resource piles
                    //go idle
                    mNoResources = true;
                    mTimer = kOneSecond;

                    audioComponent.Stop();
                }
            }
            else if (mCurrentResourcePile != null)
            {
                if (IsTimerDepleted)
                {
                    ResourcePileData data = mCurrentResourcePile.Data as ResourcePileData;
                    if (data.Units > 0)
                    {
                        mRefineryAddOns = FindRefineryAddOns_();
                        data.Units--;
                        
                        //secret formula
                        uint price = (uint)( data.PricePerUnit + ((float)mRefineryAddOns.Count * 1.5));

                        Resources.Minerals += price;
                    }
                    else
                    {
                        //pile has no more resources. set it to null
                        mCurrentResourcePile.UnRegister();

                        TileCoord tileCoord =
                            GeometryOps.PositionToTileCoord(mCurrentResourcePile.PositionComponent.Position);

                        TileMap.RemoveObjectOnTile(tileCoord.X, tileCoord.Y);
                        mCurrentResourcePile = null;
                    }

                    mTimer = kOneSecond;
                }


                mTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
        }

        /// Finds the first resource pile around a given tile coordinate
        private ResourcePile FindResourcePiles_(TileCoord tileCoord)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int xp = tileCoord.X + i;
                    int yp = tileCoord.Y + j;
                    TileNode tileNode = TileMap.GetTileNode(xp, yp);

                    if (tileNode == null)
                        continue;

                    if (tileNode.Type == TileType.Occupied ||
                        tileNode.Type == TileType.Impassable)
                    {
                        IPlaceable obj = tileNode.Object;
                        if (obj == null)
                            continue;

                        if (obj.ObjectType == ObjectType.ResourcePile)
                        {
                            //check it has some resources
                            ResourcePile resourcePile = obj as ResourcePile;
                            ResourcePileData pileData = resourcePile.Data as ResourcePileData;
                            if (pileData.Units != 0)
                            {
                                return resourcePile;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private List<RefineryAddOn> FindRefineryAddOns_()
        {
            List<RefineryAddOn> refineryAddOns = new List<RefineryAddOn>();

            Refinery refinery = Owner as Refinery;

            List<int> surroundingCells = CollisionHelper.GetSurroundingCells(refinery.BoundsCollisionComponent.Cells.FirstOrDefault());
            foreach (int cell in surroundingCells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);
                List<CollisionComponent> refineryAddOnComponents = nearbyComponents.Where(c => c.Owner is RefineryAddOn).ToList();

                foreach (CollisionComponent collisionComponent in refineryAddOnComponents)
                {
                    RefineryAddOn addOn = collisionComponent.Owner as RefineryAddOn;
                    TileNode tileNode = TileMap.GetTileForPosition(addOn.PositionComponent.Position.X, addOn.PositionComponent.Position.Y);
                    if (Vector2.Distance(refinery.PositionComponent.Origin, addOn.PositionComponent.Origin) < Tuner.RefineryAddOnFieldOfView && 
                        tileNode != null && tileNode.Type == TileType.Occupied)
                    {
                        refineryAddOns.Add(addOn);
                    }
                }
            }

            return refineryAddOns;
        }
    }
}
