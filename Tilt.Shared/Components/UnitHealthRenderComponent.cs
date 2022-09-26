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
    public class HealthRenderComponent : RenderComponent
    {
        protected Texture2D mFillTexture;
        public HealthRenderComponent(string borderTexturePath, string fillTexturePath, Entity owner, bool register = true)
            : base(borderTexturePath, owner, register)
        {
            mFillTexture = AssetOps.LoadSharedAsset<Texture2D>(fillTexturePath);
        }
    }

    public class UnitHealthRenderComponent : HealthRenderComponent
    {
        public UnitHealthRenderComponent(string borderTexturePath, string fillTexturePath, Entity owner, bool register = true) : base(borderTexturePath, fillTexturePath, owner, register)
        {
        }
        
        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            Unit unit = Owner as Unit;
            float healthPercentage = unit.HealthComponent.HealthPercentage;
            PositionComponent positionComponent = unit.PositionComponent;
            Rectangle healthBarRectangle = new Rectangle(0,0, (int)Math.Ceiling((mFillTexture.Width * healthPercentage)), mFillTexture.Height);

            spriteBatch.Draw(mFillTexture, new Vector2(positionComponent.X, positionComponent.Y - 20),
                healthBarRectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.34f);

            spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y - 20), null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.35f);

        }

    }
}
