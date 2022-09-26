using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Components
{
    public class BulletCollisionComponent : BoundsCollisionComponent
    {
        public BulletCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            foreach (int cell in Cells)
            {
                List<CollisionComponent> entities = CollisionHelper.GetNearby(cell);
                foreach (CollisionComponent entity in entities)
                {
                    if (!(entity.Owner is Tower) || entity == this)
                        return;




                }
            }
        }
    }
}
