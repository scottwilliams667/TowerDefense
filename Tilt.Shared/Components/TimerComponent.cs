using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Components
{
    public class TimerComponent : Component
    {
        protected float mTimeSet;
        protected float mTimeLeft;
        private bool mIsStopped;
        private LayerType mRegisteredLayer;

        public TimerComponent(Entity owner, bool register = true) : base(owner, register)
        {
        }

        public bool IsStopped
        {
            get { return mIsStopped;}
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.TimeSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).TimeSystem.UnRegister(this);
        }

        public override void Update()
        {
            if (mIsStopped)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            mTimeLeft -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimeLeft <= 0)
            {
                Done_();
            }
            base.Update();
        }

        protected virtual void Reset_()
        {
            mTimeLeft = mTimeSet;
        }

        protected virtual void Stop_()
        {
            mIsStopped = true;
        }

        protected virtual void Start_()
        {
            mIsStopped = false;
        }

        protected virtual void Done_()
        {
        }
    }

    public class EventTimerComponent : TimerComponent
    {
        private EventType mEventType;

        public EventTimerComponent(float timeInSeconds, EventType eventType, Entity owner, bool register = true) : base(owner, register)
        {
            mTimeSet = timeInSeconds;
            mTimeLeft = timeInSeconds;
            mEventType = eventType;
        }

        protected override void Done_()
        {
            EventSystem.EnqueueEvent(mEventType, Owner, null);
            Stop_();
            Reset_();
            Start_();
        }

        public void Stop()
        {
            Stop_();
        }

        public void Start()
        {
            Start_();
        }

        public void Reset()
        {
            Reset_();
        }
    }
}
