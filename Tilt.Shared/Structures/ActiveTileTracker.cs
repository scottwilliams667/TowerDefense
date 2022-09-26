using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Structures
{
    /*
     * The ActiveTileTracker is responsible for updating the visual set of Tiles that can be seen by
     * the camera. If a tile goes into the camera's view. A render compoent is added which shows
     * whether the user can place a tower on that tile. When a tile goes out of view its render
     * component is unregistered.
     *
     * This class is one of the best written classes and one of the worst at the same time. In order
     * to get all the tiles to render the very first time, we have to call a
     *     EventSystem.EnqueueEvent(EventType.MapScrolled ...);
     * when we create the object. Everty time the camera changes position, it raises a MapScrolledEvent   
     * which then notifys this class.
     * 
     * This class is still fairly fast. It determines which tiles have gone out of the screens view/which 
     * tiles have come into view by keeping a hashset of the visible tiles from the last frame. It then
     * calls UnRegister on any tiles out of view, and registers new RenderComponents for tiles that have
     * come into view
     */
    public class ActiveTileTracker : Entity, IEventChanged
    {
        private HashSet<Tile> mPreviousTileSet = new HashSet<Tile>();
        private HashSet<Tile> mVisibleTileSet = new HashSet<Tile>();
        private HashSet<TileRenderComponent> mTileRenderSet = new HashSet<TileRenderComponent>();
        private EmptyOverlay mEmptyOverlay;

        public ActiveTileTracker()
        {
            mEmptyOverlay = new EmptyOverlay("EmptyOverlay", null);
        }

        public override void Register()
        {
            RegisterEvent();
            base.Register();
        }

        public override void UnRegister()
        {
            UnRegisterEvent();
            foreach (TileRenderComponent tileRender in mTileRenderSet)
                TileRenderPool.ReleaseObject(tileRender);

            mEmptyOverlay.UnRegister();
            mTileRenderSet.Clear();
            mVisibleTileSet.Clear();
            mPreviousTileSet.Clear();
            
            base.UnRegister();
        }


        public void RegisterEvent()
        {
            EventSystem.SubScribe(EventType.MapScrolled, OnEventChanged);
        }

        public void UnRegisterEvent()
        {
            EventSystem.UnSubScribe(EventType.MapScrolled, OnEventChanged);
        }

        public void OnEventChanged(object sender, IGameEventArgs e)
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            

            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();

            Camera camera = gameLayer.EntitySystem.GetEntitiesByType<Camera>().FirstOrDefault();
            CameraPositionComponent positionComponent = camera.PositionComponent;
            Vector2 cameraSize = new Vector2(graphicsDevice.Viewport.Width / camera.PositionComponent.Zoom, graphicsDevice.Viewport.Height / camera.PositionComponent.Zoom);
            Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero,
                Matrix.Invert(
                    Microsoft.Xna.Framework.Matrix.CreateTranslation(new Vector3(-positionComponent.Position, 0)) *
                    Matrix.CreateTranslation(new Vector3(-positionComponent.Origin, 0)) *
                    Matrix.CreateScale(positionComponent.Zoom, positionComponent.Zoom, 1f) *
                    Matrix.CreateTranslation(new Vector3(positionComponent.Origin, 0))));

            Vector2 positionOffset = positionComponent.Position - cameraWorldMin;

            Vector2 cameraLeftPosition = Vector2.Clamp(camera.PositionComponent.Position, cameraWorldMin, new Vector2(TileMap.Width, TileMap.Height));
            Vector2 cameraRightPosition = Vector2.Clamp(new Vector2( cameraLeftPosition.X + cameraSize.X, cameraLeftPosition.Y + cameraSize.Y),
                Vector2.Zero, new Vector2(TileMap.Width, TileMap.Height));

            if (cameraLeftPosition.X + graphicsDevice.Viewport.Width > TileMap.Width)
                cameraLeftPosition.X = TileMap.Width - graphicsDevice.Viewport.Width;

            if (cameraLeftPosition.Y + graphicsDevice.Viewport.Height > TileMap.Height)
                cameraLeftPosition.Y = TileMap.Height - graphicsDevice.Viewport.Height;

            Vector2 snappedCameraLeft = new Vector2((cameraLeftPosition.X / TileMap.TileWidth) * TileMap.TileWidth, (cameraLeftPosition.Y / TileMap.TileHeight) * TileMap.TileHeight);
            Vector2 snappedCameraRight = new Vector2((cameraRightPosition.X / TileMap.TileWidth) * TileMap.TileWidth, (cameraRightPosition.Y / TileMap.TileHeight) * TileMap.TileHeight);

            TileCoord snappedLeft = GeometryOps.PositionToTileCoord(snappedCameraLeft);
            TileCoord snappedRight = GeometryOps.PositionToTileCoord(snappedCameraRight);


            if (snappedLeft.X < 0)
                snappedLeft.X = 0;
            if (snappedLeft.Y < 0)
                snappedLeft.Y = 0;

            // sometimes the camera snaps to the wrong tile, leaving a empty row of neither empty or occupied tiles
            //if so, grab the next row/column just to be safe
            if (snappedRight.X > TileMap.Tiles.GetLength(1))
                snappedRight.X = snappedRight.X - 1;
            else if(snappedRight.X < TileMap.Tiles.GetLength(1) - 1)
            {
                snappedRight.X = snappedRight.X + 1;
            }
            if (snappedRight.Y > TileMap.Tiles.GetLength(0))
                snappedRight.Y = snappedRight.Y - 1;
            else if(snappedRight.Y < TileMap.Tiles.GetLength(0) - 1)
            {
                snappedRight.Y = snappedRight.Y + 1;
            }


            HashSet<Tile> previousTemp = new HashSet<Tile>(mPreviousTileSet);
            HashSet<Tile> previousTemp2 = new HashSet<Tile>(mPreviousTileSet);
                
            mVisibleTileSet = new HashSet<Tile>();

            for(int i = snappedLeft.Y; i < snappedRight.Y; i++)
            {
                for (int j = snappedLeft.X; j < snappedRight.X; j++)
                {
                    TileNode tileNode = TileMap.GetTileNode(j,i);

                    if(tileNode.Type == TileType.Occupied)
                        mVisibleTileSet.Add(tileNode.Tile);

                }
            }


            mPreviousTileSet = new HashSet<Tile>(mVisibleTileSet);
            previousTemp.ExceptWith(mVisibleTileSet);
            mVisibleTileSet.ExceptWith(previousTemp2);

            mEmptyOverlay.Position = cameraLeftPosition;

            foreach (Tile tile in previousTemp)
            {
                TileRenderComponent tileRenderComponent = mTileRenderSet.FirstOrDefault(t => t.Tile == tile);
                if (tile == null || tileRenderComponent == null)
                    continue;
                TileRenderPool.ReleaseObject(tileRenderComponent);
            }
            foreach (Tile tile in mVisibleTileSet)
            {
                //TileRenderComponent renderComponent = new TileRenderComponent("occupied", "empty", tile, false);
                //tile.RenderComponent = renderComponent;
                //gameLayer.RenderSystem.Register(renderComponent);
                TileRenderComponent tileRenderComponent = TileRenderPool.GetObject();
                tileRenderComponent.Tile = tile;
                mTileRenderSet.Add(tileRenderComponent);
            }


        }

    }
}
