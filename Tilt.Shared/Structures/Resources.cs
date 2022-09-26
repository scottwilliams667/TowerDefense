using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Structures
{
    public static class Resources
    {
        private static uint mMinerals;
        private static int mUnitsDestroyedOverLevel;
        private static int mUnitsDestroyedOverCampaign;
        private static int mResourcesSpentOverLevel;
        private static int mResourcesSpentOverCampaign;

        public static uint Minerals
        {
            get { return mMinerals; }
            set 
            {

                EventSystem.EnqueueEvent(EventType.MineralsChanged, null, new MineralsChangedArgs()
                {
                    OldValue = mMinerals,
                    NewValue = value
                });
                mMinerals = value; 
            }
        }

        public static int ResourcesSpentOverCampaign
        {
            get { return mResourcesSpentOverCampaign; }
            set { mResourcesSpentOverCampaign = value; }
        }

        public static int ResourcesSpentOverLevel
        {
            get { return mResourcesSpentOverLevel; }
            set { mResourcesSpentOverLevel = value; }
        }

        public static int UnitsDestroyedOverCampaign
        {
            get { return mUnitsDestroyedOverCampaign; }
            set { mUnitsDestroyedOverCampaign = value; }
        }

        public static int UnitsDestroyedOverLevel
        {
            get { return mUnitsDestroyedOverLevel; }
            set { mUnitsDestroyedOverLevel = value; }
        }
    }
}
