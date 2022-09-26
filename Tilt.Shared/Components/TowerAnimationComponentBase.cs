using System;
using System.Collections.Generic;
using System.Text;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;

namespace Tilt.Shared.Components
{
    public class TowerAnimationComponentBase : Component
    {
        protected LayerType mRegisteredLayer;
        protected TowerState mTowerState;
        protected AnimationState mIdleState;
        protected AnimationState mFireState;
        protected AnimationState mState;

        private ulong mTargettedEntityId;

        private bool mShowSmokeFromDamage;

        public TowerAnimationComponentBase(Entity owner, bool register = true)
            : base(owner, register)
        {
        }

        public AnimationState State
        {
            get { return mState; }
        }

        public TowerState TowerState
        {
            get { return mTowerState; }
            set { mTowerState = value; }
        }


        public ulong TargettedEntityId
        {
            get { return mTargettedEntityId; }
            set { mTargettedEntityId = value; }
        }

        public bool ShowSmokeFromDamage
        {
            get { return mShowSmokeFromDamage; }
            set { mShowSmokeFromDamage = value; }
        }


    }

}
