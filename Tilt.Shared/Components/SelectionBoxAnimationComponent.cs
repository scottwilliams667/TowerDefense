using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Components
{
    public class SelectionBoxAnimationComponent : AnimationComponent
    {
        public SelectionBoxAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) 
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            if (TileMap.SelectedTile == null)
                return;
            
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            Vector2 offset = Vector2.Zero;

            //have to adjust where we draw selection box since
            //every object is different size and are not always centered in
            //sprite sheet
            if(TileMap.SelectedTile.Object is Barricade)
                offset = new Vector2(2, 2);
            if(TileMap.SelectedTile.Object is BulletTower)
                offset = new Vector2(3, 2);
            if (TileMap.SelectedTile.Object is HeavyTower)
                offset = new Vector2(0,1);
            if (TileMap.SelectedTile.Object is LaserTower)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is NuclearTower)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is RocketTower)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is ShotgunTower)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is AmmoAddOn)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is RefineryAddOn)
                offset = new Vector2(1,1);
            if(TileMap.SelectedTile.Object is Refinery)
                offset = new Vector2(2,2);
            if (TileMap.SelectedTile.Object is RangeBoosterAddOn)
                offset = new Vector2(2,2);
            if (TileMap.SelectedTile.Object is CooldownAddOn)
                offset = new Vector2(2, 2);
            if (TileMap.SelectedTile.Object is ShieldGenerator)
                offset = new Vector2(1,1);
            

            

            spriteBatch.Draw(mTexture, TileMap.SelectedTile.Tile.PositionComponent.Position, CurrentRectangle, Color.White, 0, offset, 1.0f, SpriteEffects.None, 0.1f );

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }


            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
            }

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

        }
    }
}
