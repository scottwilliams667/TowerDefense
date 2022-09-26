using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Components
{
    public class Component 
    {
        public Component(Entity owner, bool register = true)
        {
            if(register)
                Register();

            mOwner = owner;
        }

        private readonly Entity mOwner;

        public Entity Owner
        {
            get { return mOwner; }
        }

        public virtual void Update() { }
        public virtual void UnRegister() { }
        public virtual void Register() { }
    }
}
