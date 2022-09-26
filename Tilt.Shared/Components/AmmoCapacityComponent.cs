using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class AmmoCapacityComponent : TimerComponent
    {
        private int mAmmoCapacity;
        private int mAmmo;
        public AmmoCapacityComponent(int ammoCapacity, Entity owner, bool register = true) : base(owner, register)
        {
            mAmmoCapacity = ammoCapacity;
            mAmmo = ammoCapacity;
        }

        public int AmmoCapacity
        {
            get { return mAmmoCapacity; }
            set { mAmmoCapacity = value; }
        }

        public int Ammo
        {
            get { return mAmmo; }
            set { mAmmo = value; }
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;
            CooldownComponent cooldownComponent = tower.CooldownComponent;

            if (cooldownComponent == null)
                return;

            //ammo is empty and tower needs to be cooled down
            if (Ammo <= 0 && !cooldownComponent.IsCooling)
            {
                cooldownComponent.Cooldown();
                Ammo = AmmoCapacity;
            }
            
        }
    }

    public class AmmoCapacityRenderComponent : RenderComponent
    {
        private Texture2D mShadow;
        public AmmoCapacityRenderComponent(string texturePath, Entity owner) : base(texturePath, owner)
        {
            mShadow = AssetOps.LoadSharedAsset<Texture2D>("fillbarshadow");
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;

            if(tower == null)
                return;

            //SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            //CooldownComponent cooldownComponent = tower.CooldownComponent;
            //AmmoCapacityComponent ammoCapacityComponent = tower.AmmoCapacityComponent;
            //PositionComponent positionComponent = tower.PositionComponent;

            ////only show when we are not cooling
            //if (cooldownComponent == null || ammoCapacityComponent == null || cooldownComponent.IsCooling)
            //    return;

            //float percentage = (float)((float)ammoCapacityComponent.Ammo / (float)ammoCapacityComponent.AmmoCapacity);

            //Rectangle sourceRectangle = new Rectangle(0, 0, (int)(percentage * mTexture.Width), mTexture.Height);

            //spriteBatch.Draw(mShadow, new Vector2(positionComponent.Position.X + TileMap.TileWidth / 2, positionComponent.Position.Y), 
            //    null, Color.White, 0.0f, new Vector2(mTexture.Width / 2, 0), 1.0f, SpriteEffects.None, 0.34f);
            //spriteBatch.Draw(mTexture, new Vector2(positionComponent.Position.X + TileMap.TileWidth / 2, positionComponent.Position.Y), 
            //    sourceRectangle, Color.White, 0.0f, new Vector2(mTexture.Width / 2, 0), 1.0f, SpriteEffects.None, 0.36f);

        }

    }
}
