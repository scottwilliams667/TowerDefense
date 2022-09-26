using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class BulletPositionComponent : PositionComponent
    {
        private const int kSpeed = 465;
        private Vector2 mDirection;
        private Vector2 mLaunchPosition;

        public BulletPositionComponent(int x, int y, float rotation, Entity owner) : base(x, y, owner)
        {
            mDirection = GeometryOps.Angle2Vector(rotation);
            mLaunchPosition = new Vector2(x,y);
            Speed = kSpeed;
        }

        public Vector2 Direction
        {
            get { return mDirection;}
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Bullet bullet = Owner as Bullet;
            mPosition += mDirection*Speed*(float) gameTime.ElapsedGameTime.TotalSeconds;
            BoundsCollisionComponent collisionComponent = bullet.CollisionComponent as BoundsCollisionComponent;
            Rectangle bulletBounds = collisionComponent.Bounds;
            collisionComponent.Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, bulletBounds.Width, bulletBounds.Height );
            ProjectileData projectileData = bullet.Data;
            if (Vector2.Distance(mLaunchPosition, mPosition) > projectileData.DestructDistance)
            {
                bullet.UnRegister();
            }

            //do some logic if the bullet gets too far away from the tower
        }
    }
}
