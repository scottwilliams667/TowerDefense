using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class TileRenderComponent : Component
    {
        private static Texture2D mOccupiedTexture = AssetOps.LoadSharedAsset<Texture2D>("occupied");
        private Tile mTile;
        private bool mIsVisible;
        private LayerType mRegisteredLayer;

        public TileRenderComponent(Entity owner, bool register = true)
            : base(owner, register)
        {
        }

        public override void UnRegister()
        {
            LayerManager.Layer.RenderSystem.UnRegister(this);
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.Register(this);
        }

        public Tile Tile
        {
            get { return mTile; }
            set { mTile = value; }
        }

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; }
        }

        public override void Update()
        {
            if (!mIsVisible)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            spriteBatch.Draw(mOccupiedTexture, mTile.PositionComponent.Position, null, Color.White * 1.4f, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);
            
        }
    }

    /*
     * This class is responsible for showing the green overlay in build mode. This
     * overlay covers the entire screen, and the red tiles indicate which tiles are
     * unavailable to build on.
     * 
     * This class also breaks the ECS rules since it has no Entity/PositionComponent
     * associated with it
     */
    public class EmptyOverlay : RenderComponent
    {
        private Vector2 mPosition;
        private LayerType mRegisteredLayer;
        public EmptyOverlay(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
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

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            spriteBatch.Draw(mTexture, Position, null, Color.White * 1.4f, 0, Vector2.Zero, new Vector2( mTexture.Width / viewport.Width, mTexture.Height / viewport.Height), SpriteEffects.None, 0.10f );

        }
    }

}
