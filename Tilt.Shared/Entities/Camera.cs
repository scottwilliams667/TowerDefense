using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    /* 
     * This class wraps a Camera2D object from MonoGame.Extended library.
     * 
     * All camera movement and updating is done in CameraPositionComponent.cs
     */
    public class Camera : Entity
    {
        private CameraPositionComponent mPositionComponent;
 
        public Camera(int x, int y, Viewport viewport)
        {
           mPositionComponent = new CameraPositionComponent(x,y, viewport, this);   
        }

        public override void UnRegister()
        {
            mPositionComponent.UnRegister();

            base.UnRegister();
        }


        public CameraPositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }



    }
}
