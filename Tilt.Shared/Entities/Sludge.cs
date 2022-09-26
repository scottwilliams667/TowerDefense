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
    public class Sludge : Projectile
    {
        private SludgeAnimationComponent mAnimationComponent;
        public Sludge(string texturePath, int x, int y, float rotation, ProjectileData projectileData)
            : base(ProjectileType.Sludge)
        {
            mAnimationComponent = new SludgeAnimationComponent(texturePath, new Rectangle(32,96,32,32), 1, 1, 0.3f, this);
            PositionComponent = new SludgePositionComponent(x, y, rotation, this);
            CollisionComponent = new SludgeCollisionComponent(new Rectangle(x, y, 0,0), this);
            
            Data = projectileData;
        }

        public SludgeAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public override void UnRegister()
        {
            mAnimationComponent.UnRegister();
            PositionComponent.UnRegister();
            CollisionComponent.UnRegister();
        }
    }

    public class SludgePositionComponent : PositionComponent
    {
        public SludgePositionComponent(int x, int y, float rotation, Entity owner, Vector2 origin = new Vector2())
            : base(x,y,owner, origin)
        {
        }
    }

    public class SludgeCollisionComponent : BoundsCollisionComponent
    {
        public SludgeCollisionComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public override void Update()
        {
            Sludge sludge = Owner as Sludge;
            SludgeAnimationComponent animationComponent = sludge.AnimationComponent as SludgeAnimationComponent;
            PositionComponent positionComponent = sludge.PositionComponent;

            Vector2 position = positionComponent.Position;
            Vector2 frameSize = animationComponent.Scale;

            Vector2 paddedPosition = position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight);

            Rectangle bounds = new Rectangle(0, 0,
                (int)(animationComponent.SourceRectangle.Width * frameSize.X),
                (int)(animationComponent.SourceRectangle.Height * frameSize.Y));

            Vector2 halfSize = new Vector2(bounds.Width / 2, bounds.Height / 2);


            Bounds = new Rectangle((int)(paddedPosition.X - halfSize.X), (int)(paddedPosition.Y - halfSize.Y), 
                (int)(bounds.Width), (int)(bounds.Width));


        }
    }

    public class SludgeAnimationComponent : AnimationComponent
    {
        private const float kMaxSludgeScale = 3.0f;
        private const float kSludgeIncrementPerSecond = 1.17f;

        private Vector2 mScale = Vector2.One;
        
        public SludgeAnimationComponent(string texturePath, Rectangle sourceRectangle, int rows, int columns, float interval, Entity owner, bool register = true) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public Vector2 Scale
        {
            get { return mScale; }
        }

        public override void Update()
        {
            Sludge sludge = Owner as Sludge;
            SludgePositionComponent positionComponent = sludge.PositionComponent as SludgePositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            spriteBatch.Draw(mTexture, positionComponent.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight), CurrentRectangle, Color.White, 0.0f, new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2), mScale.X, SpriteEffects.None, 0.25f);

            if (SystemsManager.Instance.IsPaused)
                return;
            

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                mScale = new Vector2( (float)(mScale.X * kSludgeIncrementPerSecond), (float)(mScale.Y * kSludgeIncrementPerSecond));
                
                CurrentTime = Interval;
            }
            
            //sludge is at max, remove the AOE effect
            if(mScale.X >= kMaxSludgeScale)
                sludge.UnRegister();
            
        }

    }
}
