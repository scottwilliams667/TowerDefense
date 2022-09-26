using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json.Linq;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Entities
{
    public class StartMenuScreen : Entity
    {
        private StartMenuScreenRenderComponent mRenderComponent;
        private StartMenuCameraComponent mCameraComponent;
        

        public StartMenuScreen(
            string startMenuMapTexture,
             string roller0Texture,
            string roller1Texture,
            string enemyTexture,
            Rectangle rollerSourceRectangle,
            Rectangle enemiesRectangle,
            int rollerRows,
            int rollerColumns,
            float rollerInterval)
        {
            mRenderComponent = new StartMenuScreenRenderComponent(
                startMenuMapTexture,
                roller0Texture,
                roller1Texture,
                enemyTexture,
                rollerSourceRectangle,
                enemiesRectangle,
                rollerRows,
                rollerColumns,
                rollerInterval,
                this);

            mCameraComponent = new StartMenuCameraComponent(0,0, this);
        }

        public StartMenuCameraComponent CameraComponent
        {
            get { return mCameraComponent; }
        }

        public StartMenuScreenRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class StartMenuCameraComponent : PositionComponent
    {
        private Camera2D mCamera;

        private const int kSpeed = 17;

        private bool mMoveRight;

        public StartMenuCameraComponent(int x, int y, Entity owner, Vector2 origin = new Vector2()) : base(x, y, owner, origin)
        {
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            GameWindow gameWindow = ServiceLocator.GetService<GameWindow>();

            BoxingViewportAdapter adapter = new BoxingViewportAdapter(gameWindow, graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            mCamera = new Camera2D(adapter);
            mCamera.Position = Vector2.Zero;

            mCamera.Origin = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            mCamera.Zoom = (AssetOps.Version == "P") ? 2.5f : 2.5f;

        }

        public Camera2D Camera
        {
            get { return mCamera;}
        }

        public override void Update()
        {
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            Viewport viewport = graphicsDevice.Viewport;

            StartMenuScreen screen = Owner as StartMenuScreen;
            StartMenuScreenRenderComponent renderComponent = screen.RenderComponent;
            Texture2D mapTexture = renderComponent.MapTexture;


            if(mCamera.BoundingRectangle.X < 0.0f)
            {
                mCamera.Move(new Vector2(-mCamera.BoundingRectangle.X, 0));
                mMoveRight = true;
            }
            


            if(mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > Math.Min(mapTexture.Width, viewport.Width * 3/4)  )
            {
                mCamera.Move(new Vector2(Math.Min(mapTexture.Width, viewport.Width * 3 / 4) - mCamera.BoundingRectangle.X - mCamera.BoundingRectangle.Width, 0));
                mMoveRight = false;
            }

            if (mMoveRight)
            {
                mCamera.Move(new Vector2(1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }
            if (!mMoveRight)
            {
                mCamera.Move(new Vector2(-1 * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0));
            }

            //if(mCamera.BoundingRectangle.X > 0.0f)
            //{
            //    mCamera.Move(new Vector2(-mCamera.BoundingRectangle.X, 0));
            //}
            if (mCamera.BoundingRectangle.Y > 0.0f)
            {
                mCamera.Move(new Vector2(0, -mCamera.BoundingRectangle.Y));
            }

            //if (mCamera.BoundingRectangle.X + mCamera.BoundingRectangle.Width > (viewport.Width ) + Math.Max(TileMap.Width / 2, (viewport.Width) / 4))
            //{
            //    mCamera.Move(new Vector2(viewport.Width * 2 / 3 + Math.Max(TileMap.Width / 2, (viewport.Width*2/3) / 4) - mCamera.BoundingRectangle.Width - mCamera.BoundingRectangle.X, 0));
            //}

            //if (mCamera.BoundingRectangle.Y + mCamera.BoundingRectangle.Height > viewport.Height + Math.Max(TileMap.Height / 2, (viewport.Width) / 4))
            //{
            //    mCamera.Move(new Vector2(0, viewport.Height + Math.Max(TileMap.Height / 2, viewport.Height / 4) - mCamera.BoundingRectangle.Height - mCamera.BoundingRectangle.Y));
            //}
        }
    }

    public class StartMenuScreenRenderComponent : Component
    {
        private LayerType mRegisteredLayer;

        private Texture2D mStartMenuMap;

        private Texture2D mRoller0Texture;
        private Texture2D mRoller1Texture;
        private Texture2D mEnemiesTexture;
        private Rectangle mRollerRectangle;
        private Rectangle mEnemiesRectangle;
        private int mRollerRows;
        private int mRollerColumns;
        private float mRollerInterval;



        public StartMenuScreenRenderComponent(
            string startMenuMapTexture,
            string roller0Texture,
            string roller1Texture,
            string enemyTexture,
            Rectangle rollerSourceRectangle,
            Rectangle enemiesRectangle,
            int rollerRows,
            int rollerColumns,
            float rollerInterval,
            Entity owner, 
            bool register = true) : base(owner, register)
        {
            mStartMenuMap = AssetOps.LoadSharedAsset<Texture2D>(startMenuMapTexture);
            
            mRoller0Texture = AssetOps.LoadSharedAsset<Texture2D>(roller0Texture);
        }

        public Texture2D MapTexture
        { get { return mStartMenuMap;} }

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
            StartMenuScreen startMenuScreen = Owner as StartMenuScreen;

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            Viewport viewport = graphicsDevice.Viewport;

            StartMenuCameraComponent cameraComponent = startMenuScreen.CameraComponent;

            Layer layer = LayerManager.GetLayerOfEntity(Owner);
            if (layer == null)
                return;
            


            //spriteBatch.Draw(mStartMenuMap, new Vector2(0,0), Color.White);


            int viewportWidth = viewport.Width * 2 / 3;
            int viewportHeight = viewport.Height;

            decimal xScale = 1;
            decimal yScale = 1;

            if(viewportWidth > mStartMenuMap.Width)
            {
                xScale = decimal.Divide(viewportWidth, mStartMenuMap.Width);
            }

            if(viewportHeight > mStartMenuMap.Height)
            {
                yScale = decimal.Divide(viewportHeight, mStartMenuMap.Height);   
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, cameraComponent.Camera.GetViewMatrix());

            //spriteBatch.Draw(mStartMenuMap, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);


            spriteBatch.Draw(mStartMenuMap, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero,
                1.0f, SpriteEffects.None, 0.0f); 


            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

            //int kFrameWidth = 48;

            //int viewportWidth = viewport.Width * 2 / 3;
            //int viewportHeight = viewport.Height;

            //for (int k = 0; k < viewportHeight; k += kFrameWidth)
            //{


            //    for (int i = k; i < viewportWidth; i += kFrameWidth)
            //    {
            //        for (int j = 0; j < viewportHeight; j += kFrameWidth)
            //        {
            //            spriteBatch.Draw(mRoller0Texture, new Vector2(i, j), new Rectangle(0, 0, 48, 48), Color.White);

            //            i += 48;
            //        }
            //    }

            //}

        }
    }
}
