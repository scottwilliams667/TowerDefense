using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Utilities
{
    public class IdGenerator
    {
        public static ulong mCurrentId = 0;

        public static ulong GetId()
        {
            return mCurrentId++;
        }
    }
    
}
