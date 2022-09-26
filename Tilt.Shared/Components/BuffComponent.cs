using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Components
{

    public class BuffComponent : TimerComponent
    {
        private List<Buff> mBuffs = new List<Buff>();
        public BuffComponent(Entity owner, bool register = true)
            : base(owner, register)
        {
        }

        public override void UnRegister()
        {
            mBuffs.Clear();
        }

        public bool ApplyBuff(ProjectileType projectileType)
        {
            Buff alreadyAppliedBuff = mBuffs.FirstOrDefault(b => b.Type == projectileType);

            if (alreadyAppliedBuff != null)
            {
                alreadyAppliedBuff.Reset();
                return false;
            }

            


            BuffTree.DetermineDeadBuffs(projectileType, mBuffs);
            //BUG: fire towers next to each other with their fires overlapping will instantly kill a creature.
            //This is due to the projectile Id switching every frame. Maybe best to hold a list of projectileId's ?
            //maybe just reset the timer if same tower type?
            Buff buff = BuffFactory.GenerateBuffForEntity(projectileType, Owner);

            if (buff == null)
                return false;

            mBuffs.Add(buff);

            return true;
        }

        public void RemoveBuff(Buff buff)
        {
            mBuffs.Remove(buff);
        }

        public void RemoveBuff(ProjectileType type)
        {
            Buff buff = mBuffs.FirstOrDefault(b => b.Type == type);
            mBuffs.Remove(buff);
        }

        public override void Update()
        {
            foreach (Buff buff in mBuffs.ToList())
            {
                buff.Update();
            }

            base.Update();
        }
    }

}
