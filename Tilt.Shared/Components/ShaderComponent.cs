using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{

    public class ShaderComponent : EffectComponent
    {
        public ShaderComponent(string shaderName, Entity owner, bool register = true) : base(owner, register)
        {
            // mEffect = AssetOps.LoadSharedAsset<Effect>(shaderName);
        }

    }

}
