using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class Rocket : Projectile
    {
        private RocketAnimationComponent mAnimationComponent;

        public Rocket(string texturePath, int x, int y, Rectangle sourceRectangle, int rows, int columns, float interval, float rotation, ProjectileData projectileData, ulong entityId )
            : base(ProjectileType.Rocket)
        {
            PositionComponent = new RocketPositionComponent(x, y, rotation, entityId, this);
            CollisionComponent = new RocketCollisionComponent(
                    new Vector2(x,y),
                    new Vector2(x + 46, y),
                    new Vector2(x, y + 8),
                    new Vector2(x + 46, y + 8),
                    new Vector2(x,y),
                    this);
            AnimationComponent = new RocketAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            Data = projectileData;
        }

        public RocketAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            PositionComponent.UnRegister();
            CollisionComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class RocketPositionComponent : PositionComponent
    {
        private float mRotation;
        private Vector2 mDirection;
        private Vector2 mLaunchPosition;
        private const int kSpeed = 150;
        private Unit mTargetedUnit;

        public RocketPositionComponent(int x, int y, float rotation, ulong entityId, Entity owner)
            : base(x,y, owner)
        {
            mDirection = GeometryOps.Angle2Vector(rotation);
            mRotation = rotation;
            mLaunchPosition = new Vector2(x, y);

            mTargetedUnit = LayerManager.Layer.EntitySystem.GetEntityById(entityId) as Unit;

            EventSystem.SubScribe(EventType.UnitDestroyed, OnUnitDestroyed_);
        }

        public override void UnRegister()
        {
            EventSystem.UnSubScribe(EventType.UnitDestroyed, OnUnitDestroyed_);
            base.UnRegister();
        }

        public float Rotation
        {
            get { return mRotation; }
        }

        public Vector2 LaunchPosition
        {
            get { return mLaunchPosition; }
        }

        public Unit TargettedUnit
        {
            get { return mTargetedUnit; }
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            Rocket rocket = Owner as Rocket;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();


            double angle = GeometryOps.AngleBetweenTwoVectors(Position, TargettedUnit.PositionComponent.Position);
            mDirection = GeometryOps.Angle2Vector((float)angle + (float)Math.PI); 

            mPosition += kSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

            ProjectileData data = rocket.Data;

            if(Vector2.Distance(mLaunchPosition, mPosition) > data.DestructDistance)
            {
                rocket.UnRegister();
            }
        }

        private void OnUnitDestroyed_(object sender, IGameEventArgs e)
        {
            Unit unit = sender as Unit;

            if (unit != null && unit == TargettedUnit)
            {
                Rocket rocket = Owner as Rocket;
                rocket.UnRegister();
            }
        }

    }

    public class RocketCollisionComponent : PointCollisionComponent
    {
        public RocketCollisionComponent(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 origin, Entity owner)
            : base(p1, p2, p3, p4, owner)
        {
        }

        public override void Update()
        {
            Rocket rocket = Owner as Rocket;
            RocketPositionComponent positionComponent = rocket.PositionComponent as RocketPositionComponent;
            RocketAnimationComponent animationComponent = rocket.AnimationComponent as RocketAnimationComponent;

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


            Point1 = new Vector2(pos1.X, pos1.Y);
            Point2 = new Vector2(pos2.X, pos2.Y);
            Point3 = new Vector2(pos3.X, pos3.Y);
            Point4 = new Vector2(pos4.X, pos4.Y);
        }
    }

    public class RocketAnimationComponent : AnimationComponent
    {
        private const float kTimerInterval = 0.1f;
        private float mTimerInterval = 0.0f;
        private float mShowGrayedOutRocketInterval = 0.1f;

        private bool mShowGrayedOutRocket;
        private Vector2 mOldPosition;

        private int mShowGrayedRocketCount = 0;
        private const int kShowGrayedRocketCount = 3;

        public RocketAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Rocket rocket = Owner as Rocket;
            RocketPositionComponent positionComponent = rocket.PositionComponent as RocketPositionComponent;
            Vector2 position = positionComponent.Position;

            double angle = GeometryOps.AngleBetweenTwoVectors(positionComponent.Position, positionComponent.TargettedUnit.PositionComponent.Position);

            spriteBatch.Draw(mTexture, position, CurrentRectangle, Color.White, (float)angle + (float)Math.PI, new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2), 1.0f, SpriteEffects.None, 0.35f);

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;

                if (CurrentColumnIndex >= Columns)
                {
                    CurrentColumnIndex = 0;
                }
            }

            CurrentRectangle = new Rectangle(SourceRectangle.X + CurrentColumnIndex * SourceRectangle.Width,
                       SourceRectangle.Y + CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            if (mShowGrayedRocketCount >= kShowGrayedRocketCount)
                return;

            mTimerInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(mTimerInterval <= 0.0f)
            {
                mShowGrayedOutRocket = true;
                mTimerInterval = kTimerInterval;
                mOldPosition = new Vector2(positionComponent.Position.X, positionComponent.Position.Y);
                
            }

            if(mShowGrayedOutRocket && mShowGrayedOutRocketInterval > 0.0f)
            {
                spriteBatch.Draw(mTexture, mOldPosition, SourceRectangle, Color.Gray, (float)angle + (float)Math.PI, new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2), 1.0f, SpriteEffects.None, 0.34f);
                mShowGrayedOutRocketInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if(mShowGrayedOutRocketInterval <= 0.0f)
            {
                mShowGrayedOutRocket = false;
                mShowGrayedRocketCount++;
                mShowGrayedOutRocketInterval = kTimerInterval;
            }







            
        }
    }
}
