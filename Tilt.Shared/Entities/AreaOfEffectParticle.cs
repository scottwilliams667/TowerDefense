using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.Shared.Components;

namespace Tilt.Shared.Entities
{
    public class AreaOfEffectParticle
        : Entity
    {
        private AreaOfEffectAnimationComponent mAnimationComponent;
        private PositionComponent mPositionComponent;

        public AreaOfEffectParticle(int x, int y, string texturePath, Rectangle sourceRectangle, int rows, int columns, float interval)
        {
            mAnimationComponent = new AreaOfEffectAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mPositionComponent = new PositionComponent(x, y, this);
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            mPositionComponent.UnRegister();
            base.UnRegister();
        }

        public AreaOfEffectAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }
    }
}
