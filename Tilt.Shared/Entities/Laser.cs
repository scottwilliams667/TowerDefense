using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{

    public class Laser : Projectile
    {
        private LaserAnimationComponent mAnimationComponent;
        private LaserDebugRenderComponent mDebugComponent;

        public Laser(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, float rotation, ProjectileData projectileData)
            : base(ProjectileType.Laser)
        {
            PositionComponent = new LaserPositionComponent(x, y, rotation, this);
            CollisionComponent = new LaserCollisionComponent(
                new Vector2(x, y),
                new Vector2(x + 192, y),
                new Vector2(x, y + 16),
                new Vector2(x + 192, y + 16),
                new Vector2(x,y),
                this);
            mAnimationComponent = new LaserAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            Data = projectileData;
        }

        public LaserAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            CollisionComponent.UnRegister();
            mAnimationComponent.UnRegister();
            Data = null;
            base.UnRegister();
        }
    }


    public class LaserPositionComponent : PositionComponent
    {
        private const int kSpeed = 675;
        private Vector2 mDirection;
        private Vector2 mLaunchPosition;
        private float mRotation;

        public LaserPositionComponent(int x, int y, float rotation, Entity owner, Vector2 origin = new Vector2())
            : base(x, y, owner, origin)
        {
            mDirection = GeometryOps.Angle2Vector(rotation);
            mLaunchPosition = new Vector2(x, y);
            Speed = kSpeed;
            mRotation = rotation;
        }

        public Vector2 LaunchPosition
        {
            get { return mLaunchPosition; }
        }

        public Vector2 Direction
        {
            get { return mDirection; }
        }

        public float Rotation
        {
            get { return mRotation; }
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            Laser laser = Owner as Laser;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            mPosition += mDirection*Speed*(float) gameTime.ElapsedGameTime.TotalSeconds;

            ProjectileData projectileData = laser.Data;

            if (Vector2.Distance(mLaunchPosition, mPosition) > projectileData.DestructDistance)
            {
                laser.UnRegister();
            }
        }
    }

    public class LaserAnimationComponent : AnimationComponent
    {
        private float mSize = 0.0f;

        public LaserAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            Laser laser = Owner as Laser;
            LaserPositionComponent positionComponent = laser.PositionComponent as LaserPositionComponent;

            Vector2 launchPosition = positionComponent.LaunchPosition;
            Vector2 position = positionComponent.Position;

            double angle = GeometryOps.AngleBetweenTwoVectors(launchPosition, position);
            double dist = Vector2.Distance(launchPosition, position);

            decimal sizeX = 0;

            TowerData laserTowerData = ObjectFactory.GetDataForTower(TowerType.Laser);

            if(dist > laserTowerData.FieldOfView/2)
            {
                sizeX = Decimal.Divide((decimal)laserTowerData.FieldOfView/2, SourceRectangle.Width);
            }
            else
                sizeX = Decimal.Divide((decimal)dist, SourceRectangle.Width);

            

            
            spriteBatch.Draw(mTexture, position + new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2), CurrentRectangle,  Color.White,
                (float)angle, new Vector2(0,8), new Vector2((float)sizeX, 1.0f), SpriteEffects.None, 0.35f  );



            //Vector2 launchPosition = positionComponent.LaunchPosition;
            //Vector2 position = positionComponent.Position;

            //float distance = Vector2.Distance(launchPosition, position);
            



            
            //if(distance >= 64)
            //{
            //    mSize = 1.0f;
            //    positionComponent.Speed = 150;
            //}
            //else
            //{
            //    mSize += 0.03f;
            //    positionComponent.Speed = 25;
            //}

            //if (mSize < 0.33)
            //{
            //    CurrentRectangle = new Rectangle(SourceRectangle.X + (int)((CurrentColumnIndex * SourceRectangle.Width) * mSize), SourceRectangle.Y + (0 * SourceRectangle.Height),
            //        SourceRectangle.Width, SourceRectangle.Height);
            //}
            //if (mSize < 0.66)
            //{
            //    CurrentRectangle = new Rectangle(SourceRectangle.X + (int)((CurrentColumnIndex * SourceRectangle.Width) * mSize), SourceRectangle.Y + (1 * SourceRectangle.Height),
            //        SourceRectangle.Width, SourceRectangle.Height);
            //}
            //if (mSize < 1.0)
            //{
            //    CurrentRectangle = new Rectangle(SourceRectangle.X + (int)((CurrentColumnIndex * SourceRectangle.Width) * mSize), SourceRectangle.Y + (2 * SourceRectangle.Height),
            //        SourceRectangle.Width, SourceRectangle.Height);
            //}
            //else
            //{
            //    CurrentRectangle = new Rectangle(SourceRectangle.X + (int)((CurrentColumnIndex * SourceRectangle.Width) * mSize), SourceRectangle.Y + (3 * SourceRectangle.Height),
            //        SourceRectangle.Width, SourceRectangle.Height);
            //}

            //spriteBatch.Draw(mTexture, positionComponent.Position, CurrentRectangle, Color.White,
            //    positionComponent.Rotation, Vector2.Zero, 1.0f,
            //    SpriteEffects.None, 0.35f);

        }
    }

    public class LaserDebugRenderComponent : RenderComponent
    {
        public LaserDebugRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
        }

        public override void Update()
        {
            Laser laser = Owner as Laser;
            PointCollisionComponent collisionComponent = laser.CollisionComponent as PointCollisionComponent;
            LaserPositionComponent position = laser.PositionComponent as LaserPositionComponent;
            LaserAnimationComponent animationComponent = laser.AnimationComponent as LaserAnimationComponent;

            Vector2 point1 = collisionComponent.Point1;
            Vector2 point2 = collisionComponent.Point2;
            Vector2 point3 = collisionComponent.Point3;
            Vector2 point4 = collisionComponent.Point4;

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            Rectangle sourceRectangle = animationComponent.SourceRectangle;

            Texture2D tex1 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex2 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex3 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex4 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);

            Int32[] pixel = { 0xFFFFFF };
            tex1.SetData<Int32>(pixel, 0, sourceRectangle.Width);
            tex2.SetData<Int32>(pixel, 0, sourceRectangle.Height);
            tex3.SetData<Int32>(pixel, 0, sourceRectangle.Width);
            tex4.SetData<Int32>(pixel, 0, sourceRectangle.Height);

            spriteBatch.Draw(tex1, new Rectangle((int)point1.X, (int)point1.Y, sourceRectangle.Width, 1),
                null, Color.Green, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(tex2, new Rectangle((int)point3.X, (int)point3.Y, sourceRectangle.Width, 1),
                null, Color.Green, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(tex3, new Rectangle((int)point1.X, (int)point1.Y, 1, sourceRectangle.Height),
                null, Color.Green, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(tex4, new Rectangle((int)point2.X, (int)point2.Y, 1, sourceRectangle.Height),
                null, Color.Green, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);
        }
    }

    public class LaserCollisionComponent : PointCollisionComponent
    {
        public LaserCollisionComponent(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 origin, Entity owner) : base(p1, p2, p3, p4, origin, owner)
        {
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            Laser laser = Owner as Laser;
            LaserPositionComponent positionComponent = laser.PositionComponent as LaserPositionComponent;
            LaserAnimationComponent animationComponent = laser.AnimationComponent as LaserAnimationComponent;

            if (positionComponent == null)
                return;

            Vector2 position = positionComponent.Position;
            float rotation = positionComponent.Rotation;

            Rectangle sourceRectangle = animationComponent.SourceRectangle;

            var pos1 = new Vector2(position.X, position.Y);
            var pos2 = new Vector2(position.X + sourceRectangle.Width, position.Y);
            var pos3 = new Vector2(position.X, position.Y + sourceRectangle.Height);
            var pos4 = new Vector2(position.X + sourceRectangle.Width, position.Y + sourceRectangle.Height);
            var origin = new Vector2(position.X, position.Y);

            pos1 = GeometryOps.RotateAroundPoint(pos1, origin, positionComponent.Rotation);
            pos2 = GeometryOps.RotateAroundPoint(pos2, origin, positionComponent.Rotation);
            pos3 = GeometryOps.RotateAroundPoint(pos3, origin, positionComponent.Rotation);
            pos4 = GeometryOps.RotateAroundPoint(pos4, origin, positionComponent.Rotation);


            Point1 = new Vector2(pos1.X, pos1.Y );
            Point2 = new Vector2(pos2.X, pos2.Y );
            Point3 = new Vector2(pos3.X, pos3.Y );
            Point4 = new Vector2(pos4.X, pos4.Y );

        }
    }

}



