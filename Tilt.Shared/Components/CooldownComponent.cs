using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Components
{
    public class CooldownComponent : TimerComponent
    {
        public CooldownComponent(float timeSet, Entity owner, bool register = true) : base(owner, register)
        {
            mTimeSet = timeSet;
            mTimeLeft = timeSet;
            
            Stop_();
        }

        public bool IsCooling
        {
            get { return mTimeLeft < mTimeSet; }
        }

        public float TimeSet
        {
            get { return mTimeSet; }
            set { mTimeSet = value; }
        }

        public float TimeLeft
        {
            get { return mTimeLeft; }
            set { mTimeLeft = value; }
        }

        public void Cooldown()
        {
            Start_();

            EventSystem.EnqueueEvent(EventType.SoundEffect, null, new SoundEffectArgs()
            {
                Play = true,
                SoundEffect = "sfx_tower_powerdown"
            });

        }

        protected override void Done_()
        {
            Reset_();
            Stop_();
        }
    }

    public class CooldownRenderComponent : RenderComponent
    {
        private Texture2D mShadow;
        private Rectangle mSourceRectangle;
        private int mSourceRectangleWidth;
        private int mSourceRectangleHeight;

        public CooldownRenderComponent(string texturePath, Rectangle sourceRectangle, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mSourceRectangle = sourceRectangle;
            mSourceRectangleWidth = sourceRectangle.Width;
            mSourceRectangleHeight = sourceRectangle.Height;
        }

        public override void Update()
        {
            Tower tower = Owner as Tower;
            
            if (tower == null)
                return;

            CooldownComponent cooldownComponent = tower.CooldownComponent;
            PositionComponent positionComponent = tower.PositionComponent;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            
            if (!cooldownComponent.IsCooling || positionComponent == null)
                return;

            mSourceRectangleHeight = mSourceRectangle.Height;

            float percentage = (cooldownComponent.IsCooling) ? (cooldownComponent.TimeLeft) / cooldownComponent.TimeSet : 1.0f;

            
            spriteBatch.Draw(mTexture, new Vector2(positionComponent.Position.X, positionComponent.Position.Y + TileMap.TileWidth),
                new Rectangle(mSourceRectangle.X, mSourceRectangle.Y, mSourceRectangle.Width, (int)(mSourceRectangleHeight * percentage)), 
                Color.Black * 0.5f, 0.0f, new Vector2(0, 32), new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.36f);

        }
    }
}
