using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class Entity
    {
        private ulong mId;
        private LayerType mRegisteredLayer;

        public Entity()
        {
            mId = IdGenerator.GetId();
            Register();
            
        }

        public virtual void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.EntitySystem.Register(this);
        }

        public virtual void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).EntitySystem.UnRegister(this);
        }
        
        public ulong Id
        {
            get { return mId; }
        }

    }
}
