using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Entities
{
    public class Map : Entity
    {
        private MapRenderComponent mRenderComponent;
        private PositionComponent mPositionComponent;

        public Map(int x, int y, string texturePath)
        {
            mRenderComponent = new MapRenderComponent(texturePath, this);
            mPositionComponent = new PositionComponent(x,y,this);
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            mPositionComponent.UnRegister();
            base.UnRegister();
        }
    }
}
