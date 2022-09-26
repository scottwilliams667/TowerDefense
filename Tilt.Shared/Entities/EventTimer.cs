using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class EventTimer : Entity
    {
        private EventTimerComponent mTimer;

        public EventTimer(float timeInSeconds, EventType eventType)
        {
            mTimer = new EventTimerComponent(timeInSeconds, eventType, this);
        }

        public EventTimerComponent Timer
        {
            get { return mTimer; }
            set { mTimer = value; }
        }

    }
}
