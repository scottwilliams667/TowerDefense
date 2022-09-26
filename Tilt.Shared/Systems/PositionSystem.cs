using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Systems
{
    public class PositionSystem : ISystem
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
            //foreach(Component component in mComponents.ToList())
            //{
            //    component.Update();
            //}

            ///ToList throws a java exception in the Xamarin run time...
            /// have to do it this way instead. (investigate later)
            for(int i = mComponents.Count - 1; i >= 0; i--)
            {
                Component component = mComponents[i];
                component.Update();
            }

        }
    }
}
