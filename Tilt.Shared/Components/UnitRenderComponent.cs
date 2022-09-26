using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{


    public class UnitRenderComponent : RenderComponent
    {
        public UnitRenderComponent(string texturePath, Entity owner)
            : base(texturePath, owner)
        {
        }


        public override void Update()
        {
            Unit unit = Owner as Unit;
            PositionComponent position = unit.PositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            spriteBatch.Draw(mTexture, position.Position, null, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.6f);



        }
    }
}
