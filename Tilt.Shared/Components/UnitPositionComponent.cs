using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Components
{
    public class UnitPositionComponent : PositionComponent
    {
        private Vector2 mDirection;
        private List<TileCoord> mPath;
        private TileCoord mCurrentTile;
        private TileCoord mEnd;
        private PathFinder mPathFinder;

        public UnitPositionComponent(int x, int y, TileCoord endTileCoords, int speed, Entity owner, Vector2 origin)
            : base(x, y, owner, origin)
        {
            Speed = speed;
            mEnd = endTileCoords;
            mPathFinder = new PathFinder();
        }

        public Vector2 Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        public TileCoord CurrentTile
        {
            get { return mCurrentTile; }
        }

        public override void Register()
        {
            //EventSystem.SubScribe(EventType.MapChanged, OnMapChanged_);
            base.Register();
        }

        public override void UnRegister()
        {
            //EventSystem.UnSubScribe(EventType.MapChanged, OnMapChanged_);
            base.UnRegister();
        }


        public override void Update()
        {

            if (SystemsManager.Instance.IsPaused)
                return;

            Unit unit = Owner as Unit;
            if (mPath == null)
            {
                TileCoord currentTile = GeometryOps.PositionToTileCoord(mPosition);
                mPath = mPathFinder.FindPath((int)currentTile.X, (int)currentTile.Y, (int)mEnd.X, (int)mEnd.Y);
                mCurrentTile = new TileCoord() { X = (int)currentTile.X, Y = (int)currentTile.Y };
                GetDirection_();
            }

            if (mPath.IndexOf(mCurrentTile) == mPath.Count - 1)
            {
                mPath = mPathFinder.FindPath((int)mCurrentTile.X, (int)mCurrentTile.Y, (int)mEnd.X, (int)mEnd.Y);
                GetDirection_();
            }

            //at last tile, centered
            else if (mPosition.X <= mCurrentTile.X * TileMap.TileWidth + Tuner.MapStartPosX && mDirection.X == -1 ||
                     mPosition.X >= mCurrentTile.X * TileMap.TileWidth + Tuner.MapStartPosX && mDirection.X == 1 ||
                     mPosition.Y <= mCurrentTile.Y * TileMap.TileWidth + Tuner.MapStartPosY && mDirection.Y == -1 ||
                     mPosition.Y >= mCurrentTile.Y * TileMap.TileWidth + Tuner.MapStartPosY && mDirection.Y == 1)
            {
                    TileCoord currentTile = mCurrentTile;
                    int index = mPath.IndexOf(mCurrentTile);
                    if (index + 1 == mPath.Count)
                        return;

                    GetDirection_();
                    mPosition.X = currentTile.X * TileMap.TileWidth + Tuner.MapStartPosX;
                    mPosition.Y = currentTile.Y * TileMap.TileWidth + Tuner.MapStartPosY;

            }

            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            mPosition += mDirection * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            unit.BoundsCollisionComponent.Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, unit.BoundsCollisionComponent.Bounds.Width, unit.BoundsCollisionComponent.Bounds.Height);
            mOrigin = new Vector2(mPosition.X + unit.BoundsCollisionComponent.Bounds.Width / 2, mPosition.Y + unit.BoundsCollisionComponent.Bounds.Height / 2);
        }

        private void GetDirection_()
        {
            if (mPath == null)
                return;

            int index = mPath.IndexOf(mCurrentTile);
            if (index + 1 == mPath.Count)
                return;
            TileCoord nextTile = mPath.ElementAt(index + 1);

            if (nextTile.Y < mCurrentTile.Y)
                mDirection = new Vector2(0, -1);
            if (nextTile.Y > mCurrentTile.Y)
                mDirection = new Vector2(0, 1);
            if (nextTile.X < mCurrentTile.X)
                mDirection = new Vector2(-1, 0);
            if (nextTile.X > mCurrentTile.X)
                mDirection = new Vector2(1, 0);

            if (nextTile.Y < mCurrentTile.Y && nextTile.X < mCurrentTile.X)
                mDirection = new Vector2(-1, -1);
            if (nextTile.Y > mCurrentTile.Y && nextTile.X < mCurrentTile.X)
                mDirection = new Vector2(-1, 1);
            if (nextTile.X > mCurrentTile.X && nextTile.Y >  mCurrentTile.Y)
                mDirection = new Vector2(1, 1);
            if (nextTile.X > mCurrentTile.X && nextTile.Y < mCurrentTile.Y)
                mDirection = new Vector2(1, -1);


            mCurrentTile = nextTile;
        }

        //this might get slow after a while
        //private void OnMapChanged_(object sender, IGameEventArgs e)
        //{
        //    //if the creature has been created, but the path has not been found yet
        //    if (mPath != null)
        //        mPath = mPathFinder.FindPath(mCurrentTile.X, mCurrentTile.Y, TileMap.Base.X, TileMap.Base.Y);
        //}


    }



    public class UnitFastWaitPositionComponent : UnitPositionComponent
    {
        private const float kIdleTime = 1.0f;
        private float mIdleTime;
        private bool mIsIdle = false;
        //the number of tiles to move before waiting
        private const int kTilesWait = 3;
        //countdown the number of tiles to move
        private int mTilesMoved;

        public UnitFastWaitPositionComponent(int x, int y, TileCoord endTileCoords, int speed, Entity owner, Vector2 origin)
            : base(x, y, endTileCoords, speed, owner, origin)
        {
            mTilesMoved = kTilesWait;
            mIdleTime = kIdleTime;
        }

        public override void Update()
        {
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if (mIsIdle && mIdleTime <= 0.0f)
            {
                mIdleTime = kIdleTime;
                mTilesMoved = kTilesWait;
                mIsIdle = false;
            }
            else if (mIsIdle)
            {
                mIdleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }

            TileCoord currentTile = CurrentTile;

            base.Update();
            //we have moved a tile, update tile count
            if (currentTile != CurrentTile && mTilesMoved == 0)
            {
                mIsIdle = true;
            }
            else if (currentTile != CurrentTile)
            {
                mTilesMoved--;
            }


        }
    }


}
