using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class StartMenuPanel : UIElement
    {
        private PanelState mPanelState;
        private StartMenuPanelRenderComponent mRenderComponent;

        public StartMenuPanel(int x, int y, string texturePath, PanelState panelState)
            : base(x,y)
        {
            PositionComponent = new PositionComponent(x, y, this);
            mRenderComponent = new StartMenuPanelRenderComponent(texturePath, this);

        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            mRenderComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class StartMenuPanelRenderComponent : UIRenderComponent
    {
        public StartMenuPanelRenderComponent(string texturePath, Entity owner)
            : base(texturePath, owner)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            StartMenuPanel panel = Owner as StartMenuPanel;
            PositionComponent positionComponent = panel.PositionComponent;

            if (panel == null)
                return;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.10f);
            
        }
    }
}
