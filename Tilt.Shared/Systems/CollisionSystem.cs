using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Systems
{
    public class CollisionSystem : ISystem
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

            CollisionHelper.ClearCells();
            foreach(CollisionComponent component in mComponents.ToList())
            {
                List<int> cells = CollisionHelper.Register(component);
                component.Cells = cells;
            }

            foreach (CollisionComponent component in mComponents.ToList())
            {
                component.Update();
            }

        }
    }
}
