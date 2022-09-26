using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Systems
{
    public class RenderSystem : ISystem
    {
        private List<Component> mComponents = new List<Component>();

        public void Register(Component component)
        {
            mComponents.Add(component);
        }

        public void UnRegister(Component component)
        {
            mComponents.Remove(component);
        }

        public List<Component> Components
        {
            get { return mComponents; }
            set { mComponents = value; }
        }


        public void Update()
        {
            foreach(Component component in mComponents.ToList())
            {
                component.Update();
            }
        }
    }
}
