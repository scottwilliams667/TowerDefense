using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Components
{
    class TextPositionComponent : PositionComponent
    {
        public TextPositionComponent(int x, int y, Entity owner) : base(x, y, owner)
        {
        }

        public override void Update()
        {
            
        }
    }
}
