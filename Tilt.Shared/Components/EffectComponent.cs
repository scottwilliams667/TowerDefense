using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Components
{
    public class EffectComponent : Component
    {
        private LayerType mRegisteredLayer;
        protected Effect mEffect;
        public EffectComponent(Entity owner, bool register = true)
            : base(owner, register)
        {
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
        }

    }

}
