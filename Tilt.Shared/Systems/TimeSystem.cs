using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Systems
{
    public class TimeSystem : ISystem
    {
        private List<Component> mComponents = new List<Component>();
        public void Update()
        {
            foreach(Component component in mComponents.ToList())
                component.Update();
        }

        public void Register(Component component)
        {
            mComponents.Add(component);
        }

        public void UnRegister(Component component)
        {
            mComponents.Remove(component);
        }
    }
}
