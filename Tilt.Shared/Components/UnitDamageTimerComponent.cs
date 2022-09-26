using System;
using System.Collections.Generic;
using System.Text;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;

namespace Tilt.Shared.Components
{
    public class UnitDamageTimerComponent
        : TimerComponent
    {
        private int mSpeed;

        public UnitDamageTimerComponent(float timeInSeconds, Entity owner, bool register = true) : base(owner, register)
        {
            mTimeSet = timeInSeconds;
            mTimeLeft = timeInSeconds;
            base.Stop_();
        }

        protected override void Start_()
        {
            if (!IsStopped)
                return;

            Unit unit = Owner as Unit;
            UnitPositionComponent unitPositionComponent = unit.PositionComponent;
            UnitAnimationComponent unitAnimationComponent = unit.RenderComponent;

            mSpeed = unitPositionComponent.Speed;
            unitAnimationComponent.IsDamaged = true;

            unitPositionComponent.Speed = 0;


            base.Start_();
        }

        protected override void Done_()
        {
            Unit unit = Owner as Unit;
            UnitPositionComponent unitPositionComponent = unit.PositionComponent;
            UnitAnimationComponent unitAnimationComponent = unit.RenderComponent;
            HealthComponent unitHealthComponent = unit.HealthComponent;

            //only reset if unit can still continue
            //otherwise allow unit to fully fade and unregister()
            if (unitHealthComponent.Health > 0)
            {
                unitAnimationComponent.IsDamaged = false;
                unitPositionComponent.Speed = mSpeed;
            }
            
            base.Reset_();
            base.Stop_();
        }

        public void Start()
        {
            Start_();
        }
    }
}
