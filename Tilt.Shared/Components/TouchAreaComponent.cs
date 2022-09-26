using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class TouchAreaComponent : InputComponent
    {
        protected Rectangle mBounds;

        public TouchAreaComponent(Rectangle bounds, Entity owner, bool register = true) : base(owner, register) 
        {
            mBounds = bounds;
        }

        public Rectangle Bounds
        {
            get { return mBounds; }
            set { mBounds = value; }
        }

        public override void Update()
        {
            
        }
    }

    public class ButtonTouchComponent : TouchAreaComponent
    {
        public ButtonTouchComponent(Rectangle bounds, Entity owner, bool register = true) : base(bounds, owner, register)
        {
        }

        public override void Update()
        {
            Button button = Owner as Button;
            ButtonAnimationComponent buttonAnimationComponent = button.AnimationComponent;
            AudioComponent audioComponent = button.AudioComponent;

#if !WINDOWS
            if (TouchOps.IsTap() && TouchOps.ContainsPoint(mBounds))
#else 
            if(MouseOps.IsClick() && MouseOps.ContainsPoint(mBounds))
#endif 
            {
                if (!buttonAnimationComponent.IsEnabled || 
                    !buttonAnimationComponent.IsVisible ||
                    buttonAnimationComponent.IsStarted)
                    return;

                buttonAnimationComponent.Start();

                if(audioComponent != null)
                    audioComponent.Play();

#if !WINDOWS
                TouchOps.ClearTouch();
#endif
            }

        }
    }
}
