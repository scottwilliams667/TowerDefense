using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Components
{
   public class PositionComponent : Component
    {
       private LayerType mRegisteredLayer;
       protected Vector2 mPosition;
       protected Vector2 mOrigin;
       protected int mSpeed;

       public PositionComponent(int x, int y, Entity owner, Vector2 origin = default(Vector2)) : base(owner)
       {
            mPosition = new Vector2(x,y);
            mOrigin = origin;
       }

       public Vector2 Position
       {
           get { return mPosition;;}
           set { mPosition = value; }
       }

       public Vector2 Origin
       {
           get { return mOrigin; }
           set { mOrigin = value; }
       }

       public float X
       {
           get { return mPosition.X; }
           set { mPosition.X = value; }
       }

       public float Y
       {
           get { return mPosition.Y; }
           set { mPosition.Y = value; }
       }

       public int Speed
       {
           get { return mSpeed; }
           set { mSpeed = value; }
       }

       public override void Register()
       {
           mRegisteredLayer = LayerManager.Layer.Type;
           LayerManager.Layer.PositionSystem.Register(this);
       }

       public override void UnRegister()
       {
           LayerManager.GetLayer(mRegisteredLayer).PositionSystem.UnRegister(this);
       }

       public override void Update()
       {
           
       }
    }
}
