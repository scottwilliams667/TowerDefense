using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Entities
{
    public class Credits : Entity
    {
        private CreditsRenderComponent mRenderComponent;

        public Credits()
        {
            RenderComponent = new CreditsRenderComponent("selectionbox", this);
        }

        public CreditsRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
            set { mRenderComponent = value; }
        }

        public override void UnRegister()
        {
            RenderComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class CreditsRenderComponent : RenderComponent
    {
        private Texture2D mBg11;
        private Texture2D mBg12;

        public CreditsRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mBg11 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-1");
            mBg12 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-2");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            spriteBatch.End();

            
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);

            Vector2 topLeft = Vector2.Zero;

            spriteBatch.Draw(mBg11, topLeft, new Rectangle(0,0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.Draw(mBg12, topLeft, new Rectangle(0,0, viewport.Width, viewport.Height), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
        }


    }
}
