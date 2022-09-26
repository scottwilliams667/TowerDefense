using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class AnimationState : AnimationComponent
    {
        public AnimationState(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
        }
    }

    public class AnimationComponent : RenderComponent
    {
        private float mInterval;
        private int mRows;
        private int mColumns;
        private int mCurrentRowIndex;
        private int mCurrentColumnIndex;
        private Rectangle mSourceRectangle;
        private Rectangle mCurrentRectangle;
        private float mCurrentTime;



        public AnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner) : base(texturePath, owner)
        {
            mSourceRectangle = sourceRectangle;
            mCurrentRectangle = sourceRectangle;
            mInterval = interval;
            mRows = rows;
            mColumns = columns;
        }

        public float Interval
        {
            get { return mInterval;}
            set { mInterval = value; }
        }

        public Rectangle SourceRectangle
        {
            get { return mSourceRectangle; }
            set { mSourceRectangle = value; }
        }

        public int Rows { get { return mRows; } }

        public int Columns { get { return mColumns; } }

        public float CurrentTime
        {
            get { return mCurrentTime;}
            set { mCurrentTime = value; }
        }

        public int CurrentRowIndex 
        { 
            get { return mCurrentRowIndex; } 
            set { mCurrentRowIndex = value; } 
        }

        public int CurrentColumnIndex
        {
            get { return mCurrentColumnIndex; }
            set { mCurrentColumnIndex = value; }
        }

        public Rectangle CurrentRectangle
        {
            get { return mCurrentRectangle; }
            set { mCurrentRectangle = value; }
        }

        public override void Update()
        {
        }
    }
}
