using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Components
{
    public class UIRenderComponent : Component
    {
        protected Texture2D mTexture;
        private LayerType mRegisteredLayer;
        
        public UIRenderComponent(string texturePath, Entity owner, bool register = true) : base(owner, register)
        {
            mTexture = AssetOps.LoadAsset<Texture2D>(texturePath);
        }

        public Texture2D Texture
        {
            get { return mTexture; }
            set { mTexture = value; }
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
        }

        public override void Update()
        {
        }
    }
}
