using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Entities
{
    public enum ProjectileType
    {
        Bullet,
        Laser,
        Fire,
        Heavy,
        Shotgun,
        Sludge,
        Rocket
    }

    public class Projectile : Entity
    {

        private ProjectileType mProjectileType;
        private PositionComponent mPositionComponent;
        private CollisionComponent mCollisionComponent;
        private ProjectileData mData;

        public Projectile(ProjectileType projectileType)
        {
            mProjectileType = projectileType;

        }

        public ProjectileType ProjectileType
        {
            get { return mProjectileType;}
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
            set { mPositionComponent = value; }
        }

        public CollisionComponent CollisionComponent
        {
            get { return mCollisionComponent; }
            set { mCollisionComponent = value; }
        }

        public ProjectileData Data
        {
            get { return mData; }
            set { mData = value; }
        }
    }

    public class Bullet : Projectile
    {
        private BulletRenderComponent mRenderComponent;
        public Bullet(string texturePath, int x, int y, Rectangle sourceRectangle, float rotation, ProjectileData projectileData) : base(ProjectileType.Bullet)
        {
            PositionComponent = new BulletPositionComponent(x,y, rotation, this);
            CollisionComponent = new BoundsCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            RenderComponent = new BulletRenderComponent(texturePath, sourceRectangle, this);
            Data = projectileData;
        }

        public BulletRenderComponent RenderComponent
        {
            get { return mRenderComponent;}
            set { mRenderComponent = value; }
        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            CollisionComponent.UnRegister();
            RenderComponent.UnRegister();
            base.UnRegister();
        }
    }
}
