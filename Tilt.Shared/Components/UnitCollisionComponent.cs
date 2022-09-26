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
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Components
{
    public class UnitCollisionComponent : BoundsCollisionComponent
    {
        private static readonly int kBulletUnitDistance = (int)(TileMap.TileWidth*2);
        private bool mCollideable = true;

        public UnitCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            PositionComponent unitPosition = (Owner as Unit).PositionComponent;
            mBounds.X = (int) unitPosition.X;
            mBounds.Y = (int) unitPosition.Y;

            foreach (int cell in Cells)
            {
                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);
                foreach (CollisionComponent component in nearbyComponents)
                {
                    if (component == this || !(component.Owner is Projectile))
                        continue;

                    Projectile projectile = component.Owner as Projectile;
                    Unit unit = Owner as Unit;
                    
                    Vector2 unitOrigin = new Vector2(mBounds.X + mBounds.Width / 2, mBounds.Y + mBounds.Height / 2);

                    Vector2 bulletOrigin = projectile.CollisionComponent.Origin;

                    if ((projectile.CollisionComponent is PointCollisionComponent && 
                        GeometryOps.Intersects(((PointCollisionComponent)projectile.CollisionComponent).Points, unit.BoundsCollisionComponent.Bounds)) ||
                        (Vector2.Distance(bulletOrigin, unitOrigin) < kBulletUnitDistance &&
                        projectile.CollisionComponent is BoundsCollisionComponent &&
                        GeometryOps.Intersects(((BoundsCollisionComponent)projectile.CollisionComponent).Bounds, unit.BoundsCollisionComponent.Bounds)))
                    {
                        ////apply buff and take damage
                        if (unit.BuffComponent.ApplyBuff(projectile.ProjectileType))
                        {
                            //possible bug: change health to float now that we have a multiplier
                            unit.HealthComponent.Health -= (int)projectile.Data.Damage;
                        }
                        else
                        {

                            unit.HealthComponent.Health -= (int)projectile.Data.Damage;

                        }

                        if(!(projectile is Laser))
                            projectile.UnRegister();

                        EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                        {
                            Play = true,
                            SoundEffect = "sfx_enemy_hit",
                            Volume = 0.5f
                            
                        });


                        if (unit.HealthComponent.Health <= 0 && mCollideable)
                        {
                            Resources.UnitsDestroyedOverLevel++;
                            mCollideable = false;
                        }

                        UnitDamageTimerComponent timerComponent = unit.DamageTimerComponent;
                        timerComponent.Start();
                    }

                }
            }
        }
    }
}
