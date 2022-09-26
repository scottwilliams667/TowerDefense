using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Entities
{
    public enum SelectionMode
    {
        Build,
        Normal,
        Sell
    }

    public class SelectionBox : Entity
    {
        private SelectionBoxAnimationComponent mAnimationComponent;
        private SelectionBoxTouchComponent mTouchComponent;

        public SelectionBox(Rectangle bounds,string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns)
        {
            mAnimationComponent = new SelectionBoxAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);   
            mTouchComponent = new SelectionBoxTouchComponent(bounds, this);
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mTouchComponent.UnRegister();
            base.UnRegister();
        }

        public SelectionBoxAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }
    }
}
