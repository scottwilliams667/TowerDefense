using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class TileTouchComponent : TouchAreaComponent
    {
        public TileTouchComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Tile tile = Owner as Tile;
        }
    }
}
