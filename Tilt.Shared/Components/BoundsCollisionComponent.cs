using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class CollisionComponent : Component
    {
        private LayerType mRegisteredLayer;
        private List<int> mCells;

        public CollisionComponent(Entity owner, bool register = true)
            : base(owner, register)
        {
        }

        public List<int> Cells
        {
            get { return mCells; }
            set { mCells = value; }
        }

        public virtual Vector2 Origin { get { return Vector2.Zero; } }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.CollisionSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).CollisionSystem.UnRegister(this);
        }

        
    }

    public class BoundsCollisionComponent : CollisionComponent
    {
        protected Rectangle mBounds;

        public BoundsCollisionComponent(Rectangle bounds, Entity owner) : base(owner)
        {
            mBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        public Rectangle Bounds
        {
            get { return mBounds; }
            set { mBounds = value; }
        }

        public override Vector2 Origin
        {
            get { return new Vector2(mBounds.X + mBounds.Width / 2, mBounds.Y + mBounds.Height / 2); } 
        }

        public override void Update()
        {
        }
    }

    public class PointCollisionComponent : CollisionComponent
    {
        private Vector2 mOrigin;
        public PointCollisionComponent(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Entity owner) : base(owner)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
            Point4 = p4;
        }

        public PointCollisionComponent(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 origin, Entity owner) :
            this(p1, p2, p3, p4, owner)
        {
            mOrigin = origin;
        }

        /// Top Left
        public Vector2 Point1 { get; set; }
        /// Top Right
        public Vector2 Point2 { get; set; }
        /// Bottom Left
        public Vector2 Point3 { get; set; }
        /// Bottom Right
        public Vector2 Point4 { get; set; }

        public Vector2[] Points
        {
            get { return new Vector2[] { Point1, Point2, Point3, Point4}; }
        }

        public override Vector2 Origin
        {
            get { return mOrigin; }
        }

        public override void Update()
        {
            
        }
    }
}
