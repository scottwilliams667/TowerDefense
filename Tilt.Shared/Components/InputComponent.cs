using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Components
{
    public class InputComponent : Component
    {
        private LayerType mRegisteredLayer;
        public InputComponent(Entity owner, bool register = true) : base(owner, register)
        {
        }
        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.InputSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).InputSystem.UnRegister(this);
        }

        public override void Update()
        {
            
        }
    }
}
