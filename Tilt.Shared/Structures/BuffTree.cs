using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Structures
{
    public static class BuffTree
    {
        public static void DetermineDeadBuffs(ProjectileType buffToApply, List<Buff> buffs)
        {
            foreach (Buff buff in buffs.ToList())
            {
                if (buff.Type == buffToApply)
                {
                    buffs.Remove(buff);
                }

                //if (buff.Type == ProjectileType.Fire && buffToApply == ProjectileType.Ice)
                //{
                //    buffs.Remove(buff);
                //}
                if (buff.Type == ProjectileType.Fire && buffToApply == ProjectileType.Fire)
                {
                    buffs.Remove(buff);
                }
            }
        }
    }
}
