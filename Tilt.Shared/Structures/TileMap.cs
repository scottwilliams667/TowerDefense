using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Structures
{
    public enum TileType
    {
        //Object is built on it, but build is not confirmed yet
        Occupied,
        //nothing in tile
        Empty, 
        //obj on tile
        Placed,
        //obj is water etc
        Impassable
    }

    public class TileNode
    {
        private Tile mTile;
        private IPlaceable mObject;
        private TileType mType;

        public TileNode(Tile tile, TileType type)
        {
            mTile = tile;
            mType = type;
        }

        public bool IsTowerPlaced
        {
            get { return mType == TileType.Placed; }
        }

        public bool HasObject
        {
            get { return mObject != null; }
        }

        public Tile Tile
        {
            get { return mTile; }
            set { mTile = value; }
        }

        public IPlaceable Object
        {
            get { return mObject;}
            set { mObject = value; }
        }

        public TileType Type
        {
            get { return mType; }
            set { mType = value; }
        }
    }

    public static class TileMap
    {
        private const int kTileWidth = 32;
        private const int kTileHeight = 32;
        private const int kMapWidth = 35;
        private const int kMapHeight = 16;
        private static TileNode[,] mTiles;
        private static TileCoord mBase;
        private static TileNode mSelectedTile;
        

        static TileMap()
        {
            mTiles = new TileNode[kMapHeight,kMapWidth];
            EventSystem.SubScribe(EventType.SelectionModeChanged, OnSelectionModeChanged_);
        }

        public static TileCoord Base
        {
            get { return mBase; }
            set
            {
                if (mBase != null)
                {
                    GetTileNode(mBase.X, mBase.Y).Type = TileType.Empty;
                    GetTileNode(mBase.X + 1, mBase.Y).Type = TileType.Empty;
                    GetTileNode(mBase.X, mBase.Y + 1).Type = TileType.Empty;
                    GetTileNode(mBase.X + 1, mBase.Y + 1).Type = TileType.Empty;
                }
                mBase = value;
                if (mBase != null)
                {
                    GetTileNode(mBase.X, mBase.Y).Type = TileType.Occupied;
                    GetTileNode(mBase.X + 1, mBase.Y).Type = TileType.Occupied;
                    GetTileNode(mBase.X, mBase.Y + 1).Type = TileType.Occupied;
                    GetTileNode(mBase.X + 1, mBase.Y + 1).Type = TileType.Occupied;
                }
            }
        }

        public static TileNode SelectedTile
        {
            get { return mSelectedTile; }
            set
            {
                mSelectedTile = value;
                EventSystem.EnqueueEvent(EventType.SelectedTileChanged, mSelectedTile, null);
            }
        }

        public static void LoadTiles(List<TileCoord> deadTiles, List<TileCoord> spawnTiles, List<ResourceTile> resourceTiles)
        {
            int mapOffsetX = Tuner.MapStartPosX;
            int mapOffsetY = Tuner.MapStartPosY;

            mTiles = new TileNode[kMapHeight,kMapWidth];
            for (int i = 0; i < mTiles.GetLength(1); i++)
            {
                for (int j = 0; j < mTiles.GetLength(0); j++)
                {
                    Tile tile = new Tile(new Rectangle(i * kTileWidth + mapOffsetX, j * kTileWidth + mapOffsetY, kTileWidth, kTileHeight));
                    TileNode tileNode = new TileNode(null, TileType.Empty);
                    tileNode.Tile = tile;
                    mTiles[j,i] = tileNode;

                }
            }

            foreach (TileCoord deadTile in deadTiles)
            {
                GetTileNode(deadTile.X, deadTile.Y).Type = TileType.Impassable;
            }
            foreach (TileCoord spawnTile in spawnTiles)
            {
                GetTileNode(spawnTile.X, spawnTile.Y).Type = TileType.Occupied;
            }
            foreach (ResourceTile resourceTile in resourceTiles)
            {
                //GetTileNode(resourceTile.X, resourceTile.Y).Type = TileType.Occupied;
            }

        }

        public static TileNode[,] Tiles
        {
            get { return mTiles; }
        }

        /// When objects are confirmed for placing, this function runs through
        /// and places all the objects on the tiles
        public static void SetObjectsOnTiles(List<IPlaceable> objects)
        {
            foreach (IPlaceable obj in objects)
            {
                IBuildable buildable = obj as IBuildable;
                buildable.Enabled = true;
                TileCoord tileCoord = GeometryOps.PositionToTileCoord(obj.PositionComponent.Position);
                TileNode tileNode = GetTileNode(tileCoord.X, tileCoord.Y);
                tileNode.Object = obj;
                tileNode.Type = TileType.Occupied;
            }

            EventSystem.EnqueueEvent(EventType.MapChanged, objects, new MapChangedArgs());
            EventSystem.EnqueueEvent(EventType.TowerAdded, objects, null);
        }

        //responsible for selling all the highlighted objects
        public static void SellObject(IPlaceable obj)
        {
            TileCoord tileCoord = GeometryOps.PositionToTileCoord(obj.PositionComponent.Position);
            TileNode tileNode = GetTileNode(tileCoord.X, tileCoord.Y);
            Entity entity = tileNode.Object as Entity;
            entity.UnRegister();
            tileNode.Object = null;
            tileNode.Type = TileType.Empty;

            EventSystem.EnqueueEvent(EventType.MapChanged, obj, new MapChangedArgs());
            EventSystem.EnqueueEvent(EventType.TowerRemoved, obj, null);

            obj = null;
            SelectedTile = null;
        }

        public static void RemoveObjectOnTile(int x, int y)
        {
            TileNode tileNode = GetTileNode(x, y);
            if (!tileNode.HasObject)
                return;

            Entity entity = tileNode.Object as Entity;
            tileNode.Type = TileType.Empty;
            entity.UnRegister();

            tileNode.Object = null;

            EventSystem.EnqueueEvent(EventType.TowerRemoved, entity, null);
            EventSystem.EnqueueEvent(EventType.MapChanged, entity, new MapChangedArgs());
        }

        public static TileNode GetTileNode(int x, int y)
        {
            try
            {
                return mTiles[y, x];
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        private static TileNode GetTileForPosition(int x, int y)
        {
            return GetTileNode((x - Tuner.MapStartPosX)/kTileWidth, (y - Tuner.MapStartPosY)/kTileHeight);
        }

        public static TileNode GetTileForPosition(float x, float y)
        {
            return GetTileForPosition((int) x, (int) y);
        }

        public static int Width
        {
            get { return mTiles.GetLength(1) * kTileWidth; }
        }

        public static int Height 
        {
            get { return mTiles.GetLength(0) * kTileHeight; }
        }

        public static int TileWidth
        {
            get { return kTileWidth; }
        }

        public static int TileHeight
        {
            get { return kTileHeight; }
        }

        private static void OnSelectionModeChanged_(object sender, IGameEventArgs e)
        {
            SelectionMode mode = (SelectionMode) sender;
            if (mode == SelectionMode.Build || mode == SelectionMode.Sell)
            {
                EventSystem.EnqueueEvent(EventType.TowerDeselected, null, null);
                SelectedTile = null;
            }
        }

    }
}
