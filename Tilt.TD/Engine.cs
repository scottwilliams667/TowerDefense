using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.Content.Res;
using Java.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.TD
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private FrameCounter frameCounter;
        private SpriteFont mFont;

        private bool mIsInitialized;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
        }

        protected override void Initialize()
        {
            TouchPanel.EnabledGestures = GestureType.None | GestureType.Hold | GestureType.Flick | GestureType.FreeDrag | 
                GestureType.Tap | GestureType.DoubleTap | GestureType.Pinch | GestureType.PinchComplete | 
                GestureType.DragComplete | GestureType.HorizontalDrag | GestureType.VerticalDrag | GestureType.DoubleTap;
            TouchPanelCapabilities caps = TouchPanel.GetCapabilities();

            base.Initialize();
        }

       
        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth =  GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            


            ServiceLocator.AddService<GraphicsDevice>(GraphicsDevice);
            ServiceLocator.AddService<GraphicsDeviceManager>(graphics);
            ServiceLocator.AddService<SpriteBatch>(spriteBatch);
            ServiceLocator.AddService<ContentManager>(Content);
            ServiceLocator.AddService<GameWindow>(Window);


            frameCounter = new FrameCounter();

            mFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");

            AssetOps.Serializer.DeserializeStringsFile("thisisatest");
            
            SystemsManager.Instance.Initialize();
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
            GraphicsDevice.Clear(new Color(51,51,51));

            SystemsManager.Instance.Draw();
            
            base.Draw(gameTime);
        }

        private void Quit_()
        {
            Exit();
        }

    }
}
