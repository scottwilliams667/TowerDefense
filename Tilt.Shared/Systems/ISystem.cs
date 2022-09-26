using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Systems
{
    public interface ISystem
    {
        void Update();
        void Register(Component component);
        void UnRegister(Component component);
    }
}
