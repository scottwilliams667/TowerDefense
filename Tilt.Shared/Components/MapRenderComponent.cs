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
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Components
{
    public class MapRenderComponent : RenderComponent
    {
        private Texture2D mBackground0;
        private Texture2D mBackground1;
        private Texture2D mBg11;
        private Texture2D mBg12;
        private Texture2D mBg21;
        private Texture2D mBg22;


        public MapRenderComponent(string texturePath, Entity owner) : base(texturePath, owner)
        {
            mBg11 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-1");
            mBg12 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-2");
            mBg21 = AssetOps.LoadSharedAsset<Texture2D>("mapbg2-1");
            mBg22 = AssetOps.LoadSharedAsset<Texture2D>("mapbg2-2");
            mBackground0 = AssetOps.LoadSharedAsset<Texture2D>(LevelManager.Level.MapNameLeft);
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            spriteBatch.End();

            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);

            Vector2 topLeft = Vector2.Zero;
            Camera camera = gameLayer.EntitySystem.GetEntitiesByType<Camera>().FirstOrDefault();
            if (camera == null)
                return;

            spriteBatch.Draw(mBg11, topLeft, new Rectangle((int)(camera.PositionComponent.Position.X * 0.5f), (int)(camera.PositionComponent.Position.Y * 0.5f), viewport.Width, viewport.Height), Color.White);
            spriteBatch.Draw(mBg21, topLeft, new Rectangle((int)(camera.PositionComponent.Position.X * 0.8f), (int)(camera.PositionComponent.Position.Y * 0.8f), viewport.Width, viewport.Height), Color.White);
            
            
            topLeft = new Vector2(mBg11.Width * 1.0f, 0);


            spriteBatch.Draw(mBg12, topLeft, new Rectangle((int)(camera.PositionComponent.Position.X * 0.5f), (int)(camera.PositionComponent.Position.Y * 0.5f), viewport.Width, viewport.Height), Color.White);
            spriteBatch.Draw(mBg22, topLeft, new Rectangle((int)(camera.PositionComponent.Position.X * 0.8f), (int)(camera.PositionComponent.Position.Y * 0.8f), viewport.Width, viewport.Height), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, gameLayer.Matrix);

            topLeft = new Vector2(Tuner.MapStartPosX, Tuner.MapStartPosY);
            spriteBatch.Draw(mBackground0, topLeft, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.1f);

            

            
        }
    }
}
