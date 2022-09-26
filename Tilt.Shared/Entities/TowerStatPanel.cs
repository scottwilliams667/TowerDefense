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
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class StatPanel : UIElement
    {
        private StatPanelRenderComponent mRenderComponent;
        public StatPanel(int x, int y) : base(x,y)
        {
            PositionComponent = new PositionComponent(x, y, this);
            mRenderComponent = new StatPanelRenderComponent("selectionbox", this);
        }
    }

    public class StatPanelRenderComponent : UIRenderComponent
    {
        private SpriteFont mFont;
        private float viewportWidth;
        public StatPanelRenderComponent(string texturePath, Entity owner) : base(texturePath, owner)
        {
            mFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
            viewportWidth = ((StatPanel)owner).PositionComponent.X;
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            StatPanel panel = Owner as StatPanel;
            PositionComponent positionComponent = panel.PositionComponent;
            float x = positionComponent.X;
            float y = positionComponent.Y;


            TileNode tile = TileMap.SelectedTile;

            if(tile != null && tile.HasObject)
            {
                IPlaceable placeable = tile.Object;

                if(placeable is Tower)
                {
                    Tower tower = placeable as Tower;
                    TowerData towerData = tower.Data as TowerData;
                    AmmoCapacityComponent ammoCapacityComponent = tower.AmmoCapacityComponent;
                    HealthComponent healthComponent = tower.HealthComponent;
                    CooldownComponent cooldownComponent = tower.CooldownComponent;

                    spriteBatch.DrawString(mFont, string.Format("Damage: {0}", towerData.Damage), new Vector2(x, y), 
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

                    spriteBatch.DrawString(mFont, string.Format("FOV: {0}", towerData.FieldOfView), new Vector2(x, y+30),
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

                    spriteBatch.DrawString(mFont, string.Format("Cooldown: {0}", cooldownComponent.TimeSet), new Vector2(x, y+60),
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

                    spriteBatch.DrawString(mFont, string.Format("Ammo: {0}", ammoCapacityComponent.AmmoCapacity), new Vector2(x + 150, y ),
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

                    spriteBatch.DrawString(mFont, string.Format("Health: {0}", healthComponent.Health), new Vector2(x + 150, y + 30),
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

                    spriteBatch.DrawString(mFont, string.Format("Fire Rate: {0}", towerData.FireRate), new Vector2(x + 150, y + 60),
                        Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);
                }

                if(placeable is AddOn)
                {
                    AddOn addon = placeable as AddOn;
                    
                }
            }
            else
            {
                //render pre-baked info?
            }

        }
    }
}
