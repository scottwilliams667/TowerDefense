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
    public class Fire : Projectile
    {
        private ulong mOwnerId;
        private FireAnimationComponent mAnimationComponent;
        private FireDebugPointCollisionComponent mFireDebugRenderCollision;
        public Fire(string texturePath, int x, int y, ulong ownerId, ProjectileData projectileData)
            : base(ProjectileType.Fire)
        {
            mOwnerId = ownerId;
            PositionComponent = new FirePositionComponent(x, y, this);
            //dunno what the actual measurements are
            CollisionComponent = new FireCollisionComponent(
                new Vector2(x, y),
                new Vector2(x + 32, y),
                new Vector2(x, y + 32),
                new Vector2(x + 32, y + 32),
                new Vector2(x, y + 16),
                this);
            AnimationComponent = new FireAnimationComponent("FireSpriteSheet_5X_Speed1", new Rectangle(0, 0, 32, 32), 0.0f, 1, 16, this);
            //FireDebugRenderCollision = new FireDebugPointCollisionComponent("FireSpriteSheet_5X_Speed1", this);

            Data = projectileData;
        }



        /// <summary>
        /// The fireTower that owns the fire
        /// </summary>
        public ulong OwnerId { get { return mOwnerId; } }

        public FireAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public FireDebugPointCollisionComponent FireDebugRenderCollision
        {
            get { return mFireDebugRenderCollision; }
            set { mFireDebugRenderCollision = value; }
        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            AnimationComponent.UnRegister();
            CollisionComponent.UnRegister();
            //FireDebugRenderCollision.UnRegister();
        }
    }

    public class FirePositionComponent : PositionComponent
    {
        private float mRotation;
        private Vector2 mOriginalPosition;
        public FirePositionComponent(int x, int y, Entity owner, Vector2 origin = new Vector2())
            : base(x, y, owner, origin)
        {
            mOriginalPosition = Position;
        }

        public float Rotation
        {
            get { return mRotation; }
        }

        public override void Update()
        {
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Fire fire = Owner as Fire;

            ulong ownerId = fire.OwnerId;

            FireTower fireTower = LayerManager.Layer.EntitySystem.GetEntityById(ownerId) as FireTower;

            if (fireTower == null)
                return;

            TowerAimerPositionComponent towerPositionComponent = fireTower.CannonPositionComponent;

            mRotation = towerPositionComponent.Rotation;

        }
    }

    public class FireAnimationComponent : AnimationComponent
    {
        public FireAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            Fire fire = Owner as Fire;
            FirePositionComponent positionComponent = fire.PositionComponent as FirePositionComponent;
            Vector2 position = positionComponent.Position;
            float rotation = positionComponent.Rotation;

            spriteBatch.Draw(mTexture, position, CurrentRectangle, Color.White, rotation, new Vector2(0, 16), 1.0f, SpriteEffects.None, 0.4f);

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentColumnIndex++;
            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            if (CurrentColumnIndex >= Columns)
            {
                fire.UnRegister();
            }

        }
    }

    public class FireDebugPointCollisionComponent : RenderComponent
    {
        public FireDebugPointCollisionComponent(string texturePath, Entity owner, bool register = true)
            : base(texturePath, owner, register)
        {
        }

        public override void Update()
        {
            Fire fire = Owner as Fire;
            PointCollisionComponent collisionComponent = fire.CollisionComponent as PointCollisionComponent;
            FirePositionComponent position = fire.PositionComponent as FirePositionComponent;

            Vector2 point1 = collisionComponent.Point1;
            Vector2 point2 = collisionComponent.Point2;
            Vector2 point3 = collisionComponent.Point3;
            Vector2 point4 = collisionComponent.Point4;

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            Texture2D tex1 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex2 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex3 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Texture2D tex4 = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);

            Int32[] pixel = { 0xFFFFFF };
            tex1.SetData<Int32>(pixel, 0, 32);
            tex2.SetData<Int32>(pixel, 0, 32);
            tex3.SetData<Int32>(pixel, 0, 32);
            tex4.SetData<Int32>(pixel, 0, 32);

            spriteBatch.Draw(tex1, new Rectangle((int)point1.X, (int)point1.Y, 32, 1),
                null, Color.Red, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(tex2, new Rectangle((int)point3.X, (int)point3.Y, 32, 1),
                null, Color.Red, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(tex3, new Rectangle((int)point1.X, (int)point1.Y, 1, 32),
                null, Color.Red, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);


            spriteBatch.Draw(tex4, new Rectangle((int)point2.X, (int)point2.Y, 1, 32),
                null, Color.Red, position.Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);
            base.Update();
        }
    }

    public class FireCollisionComponent : PointCollisionComponent
    {
        public FireCollisionComponent(
            Vector2 point1,
            Vector2 point2,
            Vector2 point3,
            Vector2 point4,
            Vector2 origin,
            Entity owner)
            : base(point1, point2, point3, point4, origin, owner)
        {
        }

        public override void Update()
        {
            if (SystemsManager.Instance.IsPaused)
                return;

            Fire fire = Owner as Fire;
            FirePositionComponent positionComponent = fire.PositionComponent as FirePositionComponent;

            if (positionComponent == null)
                return;


            Vector2 position = positionComponent.Position;
            float rotation = positionComponent.Rotation;

            var pos1 = new Vector2(position.X, position.Y);
            var pos2 = new Vector2(position.X + TileMap.TileWidth, position.Y);
            var pos3 = new Vector2(position.X, position.Y + TileMap.TileHeight);
            var pos4 = new Vector2(position.X + TileMap.TileWidth, position.Y + TileMap.TileHeight);

            pos1 = GeometryOps.RotateAroundPoint(pos1, Origin, rotation);
            pos2 = GeometryOps.RotateAroundPoint(pos2, Origin, rotation);
            pos3 = GeometryOps.RotateAroundPoint(pos3, Origin, rotation);
            pos4 = GeometryOps.RotateAroundPoint(pos4, Origin, rotation);

            ///HACK HACK
            /// Cant seem to line up the collision box properly
            /// when rotating each point. Have to move the box
            /// up by 16 in order for it to align.
            /// This may be caused by an improper setting of the origin in the
            /// PointCollisionComponent. will fix later. 
            /// Even with this hack, it still seems to find collisions though.
            Point1 = new Vector2(pos1.X, pos1.Y - 16);
            Point2 = new Vector2(pos2.X, pos2.Y - 16);
            Point3 = new Vector2(pos3.X, pos3.Y - 16);
            Point4 = new Vector2(pos4.X, pos4.Y - 16);
        }
    }

}
