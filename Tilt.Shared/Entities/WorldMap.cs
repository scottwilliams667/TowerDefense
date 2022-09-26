using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.EntityComponent.Structures;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class WorldMap : Entity
    {
        private WorldMapRenderComponent mRenderComponent;
        private PositionComponent mPositionComponent;
        private List<Button> mLevelButtons = new List<Button>();

        public WorldMap(string texturePath, int x, int y)
        {
            mRenderComponent = new WorldMapRenderComponent(texturePath, this);
            mPositionComponent = new PositionComponent(x,y, this);
            EventSystem.SubScribe(EventType.LevelComplete, OnLevelComplete_);
        }

        public override void UnRegister()
        {
            mRenderComponent.UnRegister();
            mPositionComponent.UnRegister();
            EventSystem.UnSubScribe(EventType.LevelComplete, OnLevelComplete_);
            base.UnRegister();
        }

        public List<Button> LevelButtons
        {
            get { return mLevelButtons; }
        }

        public void Initialize()
        {
            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();
            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;

            int levelCount = LevelManager.Levels.Count;
            int levelCompleted = LevelManager.SaveFile.LevelCompleted;

            Texture2D buttonTexture = AssetOps.LoadAsset<Texture2D>("soundfxbutton");
            int xOffset = viewportWidth / levelCount;
            int x = 0;
            int y = 0;
        }

        private void OnLevelComplete_(object sender, IGameEventArgs e)
        {
            int levelCompleted = LevelManager.SaveFile.LevelCompleted;
            int levelCount = LevelManager.Levels.Count;
        }
    }

    public class WorldMapRenderComponent : UIRenderComponent
    {
        private Texture2D mBg11;
        private Texture2D mBg12;

        private Texture2D mLevelText1;
        private Texture2D mMapBar1;

        private Texture2D mLevelText2;
        private Texture2D mMapBar2;

        private Texture2D mLevelText3;
        private Texture2D mMapBar3;

        private Texture2D mLevelText4;
        private Texture2D mMapBar4;

        private Texture2D mLevelText5;
        private Texture2D mMapBar5;

        private Texture2D mLevelText6;
        private Texture2D mMapBar6;

        private Texture2D mLevelText7;
        private Texture2D mMapBar7;

        private Texture2D mLevelText8;
        private Texture2D mMapBar8;

        private Texture2D mLevelText9;
        private Texture2D mMapBar9;

        private Texture2D mLevelText10;
        private Texture2D mMapBar10;

        private Texture2D mEarth;

        private SpriteFont mFont;

        private Texture2D mBackground;
        
        private static readonly Vector3 k_color = new Vector3(1,0,0);

        private Texture2D mLetterB;


        public WorldMapRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mBg11 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-1");
            mBg12 = AssetOps.LoadSharedAsset<Texture2D>("mapbg1-2");
            mEarth = AssetOps.LoadAsset<Texture2D>("earth");
            mFont = AssetOps.LoadAsset<SpriteFont>("WorldMapTextFont");
            mBackground = AssetOps.LoadAsset<Texture2D>("objectivespanel");

            mLevelText1 = AssetOps.LoadAsset<Texture2D>("01-leveltext");
            mMapBar1 = AssetOps.LoadAsset<Texture2D>("01-mapbar");

            mLevelText2 = AssetOps.LoadAsset<Texture2D>("02-leveltext");
            mMapBar2 = AssetOps.LoadAsset<Texture2D>("02-mapbar");

            mLevelText3 = AssetOps.LoadAsset<Texture2D>("03-leveltext");
            mMapBar3 = AssetOps.LoadAsset<Texture2D>("03-mapbar");

            mLevelText4 = AssetOps.LoadAsset<Texture2D>("04-leveltext");
            mMapBar4 = AssetOps.LoadAsset<Texture2D>("04-mapbar");

            mLevelText5 = AssetOps.LoadAsset<Texture2D>("05-leveltext");
            mMapBar5 = AssetOps.LoadAsset<Texture2D>("05-mapbar");

            mLevelText6 = AssetOps.LoadAsset<Texture2D>("06-leveltext");
            mMapBar6 = AssetOps.LoadAsset<Texture2D>("06-mapbar");

            mLevelText7 = AssetOps.LoadAsset<Texture2D>("07-leveltext");
            mMapBar7 = AssetOps.LoadAsset<Texture2D>("07-mapbar");

            mLevelText8 = AssetOps.LoadAsset<Texture2D>("08-leveltext");
            mMapBar8 = AssetOps.LoadAsset<Texture2D>("08-mapbar");

            mLevelText9 = AssetOps.LoadAsset<Texture2D>("09-leveltext");
            mMapBar9 = AssetOps.LoadAsset<Texture2D>("09-mapbar");

            mLevelText10 = AssetOps.LoadAsset<Texture2D>("10-leveltext");
            mMapBar10 = AssetOps.LoadAsset<Texture2D>("10-mapbar");

            mLetterB = AssetOps.LoadAsset<Texture2D>("letterb");

        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();


            WorldMap worldMap = Owner as WorldMap;

            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;

            int levelTextWidth = mLevelText1.Width;
            int levelTextHeight = mLevelText1.Height;

            Viewport viewport = graphicsDevice.Viewport;

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);

            Vector2 topLeft = Vector2.Zero;

            spriteBatch.Draw(mBg11, topLeft, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.Draw(mBg12, topLeft, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
            spriteBatch.Draw(mBackground, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);
            
            Layer layer = LayerManager.GetLayerOfEntity(Owner);

            int levelCompleted = LevelManager.SaveFile.LevelCompleted;
            
            uint level1minerals = 0;
            uint level2minerals = 0;
            uint level3minerals = 0;
            uint level4minerals = 0;
            uint level5minerals = 0;
            uint level6minerals = 0;
            uint level7minerals = 0;
            uint level8minerals = 0;
            uint level9minerals = 0;
            uint level10minerals = 0;

            LevelManager.SaveFile.Minerals.TryGetValue(0, ref level1minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(1, ref level2minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(2, ref level3minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(3, ref level4minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(4, ref level5minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(5, ref level6minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(6, ref level7minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(7, ref level8minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(8, ref level9minerals);
            LevelManager.SaveFile.Minerals.TryGetValue(9, ref level10minerals);

            Vector2 level5TextPosition = new Vector2(viewportWidth / 2, viewportHeight / 5);
            Vector2 level5BarPosition = new Vector2(level5TextPosition.X + (levelTextWidth / 3), level5TextPosition.Y + levelTextHeight);
            Vector2 level5ResourcesPosition = new Vector2(level5BarPosition.X, level5TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 5)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText5, level5TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar5, level5BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if(levelCompleted > 5)
            {

                spriteBatch.Draw(mLevelText5, level5TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar5, level5BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                if (level5minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level5ResourcesPosition, null, GetResourceColor(3), 0.0f,
                      //  new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level5minerals.ToString(), level5ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }

            }

            Vector2 level7TextPosition = new Vector2(viewportWidth * 8 / 10, viewportHeight * 15 / 100);
            Vector2 level7BarPosition = new Vector2(level7TextPosition.X + (levelTextWidth / 3), level7TextPosition.Y + levelTextHeight);
            Vector2 level7ResourcesPosition = new Vector2(level7BarPosition.X, level7TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 7)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText7, level7TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar7, level7BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if(levelCompleted > 7)
            {
                spriteBatch.Draw(mLevelText7, level7TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar7, level7BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                if (level7minerals != 0)
                {
                    //spriteBatch.Draw(mLetterB, level7ResourcesPosition, null, GetResourceColor(3), 0.0f,
                    //    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level7minerals.ToString(), level7ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }

            Vector2 level8TextPosition = new Vector2(level5TextPosition.X - levelTextWidth * 12 / 10, level5TextPosition.Y + levelTextHeight + levelTextHeight / 2);  //adding padding to map bar
            Vector2 level8BarPosition = new Vector2(level8TextPosition.X + (levelTextWidth * 10 / 8), level8TextPosition.Y + levelTextHeight);
            Vector2 level8ResourcesPosition = new Vector2(level8TextPosition.X + levelTextWidth / 2, level8TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 8)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText8, level8TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar8, level8BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if(levelCompleted > 8)
            {

                spriteBatch.Draw(mLevelText8, level8TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar8, level8BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.12f);

                if (level8minerals != 0)
                {
                 //   spriteBatch.Draw(mLetterB, level8ResourcesPosition, null, GetResourceColor(3), 0.0f,
                 //       new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level8minerals.ToString(), level8ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }


            spriteBatch.Draw(mEarth, new Vector2(viewportWidth - mEarth.Width, viewportHeight - mEarth.Height), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.13f);

            Vector2 level1TextPosition = new Vector2(viewportWidth - levelTextWidth, viewportHeight / 4);
            Vector2 level1BarPosition = new Vector2(viewportWidth - levelTextWidth + (levelTextWidth / 3), viewportHeight / 4 + levelTextHeight);
            Vector2 level1ResourcesPosition = new Vector2(viewportWidth - levelTextWidth + (levelTextWidth / 3), viewportHeight / 4 + levelTextHeight / 2);

            if (levelCompleted == 1)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText1, level1TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar1, level1BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 1)
            {
                spriteBatch.Draw(mLevelText1, level1TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar1, level1BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level1minerals != 0)
                {
                 //   spriteBatch.Draw(mLetterB, level1ResourcesPosition, null, GetResourceColor(3), 0.0f,
                  //      new Vector2(0, -mLetterB.Height / 4), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont,"  " + level1minerals.ToString(), level1ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }

            Vector2 level2TextPosition = new Vector2(viewportWidth * 3 / 5, viewportHeight * 4 / 10);
            Vector2 level2BarPosition = new Vector2(level2TextPosition.X + (levelTextWidth / 3), level2TextPosition.Y + levelTextHeight);
            Vector2 level2ResourcesPosition = new Vector2(level2BarPosition.X, level2TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 2)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText2, level2TextPosition, null, new Color(255,0,0,255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar2, level2BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 2)
            {
                if (level2minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level2ResourcesPosition, null, GetResourceColor(3), 0.0f,
                    //    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level2minerals.ToString(), level2ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }

                spriteBatch.Draw(mLevelText2, level2TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar2, level2BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
            }

            Vector2 level3TextPosition = new Vector2(level1TextPosition.X - levelTextWidth, viewportHeight * 4 / 10);
            Vector2 level3BarPosition = new Vector2(level3TextPosition.X + (levelTextWidth / 3), level3TextPosition.Y + levelTextHeight);
            Vector2 level3ResourcesPosition = new Vector2(level3BarPosition.X, level3TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 3)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText3, level3TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar3, level3BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 3)
            {
                spriteBatch.Draw(mLevelText3, level3TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar3, level3BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level3minerals != 0)
                {
                 //   spriteBatch.Draw(mLetterB, level3ResourcesPosition, null, GetResourceColor(3), 0.0f,
                  //      new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level3minerals.ToString(), level3ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }

            
            Vector2 level4TextPosition = new Vector2(viewportWidth *38/100, viewportHeight * 47/100); 
            Vector2 level4BarPosition = new Vector2(level4TextPosition.X + (levelTextWidth * 3/4), level4TextPosition.Y + levelTextHeight);
            Vector2 level4ResourcesPosition = new Vector2(level4TextPosition.X + levelTextWidth / 3, viewportHeight * 47 / 100 + levelTextHeight / 2);

            if (levelCompleted == 4)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText4, level4TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar4, level4BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 4)
            {
                spriteBatch.Draw(mLevelText4, level4TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar4, level4BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level4minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level4ResourcesPosition, null, GetResourceColor(3), 0.0f,
                   //     new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level4minerals.ToString(), level4ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }


            Vector2 level6TextPosition = new Vector2(level3BarPosition.X - levelTextWidth - levelTextWidth / 6, viewportHeight * 55 / 100);
            Vector2 level6BarPosition = new Vector2(level6TextPosition.X + (levelTextWidth / 3), level6TextPosition.Y + levelTextHeight);
            Vector2 level6ResourcesPosition = new Vector2(level6BarPosition.X, level6TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 6)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText6, level6TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar6, level6BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 6)
            {
                spriteBatch.Draw(mLevelText6, level6TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar6, level6BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level6minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level6ResourcesPosition, null, GetResourceColor(3), 0.0f,
                    //    new Vector2(mLetterB.Width / 2, mLetterB.Height), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level6minerals.ToString(), level6ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }


            Vector2 level9TextPosition = new Vector2(level3TextPosition.X - levelTextWidth / 12, level6BarPosition.Y + levelTextHeight / 2);
            Vector2 level9BarPosition = new Vector2(level9TextPosition.X + (levelTextWidth / 3), level9TextPosition.Y + levelTextHeight);
            Vector2 level9ResourcesPosition = new Vector2(level9BarPosition.X, level9TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 9)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText9, level9TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar9, level9BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
            }
            else if (levelCompleted > 9)
            {
                spriteBatch.Draw(mLevelText9, level9TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar9, level9BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level9minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level9ResourcesPosition, null, GetResourceColor(3), 0.0f,
                    //    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level9minerals.ToString(), level9ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }
            
            Vector2 level10TextPosition = new Vector2(viewportWidth * 2 / 3, viewportHeight * 22/100);
            Vector2 level10BarPosition = new Vector2(level10TextPosition.X + (levelTextWidth / 3), level10TextPosition.Y + levelTextHeight);
            Vector2 level10ResourcesPosition = new Vector2(level10BarPosition.X, level10TextPosition.Y + levelTextHeight / 2);

            if (levelCompleted == 10)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                spriteBatch.Draw(mLevelText10, level10TextPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar10, level10BarPosition, null, new Color(255, 0, 0, 255), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);   
            }
            else if (levelCompleted > 10)
            {
                spriteBatch.Draw(mLevelText10, level10TextPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);
                spriteBatch.Draw(mMapBar10, level10BarPosition, null, new Color(128, 128, 128, 50), 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                if (level10minerals != 0)
                {
                   // spriteBatch.Draw(mLetterB, level10ResourcesPosition, null, GetResourceColor(3), 0.0f,
                    //    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                    spriteBatch.DrawString(mFont, "  " + level10minerals.ToString(), level10ResourcesPosition, GetResourceColor(3), 0.0f,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.15f);

                }
            }


            

        }

        private Color GetResourceColor(int index)
        {
            Color color = (index == LevelManager.Level.Number) ? Color.White :  new Color(50,0,0, 20);
            return color;
        }

    }

}
