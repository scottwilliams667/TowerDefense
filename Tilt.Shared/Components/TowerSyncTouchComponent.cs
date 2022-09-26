using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Structures;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class TowerSyncTouchComponent : TouchAreaComponent
    {
        private List<IPlaceable> mPlacedObjects = new List<IPlaceable>();
        private List<IPlaceable> mRemovedObjects = new List<IPlaceable>(); 
        

        public TowerSyncTouchComponent(Rectangle bounds, Entity owner) : base(bounds, owner)
        {
        }

        public List<IPlaceable> PlacedObjects
        {
            get { return mPlacedObjects; }
            set { mPlacedObjects = value; }
        }

        public List<IPlaceable> RemovedObjects
        { 
            get { return mRemovedObjects; }
            set { mRemovedObjects = value; }
        }

        public override void UnRegister()
        {
            mPlacedObjects = null;
            mRemovedObjects = null;
            base.UnRegister();
        }

        public override void Update()
        {
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            TowerSynchronizer towerSynchronizer = Owner as TowerSynchronizer;
#if WINDOWS
            if (MouseOps.IsClick() &&
                MouseOps.ContainsPoint(mBounds) &&
                SystemsManager.Instance.SelectionMode == SelectionMode.Build)
#else
            if (TouchOps.IsTap() && 
                TouchOps.ContainsPoint(mBounds) &&
                SystemsManager.Instance.SelectionMode == SelectionMode.Build)
#endif
            {
                List<Button> buttons = LayerManager.GetLayer(LayerType.Hud)
                .EntitySystem.Entities.Where(e => e is Button)
                .Cast<Button>().ToList();
#if WINDOWS
                if (buttons.Any(b => b.TouchComponent != null && MouseOps.ContainsPoint(b.TouchComponent.Bounds)))
                    return;
#else
                if(buttons.Any(b => b.TouchComponent != null && TouchOps.ContainsPoint(b.TouchComponent.Bounds)))
                    return;
#endif

                GraphicsDevice viewport = ServiceLocator.GetService<GraphicsDevice>();
                Camera camera = LayerManager.Layer.EntitySystem.GetEntitiesByType<Camera>().FirstOrDefault();
#if WINDOWS
                Vector2 touchLocation = MouseOps.GetPosition();
#else
                Vector2 touchLocation = TouchOps.GetPosition();
#endif
                Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
                Vector2 worldLocation = Vector2.Transform(touchLocation, Matrix.Invert(gameLayer.Matrix));
                
                TileCoord tileCoord = GeometryOps.PositionToTileCoord(worldLocation);
                TileNode tileNode = TileMap.GetTileNode(tileCoord.X, tileCoord.Y);

                if (tileNode == null || tileNode.Type == TileType.Occupied || tileNode.Type == TileType.Placed || tileNode.Type == TileType.Impassable ||
                        tileCoord == TileMap.Base && towerSynchronizer.Type != ObjectType.None)
                {
                    EventSystem.EnqueueEvent(EventType.NotificationWindowOpened, this,
                        new NotificationArgs() { Text = "Cannot build there." });
                    return;
                }


                IPlaceable obj = ObjectFactory.Make(towerSynchronizer, (int)tileCoord.X * TileMap.TileWidth + Tuner.MapStartPosX, (int)tileCoord.Y * TileMap.TileWidth + Tuner.MapStartPosY);
                if (obj != null)
                {

                    
                    tileNode.Type = TileType.Placed;

                    mPlacedObjects.Add(obj);

                    SlidingPanel slidingPanel =
                        LayerManager.Layer.EntitySystem.GetEntitiesByType<SlidingPanel>().FirstOrDefault();

                    if (mPlacedObjects.Count > 0 &&
                        slidingPanel != null &&
                        slidingPanel.PanelAction == PanelAction.TowerSelected)
                    {
                        slidingPanel.PanelAction = PanelAction.TowerPlaced;

                    }

                    EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
                    {
                        Play = true,
                        SoundEffect = Tuner.SFXPlaceTower
                    });
                }
            }
        }
    }
}

