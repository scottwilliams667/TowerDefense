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
    public class TowerCollisionComponent : BoundsCollisionComponent
    {
        public TowerCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;
            PositionComponent positionComponent = tower.PositionComponent;

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);

            if (tile.IsTowerPlaced || SystemsManager.Instance.IsPaused)
                return;


            foreach (int cell in Cells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);

                foreach(CollisionComponent component in nearbyComponents)
                {
                    if (!(component.Owner is Unit))
                        continue;

                    Unit unit = component.Owner as Unit;
                    UnitPositionComponent unitPosition = unit.PositionComponent;
                    UnitAnimationComponent animationComponent = unit.RenderComponent;
                    UnitData unitData = unit.Data;

                    HealthComponent healthComponent = tower.HealthComponent;

                    if(Vector2.Distance(positionComponent.Position, unitPosition.Position) < TileMap.TileWidth)
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


                        //unit.UnRegister();
                        //EventSystem.EnqueueEvent(EventType.UnitDestroyed, unit, null);

                        // healthComponent.Health -= unit.Data.Damage;
                    }

                    //if (healthComponent.Health <= 0)
                    //{
                    //    EventSystem.EnqueueEvent(EventType.TowerDestroyed, Owner, null);
                    //}
                }
                


            }

            
        }
    }
}
