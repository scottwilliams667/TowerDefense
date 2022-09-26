using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Systems
{
    public class InputSystem  : ISystem
    {
        private List<Component> mComponents = new List<Component>();

        public void Register(Component component)
        {
            mComponents.Add(component);
        }

        public List<Component> Components
        {
            get { return mComponents; }
            set { mComponents = value; }
        }

        public void UnRegister(Component component)
        {
            mComponents.Remove(component);
        }

        public void Update()
        {
            foreach (Component component in mComponents.ToList())
            {
                component.Update();
            }
        }
    }
}
