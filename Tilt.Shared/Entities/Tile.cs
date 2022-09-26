using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    [System.Diagnostics.DebuggerDisplay("X: {X} Y: {Y}")]
    public class TileCoord
    { 

        public int X { get; set; }
        public int Y { get; set; }

    }

    public enum ResourceType
    {
        Gold
    }

    public class ResourceTile : TileCoord
    {
        public ResourceType ResourceType { get; set; }

        public int Amount { get; set; }
    }

    public class Tile : Entity
    {
        private PositionComponent mPositionComponent;
        private TileRenderComponent mRenderComponent;

        public Tile(Rectangle bounds) : base()
        {
            mPositionComponent = new PositionComponent(bounds.X, bounds.Y, this);
           // mTouchComponent = new TouchAreaComponent(bounds, this);
           // mRenderComponent = new TileRenderComponent("occupied", "empty", this);
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent; }
        }

        public TileRenderComponent RenderComponent
        {
            get { return mRenderComponent;}
            set { mRenderComponent = value; }
        }

        public override void UnRegister()
        {
            mPositionComponent.UnRegister();
            base.UnRegister();
        }
    }
}
