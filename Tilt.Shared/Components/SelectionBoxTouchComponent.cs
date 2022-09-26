#define WIN32

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Structures;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class SelectionBoxTouchComponent : TouchAreaComponent
    {
        public SelectionBoxTouchComponent(Rectangle bounds, Entity owner, bool register = true) : base( bounds, owner, register)
        {
        }

        public override void Update()
        {
#if WINDOWS
            if(MouseOps.IsClick())
            
#else 
            if (TouchOps.IsTap())
#endif
            
            {
#if WINDOWS
                Vector2 touchLocation = MouseOps.GetPosition();
#else
                Vector2 touchLocation = TouchOps.GetPosition();
#endif
                Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
                if (gameLayer == null || LayerManager.HasLayer(LayerType.GameMenuOverlay) ||
                    LayerManager.HasLayer(LayerType.LevelRecap))
                    return;

                Vector2 worldLocation = Vector2.Transform(touchLocation, Matrix.Invert(gameLayer.Matrix));
                TileCoord tileCoord = GeometryOps.PositionToTileCoord(worldLocation);
                TileNode tileNode = TileMap.GetTileNode(tileCoord.X, tileCoord.Y);

                if (tileNode == null || tileNode.Object == null)
                    return;

                if (tileNode == TileMap.SelectedTile)
                    return;

                if (SystemsManager.Instance.SelectionMode == SelectionMode.Build && tileNode.Type == TileType.Placed)
                    TileMap.SelectedTile = tileNode;
                if (SystemsManager.Instance.SelectionMode == SelectionMode.Normal && tileNode.Type == TileType.Occupied)
                {
                    TileMap.SelectedTile = tileNode;

                    SelectionBoxAnimationComponent animationComponent = (Owner as SelectionBox).AnimationComponent;
                    animationComponent.CurrentColumnIndex = 0;
                    animationComponent.CurrentTime = animationComponent.Interval;

                    EventSystem.EnqueueEvent(EventType.TowerSelected, this, new TowerSelectedArgs() { Object = TileMap.SelectedTile.Object as Entity });
                    EventSystem.EnqueueEvent(EventType.InfoPanelOpen);
                }
                if (SystemsManager.Instance.SelectionMode == SelectionMode.Sell && tileNode.HasObject)
                    TileMap.SelectedTile = tileNode;
            }

        }


    }
}
