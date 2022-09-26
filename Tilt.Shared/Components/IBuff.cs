using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    interface IDpsBuff
    {
        int Value { get; }

        float Duration { get; }
    }

    public interface IBuff
    {
        ProjectileType Type { get; }

        /// The Id of the Entity that this buff is being applied to
        uint OwnerId { get; }

        void Apply();

        void Remove();

        void Update();

        void Reset();
    }

    public class Buff : IBuff
    {
        private ProjectileType mBuffType;
        private uint mOwnerId;

        public Buff(ProjectileType buffType, uint ownerId)
        {
            mBuffType = buffType;
            mOwnerId = ownerId;
        }

        public ProjectileType Type { get { return mBuffType; } }

        public uint OwnerId { get { return mOwnerId;} }

        public virtual void Update()
        {   
        }

        public virtual void Apply()
        {
        }

        public virtual void Remove()
        {
        }

        public virtual void Reset()
        {
        }
    }

    public class DpsBuff : Buff, IDpsBuff
    {
        private const float kOneSecond = 1.0f;
        private float mDuration;
        private int mValue;
        private float mTimeLeft;
        private float mOneSecondTimer = kOneSecond;

        public DpsBuff(int value, float duration, ProjectileType type, uint ownerId)
            : base(type, ownerId)
        {
            mDuration = duration;
            mValue = value;
            mTimeLeft = duration;

        }

        public float Duration { get { return mDuration; } }

        public int Value { get { return mValue; } }

        protected bool IsTimerDepleted
        {
            get { return mTimeLeft <= 0.0f; }
        }

        protected bool IsOneSecondDepleted
        {
            get { return mOneSecondTimer <= 0.0f; }
        }

        public override void Update()
        {
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            //check if one second has gone by
            if (mOneSecondTimer <= 0.0f)
            {
                mDuration -= kOneSecond;
                mOneSecondTimer = kOneSecond;

            }

            mTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            mOneSecondTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;

        }

        public override void Reset()
        {
            mTimeLeft = mDuration;
            mOneSecondTimer = kOneSecond;
        }
    }

    public class UnitFireBuff : DpsBuff
    {
        public UnitFireBuff(int value, float duration, ProjectileType type, uint ownerId)
            : base(value, duration, type, ownerId)
        {
        }

        public override void Apply()
        {
            //do some preliminary applying of the buff.
        }

        public override void Remove()
        {
            Unit unit = LayerManager.Layer.EntitySystem.GetEntityById(OwnerId) as Unit;
            unit.BuffComponent.RemoveBuff(this);
        }

        public override void Update()
        {
            base.Update();

            // take damage ever 1000 ticks for a duration of time
            if (IsOneSecondDepleted)
            {
                Unit unit = LayerManager.Layer.EntitySystem.GetEntityById(OwnerId) as Unit;

                if (unit == null)
                    return;

                HealthComponent healthComponent = unit.HealthComponent;
                healthComponent.Health -= Value;

                if (healthComponent.Health <= 0)
                {
                    unit.UnRegister();
                    EventSystem.EnqueueEvent(EventType.UnitDestroyed, unit, null);
                }
            }

            if (IsTimerDepleted)
            {
                Unit unit = LayerManager.Layer.EntitySystem.GetEntityById(OwnerId) as Unit;
                if(unit != null)
                    unit.BuffComponent.RemoveBuff(this);
            }
        }
    }
}
