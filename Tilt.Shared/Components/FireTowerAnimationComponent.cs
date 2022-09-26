using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class FireTowerAnimationComponent : AnimationComponent
    {
        public FireTowerAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows,
            int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;
            PositionComponent positionComponent = tower.PositionComponent;
            TowerAimerPositionComponent cannonPosition = tower.CannonPositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), null, Color.White,
                0.0f, new Vector2(0, 0), 1.0f * 0.5f, SpriteEffects.None, 0.3f);


        }
    }

}
