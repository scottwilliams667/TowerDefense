using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;
using Tilt.Shared.Structures;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Systems
{
    public sealed class SystemsManager
    {
        private static readonly SystemsManager mInstance = new SystemsManager();
        private bool mIsPaused;
        private bool mInTutorial;
        private bool mQuit;
        private bool mInitialized;
        private bool mInitializing;
        private List<LayerCaps> mPausedLayerCaps; 
        private SelectionMode mSelectionMode = SelectionMode.Normal;
        private FrameCounter mFrameCounter;
        private SpriteFont mFPSFont;
        private Texture2D mLogo;

        static SystemsManager(){ }
        private SystemsManager() { }

        public static SystemsManager Instance
        {
            get { return mInstance; }
        }

        public void Initialize()
        {
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();

            mLogo = AssetOps.LoadAsset<Texture2D>("logo");


            AudioSystem.Initialize();
            MenuManager.Initialize();
            TouchOps.Initialize();
            
            Layer levelSelect = new Layer(LayerType.LevelSelect);
            Layer hudLayer = new Layer(LayerType.Hud);
            Layer startMenuLayer = new Layer(LayerType.StartMenu);
            Layer levelRecapLayer = new Layer(LayerType.LevelRecap);
            Layer towerSelectLayer = new Layer(LayerType.TowerSelect);
            Layer gameOverLayer = new Layer(LayerType.GameOver);
            Layer menuPanelOverlay = new Layer(LayerType.GameMenuOverlay);
            Layer infoPanelLayer = new Layer(LayerType.Info);
            Layer creditsLayer = new Layer(LayerType.Credits);

            


            LayerManager.Push(levelSelect);

            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;
            
            LayerManager.Pop(true);

            LayerManager.Push(hudLayer);


            PauseButton pauseButton = new PauseButton(0, viewportHeight / 16, "playbutton", 1, 2, () => { EventSystem.EnqueueEvent(EventType.PauseButtonPressed, null); }, Tuner.SFXButtonClickSlide, "PauseButton");
            MenuButton settingsButton = new NormalButton(viewportWidth * 90/100,  viewportHeight / 16, "settingsbutton", 1, 1, () => { EventSystem.EnqueueEvent(EventType.PausePanelOpen); }, Tuner.SFXButtonClickSlide, "SettingsButton");

            pauseButton.PositionComponent.Position = new Vector2(settingsButton.PositionComponent.Position.X - settingsButton.AnimationComponent.SourceRectangle.Width - viewportHeight / 32 , pauseButton.PositionComponent.Y);
            pauseButton.TouchComponent.Bounds = new Rectangle((int)pauseButton.PositionComponent.Position.X, (int)pauseButton.PositionComponent.Position.Y, pauseButton.AnimationComponent.SourceRectangle.Width, pauseButton.AnimationComponent.SourceRectangle.Height);

            MenuButton buildButton =
                new NormalButton(0,0, "buildbutton", new Rectangle(0, 0, 0, 0), 1, 1, MenuManager.OpenBuildPanel, Tuner.SFXButtonClick, "BuildButton");

            buildButton.PositionComponent.Position = new Vector2(viewportWidth - buildButton.AnimationComponent.Texture.Width - viewportWidth / 32, viewportHeight - buildButton.AnimationComponent.Texture.Height - viewportHeight / 32);
            buildButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, buildButton.AnimationComponent.Texture.Width, buildButton.AnimationComponent.Texture.Height);
            buildButton.TouchComponent.Bounds = new Rectangle((int)buildButton.PositionComponent.X, (int)buildButton.PositionComponent.Y, buildButton.AnimationComponent.Texture.Width, buildButton.AnimationComponent.Texture.Height);

            InfoBar infoBar = new InfoBar("infobar", 0, viewportHeight / 16, "InfoBar");

            LayerManager.Pop(true);

            LayerManager.Push(towerSelectLayer);

            SlidingPanel slidingPanel = new SlidingPanel(viewportWidth, 0, viewportWidth * 2 / 3, 0, "slidingpanelbackground", PanelAction.TowerSelecting);
            TowerSelectList list = new TowerSelectList(viewportWidth, 0, viewportWidth * 2/3, 0, viewportHeight);

            
            NotificationWindow notificationWindow = new NotificationWindow("notification", "NotificationWindowFont", viewportWidth / 4, viewportHeight, viewportWidth / 2, viewportHeight - TileMap.TileHeight);
            (notificationWindow.PositionComponent as NotificationWindowPositionComponent).YDest = viewportHeight - notificationWindow.RenderComponent.Texture.Height;

            

            MenuButton dismissButton = new NormalButton(viewportWidth / 4 + notificationWindow.RenderComponent.Texture.Width - viewportWidth / 16, viewportHeight, "closebutton", 1, 1, () =>
            {
                EventSystem.EnqueueEvent(EventType.NotificationWindowClosed);

            }, Tuner.SFXButtonClick);
            notificationWindow.DismissButton = dismissButton;



            LayerManager.Pop(true);


            LevelManager.LoadLevels();

            LayerManager.Push(startMenuLayer);

            
            int buttonXOffset = viewportWidth / 6;
            int xPadding = viewportWidth / 24;
            int buttonTextureWidth = viewportWidth / 12;
            int buttonTextureHeight = viewportHeight / 6;

            StartMenuScreen startMenuScreen = new StartMenuScreen("startmenumap", "roller0_strip4", "", "", Rectangle.Empty, Rectangle.Empty, 0, 0, 0);

            PanelState startState = new PanelState()
            {
                Elements = new List<UIElement>()
                {
                    new NormalButton(viewportWidth * 2 / 3 + xPadding / 4, viewportHeight * 16/100 , "continuegamebutton", 1, 1, MenuManager.OpenWorldMap, Tuner.SFXButtonClick, "StartMenuContinueGameButton"),
                    new NormalButton(viewportWidth * 2 / 3 + xPadding / 4, viewportHeight * 29/100, "newgamebutton",  1, 1, MenuManager.StartGame, Tuner.SFXButtonClick, "StartMenuNewGameButton"),
                    new NormalButton(viewportWidth * 2 / 3 + xPadding / 4, viewportHeight * 43/100, "creditsbutton", 1,1, MenuManager.Credits, Tuner.SFXButtonClick, "StartMenuCreditsButton"),
                    new NormalButton(viewportWidth * 2 / 3 + xPadding / 4, viewportHeight * 56/100, "quitbutton",  1, 1, MenuManager.QuitGame, Tuner.SFXButtonClick, "StartMenuQuitGameButton"),
                    new MusicButton((viewportWidth * 2/3) + xPadding / 4, viewportHeight * 74/100, "musicbutton", 2,1, () => {EventSystem.EnqueueEvent(EventType.MuteMusic, null, new MuteMusicArgs());}, Tuner.SFXButtonClick, "StartMenuMuteSoundButton"),
                    new SoundFXButton((viewportWidth * 2/3) + xPadding / 4, viewportHeight * 87/100,"soundfxbutton", 2,1, () => {EventSystem.EnqueueEvent(EventType.MuteSFX, null, new MuteSFXArgs());},Tuner.SFXButtonClick, "StartMenuMuteMusicButton")
                    
                }
            };

            

            StartMenuPanel startMenuPanel = new StartMenuPanel(viewportWidth * 2 / 3, 0, "mainmenubackgroundpanel", startState);

            StartMenuLogo startMenuLogo = new StartMenuLogo("logo", viewportWidth / 16, viewportHeight / 16);
            

            LayerManager.Pop(true);

            LayerManager.Push(levelRecapLayer);

            RecapPanel victoryRecapPanel = new VictoryRecapPanel("victoryscreenbackgroundpanel", viewportWidth / 3 , viewportHeight / 7);

            NormalButton goToNextLevelButton = new NormalButton(viewportWidth / 2 - xPadding, (viewportHeight * 8 / 10), "continuebutton", 1, 1, () => { MenuManager.OpenWorldMap(); }, Tuner.SFXButtonClick);



            LayerManager.Pop(true);

            LayerManager.Push(gameOverLayer);

            DefeatedRecapPanel defeatedRecapPanel = new DefeatedRecapPanel("defeatscreenbackgroundpanel", viewportWidth / 3, viewportHeight / 7);

            MenuArgButton goToMainMenuButton = new MenuArgButton(viewportWidth * 2/3 - 2*buttonTextureWidth, (viewportHeight * 8 / 10), "quitbutton", 1, 1, new Action<object>(
               (o) =>
               {
                   EventSystem.EnqueueEvent(EventType.StartMenu);
               }),
               LevelManager.Level.Number, Tuner.SFXButtonClick, true, "DefeatedRecapBackToMainMenuButton");

            LayerManager.Pop(true);

            LayerManager.Push(LayerType.WorldMap);

            WorldMap worldMap = new WorldMap("soundfxbutton", 0, 0);
            DialogueTextRenderer textRenderer = new DialogueTextRenderer(xPadding / 2, viewportHeight / 6, "WorldMapTextFont", string.Empty);
            

            Button worldMapBackButton = new MenuArgButton(xPadding / 4, viewportHeight * 72/100, "backbutton", 1, 1, new Action<object>(
               (o) =>
               {
                   EventSystem.EnqueueEvent(EventType.StartMenu);
               }),
               LevelManager.Level.Number, Tuner.SFXButtonClick, true,  "WorldMapBackButton");

            Button worldMapContinueButton = new NormalButton(xPadding / 4, viewportHeight * 86 / 100, "continuebutton", 1, 1, () => { MenuManager.ContinueGame(); }, Tuner.SFXButtonClick, "WorldMapContinueButton");

            worldMap.Initialize();

            LayerManager.Pop(true);


            LayerManager.Push(infoPanelLayer);

            InfoPanel infoPanel = new InfoPanel(viewportWidth, 0, viewportWidth * 2 / 3, 0, "infobackgroundpanel",  () => { EventSystem.EnqueueEvent(EventType.InfoPanelClose); });
            

            LayerManager.Pop(true);


            LayerManager.Push(menuPanelOverlay);

            PanelState pauseMenuState = new PanelState(PanelAction.PauseMenu)
            {
                Elements = new List<UIElement>()
                {
                    new NormalButton(viewportWidth + xPadding / 4, viewportHeight / 6, "returntogamebutton",  1, 1, () =>
                    {
                        PauseMenuPanel pauseMenuPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<PauseMenuPanel>().FirstOrDefault();
                        PauseMenuPanelPositionComponent panelPositionComponent = pauseMenuPanel.PositionComponent as PauseMenuPanelPositionComponent;
                        panelPositionComponent.IsSlidingOut = true;
                        EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                        {
                            Play = true,
                            SoundEffect = Tuner.SFXPanelSlide
                        });
                    },
                     Tuner.SFXButtonClickSlide,
                    "PauseMenuResumeGameButton"),
                    new NormalButton(viewportWidth + xPadding / 4, viewportHeight * 3/10, "mainmenubutton", 1, 1, 
                        () => 
                        { 
                            EventSystem.EnqueueEvent(EventType.StartMenu);
                        },
                        Tuner.SFXButtonClick,
                        "PauseMenuGoToStartMenuButton"),
                    new MusicButton(viewportWidth + xPadding / 4, viewportHeight * 57/100 + viewportHeight / 32, "musicbutton", 2, 1, () => {EventSystem.EnqueueEvent(EventType.MuteMusic, null, new MuteMusicArgs());}, Tuner.SFXButtonClick, "PauseMenuMuteSoundButton"),
                    new SoundFXButton(viewportWidth + xPadding / 4, viewportHeight * 70/100 + viewportHeight / 32,"soundfxbutton", 2, 1, () => {EventSystem.EnqueueEvent(EventType.MuteSFX, null, new MuteSFXArgs());}, Tuner.SFXButtonClick, "PauseMenuMuteMusicButton")

                }
            };

            PauseMenuPanel pauseMenu = new PauseMenuPanel(viewportWidth, 0, viewportWidth * 2 / 3, 0, "pausemenubackgroundpanel", pauseMenuState, () => { EventSystem.EnqueueEvent(EventType.PausePanelClose); });

            LayerManager.Pop(true);


            LayerManager.Push(creditsLayer);

            Credits credits = new Credits();
            DialogueTextRenderer creditsText = new DialogueTextRenderer(viewportWidth / 4, viewportHeight * 25/100, "WorldMapTextFont", string.Empty);
            Button backButton = new MenuArgButton(xPadding / 4, viewportHeight * 80 / 100, "backbutton", 1, 1, (o) =>
              {
                  EventSystem.EnqueueEvent(EventType.StartMenu);
              }, 
              LevelManager.Level.Number, Tuner.SFXButtonClick, true, "CreditsBackToMainMenuButton");

            LayerManager.Pop(true);

            LayerManager.Push(LayerType.Game);
            LayerManager.Push(hudLayer);
            LayerManager.SetLayer(LayerType.Game);

            Tuner.MapStartPosX = viewportWidth / 4;
            Tuner.MapStartPosY = viewportHeight / 3;

            LevelManager.LoadLevel();
        }

        public void CreateBaseGameObjects()
        {
            LayerManager.SetLayer(LayerType.Game);

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Camera camera = new Camera(0, 0, graphicsDevice.Viewport);
            Map map = new Map(0, 0, "map1");

            int x = LevelManager.Level.Base.X * TileMap.TileWidth + Tuner.MapStartPosX;
            int y = LevelManager.Level.Base.Y * TileMap.TileHeight + Tuner.MapStartPosY;

            int baseHealth = (LevelManager.Level.Number == 1) ? LevelManager.Level.BaseHealth : LevelManager.SaveFile.BaseHealth;
            Base bse = new Base("buildings_strip6", x , y,
                new Rectangle(64, 0, 2*TileMap.TileWidth, 2*TileMap.TileHeight), 1.0f, 1,3, baseHealth, new BaseData());

            SelectionBox selectionBox = new SelectionBox(Rectangle.Empty, "selectionbox", new Rectangle(0, 0, 36, 36), 0.18f, 1, 1);
            TileMap.SelectedTile = null;
            SelectionMode = SelectionMode.Normal;

            //EventSystem.EnqueueEvent(EventType.MusicChanged, null, new MusicChangedArgs()
            //{
            //    SongName = "title_screen_music",
            //    Play = true,
            //    IsLooping = true
            //});
        }


        public void Update()
        {

            if (!mInitialized && !mInitializing)
            {
                mInitializing = true;
                Task.Factory.StartNew(() =>
                {
                    Initialize();
                    mInitialized = true;
                    mInitializing = false;
                });
            }
            else if(mInitializing)
            {
                //mFPSFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
                //mFrameCounter = new FrameCounter();
            }
            else
            {

               
#if WIN32
                MouseOps.Update();
#endif
                TouchOps.Update();

                Spawner.Update();

                foreach (Layer layer in LayerManager.Layers.ToList())
                {
                    layer.Update();
                }
                EventSystem.Update();
            }
        }

        public void Draw()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if (!mInitialized)
            {
                if (mLogo == null)
                    return;

                GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
                Viewport viewport = graphicsDevice.Viewport;

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
                
                spriteBatch.Draw(mLogo, new Vector2(viewport.Width / 2, viewport.Height / 2), null, Color.White, 0.0f, new Vector2(mLogo.Width / 2, mLogo.Height / 2), 1.0f, SpriteEffects.None, 0.5f);
                
                spriteBatch.End();
            }
            else
            {
                if (mFPSFont == null && mFrameCounter == null)
                {
                  //  mFPSFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
                  
                  //  mFrameCounter = new FrameCounter();
                }


                for (int i = LayerManager.Layers.Count - 1; i >= 0; i--)
                {
                    Layer layer = LayerManager.Layers.ElementAt(i);

                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

                    if (layer.Type == LayerType.Hud)
                    {
                      //  mFrameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                      //  var fps = string.Format("FPS: {0}", mFrameCounter.AverageFramesPerSecond);

                        //spriteBatch.DrawString(mFPSFont, fps, new Vector2(0, 20), Color.White);
                    }

                    layer.Draw();
                    spriteBatch.End();
                }
            }
        }

        public bool Initialized
        {
            get { return mInitialized; }
            set { mInitialized = value; }
        }

        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
                if (mIsPaused != value)
                {

                    mIsPaused = value;
                    if (mIsPaused)
                    {
                        mPausedLayerCaps = LayerManager.Layers.Select(l => l.Caps).ToList();
                        foreach (Layer layer in LayerManager.Layers.ToList())
                        {
                            if (LayerManager.Layers.ToList().IndexOf(layer) != 0)
                                layer.Caps = LayerCaps.Render | LayerCaps.Touch | LayerCaps.Collision | LayerCaps.Position;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < LayerManager.Layers.Count; i++)
                        {
                            Layer layer = LayerManager.Layers.ElementAt(i);
                            layer.Caps = LayerCaps.All;
                        }

                    }
                }
                EventSystem.EnqueueEvent(EventType.PauseChanged, this, null);
            }
        }

        public bool InTutorial
        {
            get { return mInTutorial; }
            set
            {
                mInTutorial = value;
            }
        }

        public SelectionMode SelectionMode
        {
            get { return mSelectionMode; }
            set
            {
                mSelectionMode = value;
                EventSystem.EnqueueEvent(EventType.SelectionModeChanged, mSelectionMode, null);
            }
        }

        public bool Quit
        {
            get { return mQuit; }
            set { mQuit = value; }
        }

    }
}
