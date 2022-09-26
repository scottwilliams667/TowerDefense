using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    /*
     * This class is responsible for dealing with buying towers
     * from the tower select menu onto the game. It is also responsible for selling
     * towers as well. The MenuManager is responsible
     * for creating this object, and once a drop is complete, this class
     * removes itself by calling UnRegister. 
     */
    public class TowerSynchronizer : Entity
    {
        private TowerSyncTouchComponent mTouchComponent;
        private ObjectType mObjectType;
        private TowerType mSecondaryType;
        private AddOnType mAddOnType;

        public TowerSynchronizer(ObjectType objectType)
        {
            mObjectType = objectType;

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            Rectangle bounds = new Rectangle(0,0, viewport.Width * 2/3, viewport.Height);

            mTouchComponent = new TowerSyncTouchComponent(bounds, this);
            
        }

        public ObjectType Type
        {
            get { return mObjectType; }
            set { mObjectType = value; }
        }

        public TowerType TowerType
        {
            get { return mSecondaryType; }
            set { mSecondaryType = value; }
        }

        public AddOnType AddOnType
        {
            get { return mAddOnType; }
            set { mAddOnType = value; }
        }

        public override void UnRegister()
        {
            mTouchComponent.UnRegister();
            base.UnRegister();
        }


        public TowerSyncTouchComponent TouchComponent
        {
            get { return mTouchComponent; }
            set { mTouchComponent = value; }
        }

    }
}
