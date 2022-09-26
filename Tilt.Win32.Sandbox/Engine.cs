#define WIN32

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;


namespace Tilt.Win32.Sandbox
{
    public class Engine : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FrameCounter mFrameCounter;
        private SpriteFont mFont;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }


        protected override void LoadContent()
        {
            if (AssetOps.Version == "5")
            {
                graphics.PreferredBackBufferWidth = 1134;
                graphics.PreferredBackBufferHeight = 640;
                graphics.ApplyChanges();
            }
            if (AssetOps.Version == "R")
            {
                graphics.PreferredBackBufferWidth = 1336;
                graphics.PreferredBackBufferHeight = 750;
                graphics.ApplyChanges();
            }
            if (AssetOps.Version == "P")
            {
                graphics.PreferredBackBufferWidth = 2208;
                graphics.PreferredBackBufferHeight = 1242;
                graphics.ApplyChanges();
            }
            if(AssetOps.Version == "X")
            {
                graphics.PreferredBackBufferWidth = 2436;
                graphics.PreferredBackBufferHeight = 1125;
                graphics.ApplyChanges();
            }
            if(AssetOps.Version == "XR")
            {
                graphics.PreferredBackBufferWidth = 1792;
                graphics.PreferredBackBufferHeight = 828;
                graphics.ApplyChanges();
            }
            if(AssetOps.Version == "XMax")
            {
                graphics.PreferredBackBufferWidth = 2688;
                graphics.PreferredBackBufferHeight = 1242;
                graphics.ApplyChanges();
            }


            spriteBatch = new SpriteBatch(GraphicsDevice);



            ServiceLocator.AddService<GraphicsDevice>(GraphicsDevice);
            ServiceLocator.AddService<GraphicsDeviceManager>(graphics);
            ServiceLocator.AddService<SpriteBatch>(spriteBatch);
            ServiceLocator.AddService<ContentManager>(Content);
            ServiceLocator.AddService<GameWindow>(Window);

        }


        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || SystemsManager.Instance.Quit)
                Quit_();

            ServiceLocator.RemoveService<GameTime>();
            ServiceLocator.AddService<GameTime>(gameTime);
            
            SystemsManager.Instance.Update();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(35, 35, 35));

            SystemsManager.Instance.Draw();

            base.Draw(gameTime);
        }

        private void Quit_()
        {
            Exit();
        }

    }
}
