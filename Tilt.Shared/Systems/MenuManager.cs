using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Systems
{

    /*
     * This class is responsible for handling all Menu related operations
     * Most of these methods are wired up to a button, and are called via a delegate
     * in MenuActionComponent / MenuActionArgsComponent or raised by the EventSystem
     */
    public static class MenuManager
    {
        private static int mResourceSpentOverLevel;

        public static void Initialize()
        {
            EventSystem.SubScribe(EventType.TowerSelected, OnTowerSelected_);
            EventSystem.SubScribe(EventType.SellObject, OnSellObject_);
            EventSystem.SubScribe(EventType.PauseButtonPressed, OnPausePressed_);
            EventSystem.SubScribe(EventType.StartMenu, OnGoToStartMenu_);
            EventSystem.SubScribe(EventType.ReloadLevel, OnRestartLevel_);
            EventSystem.SubScribe(EventType.GameOver, OnGameOver_);
            EventSystem.SubScribe(EventType.LevelRecap, OnLevelRecap_);
            EventSystem.SubScribe(EventType.SlidingPanelClose, OnSlidingPanelClose_);
            EventSystem.SubScribe(EventType.PausePanelOpen, OnPausePanelOpen_);
            EventSystem.SubScribe(EventType.PausePanelClose, OnPausePanelClose_);
            EventSystem.SubScribe(EventType.InfoPanelOpen, OnInfoPanelOpen_);
            EventSystem.SubScribe(EventType.InfoPanelClose, OnInfoPanelClose_);
            EventSystem.SubScribe(EventType.TowerDestroyed, OnObjectDestroyed_);
        }

        private static void OnPausePressed_(object sender, IGameEventArgs e)
        {
            SystemsManager.Instance.IsPaused = !SystemsManager.Instance.IsPaused;
        }

        private static void OnRestartLevel_(object sender, IGameEventArgs e)
        {
            
            SystemsManager.Instance.IsPaused = true;

            Layer hudLayer = LayerManager.GetLayer(LayerType.Hud);
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            gameLayer.EntitySystem.UnRegisterAll();
            while (LayerManager.Layers.Count > 0)
                LayerManager.Pop();

            LayerManager.Push(gameLayer);
            LayerManager.Push(hudLayer);

            LayerManager.SetLayer(LayerType.Game);

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Camera camera = new Camera(0, 0, graphicsDevice.Viewport);
            Map map = new Map(0, 0, "background2");
            SelectionBox selectionBox = new SelectionBox(Rectangle.Empty, "selectionbox", new Rectangle(0, 0, 36, 36), 0.18f, 1, 1);


            LevelManager.LoadLevel();
        }

        public static void LoadNextLevel(int levelNumber)
        {
            LayerManager.TransitionTo(LayerType.Game);
            LayerManager.Push(LayerType.Hud, true);

            UIOps.EnableDisableHudButtons(true);

            LayerManager.SetLayer(LayerType.Game);

            LevelManager.LoadLevel(levelNumber);
        }

        private static void OnSellObject_(object sender, IGameEventArgs e)
        {
            TileNode selectedTile = TileMap.SelectedTile;

            if (selectedTile != null && 
                !selectedTile.HasObject && 
                !(selectedTile.Object is IPlaceable))
                return;

            ISellable sellable = selectedTile.Object.Data as ISellable;

            TileMap.SellObject(selectedTile.Object);

            InfoPanel infoPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<InfoPanel>().FirstOrDefault();
            InfoPanelPositionComponent positionComponent = infoPanel.PositionComponent as InfoPanelPositionComponent;
            positionComponent.IsSlidingOut = true;

            Resources.Minerals += (uint)sellable.PriceToSell;

        }
        
        private static void OnPausePanelOpen_(object sender, IGameEventArgs e)
        {
            if (LayerManager.Layer.Type == LayerType.GameMenuOverlay)
                return;

            SystemsManager.Instance.IsPaused = true;
            LayerManager.Push(LayerType.GameMenuOverlay, true);

            PauseMenuPanel panel = LayerManager.Layer.EntitySystem.GetEntitiesByType<PauseMenuPanel>().FirstOrDefault();
            PauseMenuPanelPositionComponent positionComponent = panel.PositionComponent as PauseMenuPanelPositionComponent;
            positionComponent.IsSlidingIn = true;

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXPanelSlide
            });
            
        }

        private static void OnPausePanelClose_(object sender, IGameEventArgs e)
        {
            LayerManager.Pop(true);
            LayerManager.SetLayer(LayerType.Game);


        }

        private static void OnInfoPanelOpen_(object sender, IGameEventArgs e)
        {
            if (LayerManager.Layer.Type == LayerType.Info)
                return;

            LayerManager.Push(LayerType.Info, true);

            InfoPanel infoPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<InfoPanel>().FirstOrDefault();
            InfoPanelPositionComponent positionComponent = infoPanel.PositionComponent as InfoPanelPositionComponent;
            positionComponent.IsSlidingIn = true;

            Button pauseButton = UIOps.FindElementByName("PauseButton") as Button;
            Button settingsButton = UIOps.FindElementByName("SettingsButton") as Button;

            pauseButton.AnimationComponent.IsEnabled = false;
            settingsButton.AnimationComponent.IsEnabled = false;


            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXPanelSlide
            });
        }

        private static void OnInfoPanelClose_(object sender, IGameEventArgs e)
        {
            LayerManager.Pop(true);
            LayerManager.SetLayer(LayerType.Game);
            
            Button pauseButton = UIOps.FindElementByName("PauseButton") as Button;
            Button settingsButton = UIOps.FindElementByName("SettingsButton") as Button;

            pauseButton.AnimationComponent.IsEnabled = true;
            settingsButton.AnimationComponent.IsEnabled = true;

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXPanelSlide
            });
        }

        public static void OpenBuildPanel()
        {
            if (LayerManager.Layer.Type != LayerType.Game)
                return;

            if(SystemsManager.Instance.SelectionMode != SelectionMode.Build)
                SystemsManager.Instance.SelectionMode = SelectionMode.Build;

            if (LayerManager.Peek() != LayerType.TowerSelect)
            {
                LayerManager.Push(LayerType.TowerSelect, true);

                SlidingPanel slidingPanel =
                    LayerManager.Layer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
                SlidingPanelPositionComponent positionComponent = slidingPanel.PositionComponent as SlidingPanelPositionComponent;
                positionComponent.IsSlidingIn = true;

                TowerSelectList towerSelectList =
                    LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();

                TowerSelectListPositionComponent selectListPos = towerSelectList.PositionComponent as TowerSelectListPositionComponent;
                selectListPos.IsSlidingIn = true;

                EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                {
                    Play = true,
                    SoundEffect = Tuner.SFXPanelSlide
                });
            }
        }

        public static bool ConfirmBuildObjects()
        {
            TowerSynchronizer towerSynchronizer = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();
            if (towerSynchronizer == null)
                return false;
            List<IPlaceable> placedObjects = towerSynchronizer.TouchComponent.PlacedObjects;

            uint cost = (uint)placedObjects.Select(t => t.Data as ISellable).Sum(t => t.PriceToBuy);

            if (cost > Resources.Minerals)
            {
                EventSystem.EnqueueEvent(EventType.NotificationWindowOpened, null, new NotificationArgs() { Text = "Not enough minerals." });
                return false;
            }
            SystemsManager.Instance.SelectionMode = SelectionMode.Normal;
            Resources.Minerals -= cost;

            
            TileMap.SetObjectsOnTiles(placedObjects);

            List<IPlaceable> buildables = placedObjects.Where(e => e is IBuildable).ToList();
            List<IPlaceable> sellables = buildables.Where(b => b.Data is ISellable).ToList();
            int resourceSpent = sellables.Where(p => p.Data is ISellable).Select(p => p.Data as ISellable).Sum(p => p.PriceToBuy);
            Resources.ResourcesSpentOverLevel += resourceSpent;

            //determine if there are any units on the tiles that the towers
            //are placed on. if so, let the tower squash it, and the unit will be destroyed
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            if(gameLayer == null)
                return false;

            List<Unit> units = LayerManager.Layer.EntitySystem.GetEntitiesByType<Unit>();

            Layer previousLayer = LayerManager.Layer;

            LayerManager.SetLayer(gameLayer);
            
            foreach(IPlaceable placeable in placedObjects)
            {
                List<Unit> collidingUnits = units.Where(u => (Vector2.Distance(u.PositionComponent.Origin, placeable.PositionComponent.Origin) < TileMap.TileWidth)).ToList();
                
                foreach(Unit unit in collidingUnits)
                {
                    unit.UnRegister();
                    EventSystem.EnqueueEvent(EventType.UnitDestroyed, unit, new UnitDestroyedArgs() { DestroyingEntity = placeable as Entity});
                    Resources.UnitsDestroyedOverLevel++;
                }

                TowerSlamParticle particle = new TowerSlamParticle(
                    "projectiles",
                    new Rectangle(32, 128, 30, 32),
                    (int)placeable.PositionComponent.Position.X,
                    (int)placeable.PositionComponent.Position.Y,
                    1,
                    4,
                    0.08f);



            }

            LayerManager.SetLayer(previousLayer);


            towerSynchronizer.UnRegister();

            Entity obj = placedObjects.First() as Entity;
            EventSystem.EnqueueEvent(EventType.SoundEffect, obj, new SoundEffectArgs()
            {
                Id = obj.Id,
                Play = false,
                SoundEffect = "Powerup2"
            });

            return true;
        }

        public static void ConfirmTowers()
        {
            if (!ConfirmBuildObjects())
                return;

            SlidingPanel slidingPanel =  LayerManager.Layer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            TowerSelectList towerSelectList = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();

            SlidingPanelPositionComponent slidingPanelPositionComponent = slidingPanel.PositionComponent as SlidingPanelPositionComponent;
            slidingPanelPositionComponent.IsSlidingOut = true;

            TowerSelectListPositionComponent towerSelectListPositionComponent = towerSelectList.PositionComponent as TowerSelectListPositionComponent;
            towerSelectListPositionComponent.IsSlidingOut = true;

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXPanelSlide
            });

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXObjectBuildAll
            });
        }

        public static void SelectNewTower()
        {
            TowerSelectList towerSelectList = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();
            SlidingPanel slidingPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            TowerSynchronizer towerSynchronizer = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

            towerSynchronizer.Type = ObjectType.None;
            towerSelectList.RenderComponent.IsVisible = true;
            slidingPanel.PanelAction = PanelAction.TowerSelecting;
        }

        public static void UndoTower()
        {
            TowerSynchronizer towerSynchronizer = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

            if (towerSynchronizer == null)
                return;

            Layer previousLayer = LayerManager.Layer;
            LayerManager.Layer = LayerManager.GetLayer(LayerType.Game);

            TowerSyncTouchComponent touchComponent = towerSynchronizer.TouchComponent;
            if (touchComponent.PlacedObjects.Count > 0)
            {
                IPlaceable obj = touchComponent.PlacedObjects.LastOrDefault();
                Entity entity = obj as Entity;
                TileCoord tileCoord = GeometryOps.PositionToTileCoord(obj.PositionComponent.Position);
                TileNode tileNode = TileMap.GetTileNode(tileCoord.X, tileCoord.Y);
                tileNode.Type = TileType.Empty;
                entity.UnRegister();
                touchComponent.PlacedObjects.Remove(obj);

                if (touchComponent.PlacedObjects.Count == 0)
                {
                    SlidingPanel slidingPanel = 
                        LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
                    slidingPanel.PanelAction = PanelAction.TowerSelected;
                }
            }

            LayerManager.Layer = previousLayer;
                                



        }

        public static void Credits()
        {
            LayerManager.TransitionTo(LayerType.Credits, true);

            DialogueTextRenderer textRenderer = LayerManager.Layer.EntitySystem.GetEntitiesByType<DialogueTextRenderer>().First();

            if(textRenderer != null)
            {
                textRenderer.TextRenderComponent.SetText("credits_text");
            }
        }

        public static void BuildPanelBack()
        {
            SlidingPanel slidingPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            TowerSelectList towerSelectList = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();
            TowerSynchronizer towerSynchronizer = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

            SlidingPanelPositionComponent positionComponent = slidingPanel.PositionComponent as SlidingPanelPositionComponent;
            positionComponent.IsSlidingOut = true;

            TowerSelectListPositionComponent posComponent = towerSelectList.PositionComponent as TowerSelectListPositionComponent;
            posComponent.IsSlidingOut = true;

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXPanelSlide
            });


            if (towerSynchronizer == null)
                return;

            Layer previousLayer = LayerManager.Layer;
            LayerManager.Layer = LayerManager.GetLayer(LayerType.Game);

            foreach (IPlaceable obj in towerSynchronizer.TouchComponent.PlacedObjects)
            {
                Entity entity = obj as Entity;

                TileCoord tileCoord = GeometryOps.PositionToTileCoord(obj.PositionComponent.Position);
                TileNode tileNode = TileMap.GetTileNode(tileCoord.X, tileCoord.Y);
                tileNode.Type = TileType.Empty;

                entity.UnRegister();
            }

            LayerManager.Layer = previousLayer;

            towerSynchronizer.UnRegister();
        }

        private static void OnSlidingPanelClose_(object sender, IGameEventArgs e)
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            if (gameLayer == null)
                return;

            TowerSynchronizer towerSynchronizer = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();
            if (towerSynchronizer == null)
                SystemsManager.Instance.SelectionMode = SelectionMode.Normal;

            TowerSelectList towerSelectList = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();
            towerSelectList.RenderComponent.IsVisible = true;

            Layer layer = LayerManager.GetLayer(LayerType.TowerSelect);

            LayerManager.Pop(true);
            LayerManager.SetLayer(gameLayer);

        }

        public static void SelectBuildObject(object obj1)
        {
            SelectBuildObject(obj1, null);
        }

        public static void SelectBuildObject(object obj1, object obj2)
        {
            TowerSynchronizer towerSynchronizer = LayerManager.Layer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();
            SlidingPanel slidingPanel = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
            TowerSelectList towerSelectList = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();
            
            ObjectType objectType = (ObjectType) obj1;
            
            if( towerSynchronizer == null) 
                towerSynchronizer = new TowerSynchronizer(objectType);

            towerSynchronizer.Type = objectType;

            if (obj2 is TowerType)
                towerSynchronizer.TowerType = (TowerType) obj2;
            if (obj2 is AddOnType)
                towerSynchronizer.AddOnType = (AddOnType) obj2;

            if (towerSynchronizer.TouchComponent.PlacedObjects.Count == 0)
                slidingPanel.PanelAction = PanelAction.TowerSelected;  
            else
                slidingPanel.PanelAction = PanelAction.TowerPlaced;

            TowerSelectListRenderComponent listRenderComponent = towerSelectList.RenderComponent as TowerSelectListRenderComponent;
            listRenderComponent.IsVisible = false;
#if !WINDOWS
            TouchOps.ClearTouch();
#endif
            EventSystem.EnqueueEvent(EventType.MapScrolled);

        }

        public static void StartGame()
        {
            SystemsManager.Instance.IsPaused = true;

            LevelManager.SaveFile.Reset();
            LevelManager.Level = LevelManager.Levels.First();
            AssetOps.Serializer.SerializeSaveFile();
            
            StartTutorial();

        }

        public static void StartTutorial()
        {
            SystemsManager.Instance.InTutorial = true;

            LayerManager.TransitionTo(LayerType.Game);
            LayerManager.Push(LayerType.Hud, true);


            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            Tutorial tutorial = new Tutorial(viewport.Width, viewport.Height, "victoryoverlaypanel", true, "TutorialScreen" );
           

            Layer hudLayer = LayerManager.Layer;

            LayerManager.SetLayer(LayerType.Game);

            Button buildButton = UIOps.FindElementByName("BuildButton") as Button;
            Button pauseButton = UIOps.FindElementByName("PauseButton") as Button;
            Button settingsButton = UIOps.FindElementByName("SettingsButton") as Button;
            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;

            pauseButton.AnimationComponent.IsVisible = false;
            buildButton.AnimationComponent.IsVisible = false;
            settingsButton.AnimationComponent.IsVisible = false;
            infoBar.RenderComponent.IsVisible = false;

            LevelManager.LoadLevel(LevelManager.Level.Number);


        }
        
        public static void HideTutorial()
        {
            Button buildButton = UIOps.FindElementByName("BuildButton") as Button;
            Button pauseButton = UIOps.FindElementByName("PauseButton") as Button;
            Button settingsButton = UIOps.FindElementByName("SettingsButton") as Button;
            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;
            Tutorial tutorial = UIOps.FindElementByName("TutorialScreen") as Tutorial;

            pauseButton.AnimationComponent.IsVisible = true;
            buildButton.AnimationComponent.IsVisible = true;
            settingsButton.AnimationComponent.IsVisible = true;
            infoBar.RenderComponent.IsVisible = true;

            tutorial.UnRegister();


        }

        public static void OpenWorldMap()
        {
            int levelCompleted = LevelManager.SaveFile.LevelCompleted;

            if (levelCompleted == LevelManager.Levels.Count)
            {
                LayerManager.TransitionTo(LayerType.StartMenu, true);
            }
            else
            {
                LayerManager.TransitionTo(LayerType.WorldMap, true);

                DialogueTextRenderer textRenderer = LayerManager.Layer.EntitySystem.GetEntitiesByType<DialogueTextRenderer>().First();

                if (textRenderer != null)
                {
                    if (levelCompleted == LevelManager.Levels.Count) //beat the game
                    {
                        textRenderer.TextRenderComponent.SetText("level_" + (levelCompleted) + "_string");
                    }
                    else
                    {
                        textRenderer.TextRenderComponent.SetText("level_" + (levelCompleted + 1) + "_string");
                    }
                }
            }
        }

        public static void ContinueGame()
        {
            SystemsManager.Instance.IsPaused = true;

            LayerManager.TransitionTo(LayerType.Game);
            LayerManager.Push(LayerType.Hud, true);
            LayerManager.SetLayer(LayerType.Game);

            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;
            infoBar.TimeComponent.Resume();

            int levelToLoad = (LevelManager.SaveFile.LevelCompleted != LevelManager.Levels.Count)
                ? LevelManager.SaveFile.LevelCompleted + 1
                : LevelManager.SaveFile.LevelCompleted;

            UIOps.EnableDisableHudButtons(true);

            LevelManager.LoadLevel(levelToLoad);
        }

        public static void QuitGame()
        {
            SystemsManager.Instance.Quit = true;
        }

        private static void OnGameOver_(object sender, IGameEventArgs e)
        {
            LayerManager.Push(LayerType.GameOver, true);

            SystemsManager.Instance.IsPaused = true;
            UIOps.EnableDisableHudButtons(false);
            
            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;
            if (infoBar != null)
            {
                infoBar.PauseTime();
            }

            LevelManager.SaveFile.Reset();

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXUILevelComplete
            });
        }

        private static void OnGoToStartMenu_(object sender, IGameEventArgs e)
        {
            ResetPanels_();

            UIOps.EnableDisableHudButtons(true);

            LayerManager.TransitionTo(LayerType.StartMenu, true);

            Button button = UIOps.FindElementByName("StartMenuContinueGameButton") as Button;
            button.AnimationComponent.IsVisible = (LevelManager.SaveFile.LevelCompleted > 0);

            

            StartMenuLogo startMenuLogo = LayerManager.Layer.EntitySystem.GetEntitiesByType<StartMenuLogo>().First();
            if(startMenuLogo != null)
            {
                startMenuLogo.RenderComponent.ResetScale();
            }

        }

        private static void OnLevelRecap_(object sender, IGameEventArgs e)
        {
            LevelRecapArgs args = e as LevelRecapArgs;

            ResetPanels_();
            UIOps.EnableDisableHudButtons(false);
            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;
            infoBar.PauseTime();



            LayerManager.Push(LayerType.LevelRecap, true);

            VictoryRecapPanel recapPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<VictoryRecapPanel>().FirstOrDefault();
            VictoryRecapPanelRenderComponent renderComponent = recapPanel.RenderComponent as VictoryRecapPanelRenderComponent;

            int towerRefund = args.TowerRefund;
            Resources.Minerals += (uint)towerRefund;

            LevelManager.Save();

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = Tuner.SFXUILevelComplete
            });

            EventSystem.EnqueueEvent(EventType.LevelComplete);
        }

        private static void ResetPanels_()
        {
            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            Layer infoLayer = LayerManager.GetLayer(LayerType.Info);
            Layer pauseMenuLayer = LayerManager.GetLayer(LayerType.GameMenuOverlay);

            if (towerSelectLayer != null)
            {
                SlidingPanel slidingPanel = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();
                TowerSelectList towerSelectList = LayerManager.GetLayer(LayerType.TowerSelect).EntitySystem.GetEntitiesByType<TowerSelectList>().FirstOrDefault();

                slidingPanel.Reset();
                towerSelectList.Reset();
            }

            if (infoLayer != null)
            {
                InfoPanel infoPanel = infoLayer.EntitySystem.GetEntitiesByType<InfoPanel>().FirstOrDefault();
                infoPanel.Reset();
            }

            if (pauseMenuLayer != null)
            {
                PauseMenuPanel pauseMenuPanel = pauseMenuLayer.EntitySystem.GetEntitiesByType<PauseMenuPanel>().FirstOrDefault();
                pauseMenuPanel.Reset();
            }
        }

        private static void OnTowerSelected_(object sender, IGameEventArgs e)
        {
            TowerSelectedArgs args = e as TowerSelectedArgs;
            if (args == null || args.EventType != EventType.TowerSelected)
                return;

            Entity obj = args.Object;
            IPlaceable placeable = obj as IPlaceable;
            IData data = placeable.Data;

        }

        private static void OnObjectDestroyed_(object sender, IGameEventArgs e)
        {
            if (!(sender is IPlaceable))
                return;

            IPlaceable tower = sender as IPlaceable;

            if (TileMap.SelectedTile != null &&
                TileMap.SelectedTile.Object == tower)
            {
                TileMap.SelectedTile = null;

                if (LayerManager.Layer.Type == LayerType.Info)
                {
                    InfoPanel infoPanel = LayerManager.Layer.EntitySystem.GetEntitiesByType<InfoPanel>().FirstOrDefault();
                    InfoPanelPositionComponent positionComponent = infoPanel.PositionComponent as InfoPanelPositionComponent;
                    positionComponent.IsSlidingOut = true;
                }
            }

            TileCoord tileCoord = GeometryOps.PositionToTileCoord(tower.PositionComponent.Position);
            TileMap.RemoveObjectOnTile(tileCoord.X, tileCoord.Y);
        }

    }
}
