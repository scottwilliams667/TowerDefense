using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tilt.EntityComponent.Components
{
    interface IVisibilityComponent
    {
        bool IsVisible { get; set; }
    }
}
