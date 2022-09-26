using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Components
{
    public class HealthComponent : Component
    {
        private int mHealth;
        private int mFullHealth;

        public HealthComponent(int health, Entity owner)
            : base(owner, false)
        {
            mHealth = health;
            mFullHealth = health;
        }


        public virtual int Health
        {
            get { return mHealth; }
            set { mHealth = value; }
        }

        public float HealthPercentage
        {
            get { return (float)Decimal.Divide(mHealth, mFullHealth); }
        }

    }

    public class ResistantHealthComponent : HealthComponent
    {
        private bool mIsResisting;

        public ResistantHealthComponent(int health, Entity owner)
            : base(health, owner)
        {
        }

        public bool IsResisting
        {
            get { return mIsResisting; }
            set { mIsResisting = value; }
        }

        //resist damage if boosted by shield generators
        public override int Health
        {
            get { return base.Health; }
            set
            {
                int damage = value;

                if (IsResisting)
                    damage--;

                base.Health = damage;
            }
        }
    }
}
