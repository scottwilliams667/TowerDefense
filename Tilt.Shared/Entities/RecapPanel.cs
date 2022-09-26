using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public abstract class RecapPanel : UIElement
    {
        public RecapPanel(string texturePath, int x, int y, bool register = true, string name = null) : base(x, y, register, name)
        {
            PositionComponent = new PositionComponent(x,y,this);
            //mRenderComponent = new PanelRenderComponent(texturePath, this);
        }

        public abstract PanelRenderComponent RenderComponent { get; }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            
            base.UnRegister();
        }
    }

    public class VictoryRecapPanel : RecapPanel
    {
        private VictoryRecapPanelRenderComponent mRenderComponent;
        public VictoryRecapPanel(string texturePath, int x, int y, bool register = true, string name = null)
            : base(texturePath, x, y, register, name)
        {
            mRenderComponent = new VictoryRecapPanelRenderComponent(texturePath, this);
        }

        public override PanelRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
        }
    }

    public class DefeatedRecapPanel : RecapPanel
    {
        private DefeatedRecapPanelRenderComponent mRenderComponent;

        public DefeatedRecapPanel(string texturePath, int x, int y, bool register = true, string name = null) : base(texturePath, x, y, register, name)
        {
            mRenderComponent = new DefeatedRecapPanelRenderComponent(texturePath, this);
        }

        public override PanelRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
        }
    }

    public class PanelRenderComponent : UIRenderComponent
    {

        private Texture2D mOverlay;
        public PanelRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mOverlay = AssetOps.LoadAsset<Texture2D>("victoryoverlaypanel");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            RecapPanel recapPanel = Owner as RecapPanel;

            spriteBatch.Draw(mOverlay, Vector2.Zero, null,  Color.White, 0.0f,
                Vector2.Zero, 1.0f, SpriteEffects.None, 0.08f);
        

            spriteBatch.Draw(mTexture, recapPanel.PositionComponent.Position, null,  Color.White, 0.0f,
                Vector2.Zero, 1.0f, SpriteEffects.None, 0.10f);
        }
    }

    public class VictoryRecapPanelRenderComponent : PanelRenderComponent
    {
        private SpriteFont mFont;
        public VictoryRecapPanelRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
            
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            RecapPanel recapPanel = Owner as RecapPanel;
            PositionComponent positionComponent = recapPanel.PositionComponent;

            spriteBatch.DrawString(mFont, "TIME", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height / 3),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, "RESOURCE SPENT", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 40 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, "ENEMIES KILLED", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 47 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;

            string timeElapsed = "00:00";
            if (infoBar != null)
            {
                TimeSpan timeSpan = infoBar.GetTimeElapsed();
                DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                timeElapsed = dateTime.ToString("HH:mm");
            }

            spriteBatch.DrawString(mFont, timeElapsed, new Vector2(positionComponent.Position.X + (mTexture.Width * 86/100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height / 3),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, Resources.UnitsDestroyedOverLevel.ToString(), new Vector2(positionComponent.Position.X + (mTexture.Width * 86 / 100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 47 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            // resources spent
            spriteBatch.DrawString(mFont, Resources.ResourcesSpentOverLevel.ToString(), new Vector2(positionComponent.Position.X + (mTexture.Width * 86 / 100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 40 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);
            
            //add enemy kill later

            base.Update();
        }
    }

    public class DefeatedRecapPanelRenderComponent : PanelRenderComponent
    {
        private SpriteFont mFont;
        public DefeatedRecapPanelRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            RecapPanel recapPanel = Owner as RecapPanel;
            PositionComponent positionComponent = recapPanel.PositionComponent;

            spriteBatch.DrawString(mFont, "TIME", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height / 3),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, "TOTAL RESOURCE SPENT", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 40 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, "TOTAL ENEMIES KILLED", new Vector2(positionComponent.Position.X + mTexture.Width / 32, positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 47 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            InfoBar infoBar = UIOps.FindElementByName("InfoBar") as InfoBar;

            string timeElapsed = "00:00";
            if (infoBar != null)
            {
                TimeSpan timeSpan = infoBar.GetTimeElapsed();
                DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                timeElapsed = dateTime.ToString("HH:mm");
            }

            spriteBatch.DrawString(mFont, timeElapsed, new Vector2(positionComponent.Position.X + (mTexture.Width * 86 / 100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height / 3),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, Resources.ResourcesSpentOverCampaign.ToString(), new Vector2(positionComponent.Position.X + (mTexture.Width * 86 / 100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 40 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

            spriteBatch.DrawString(mFont, Resources.UnitsDestroyedOverCampaign.ToString(), new Vector2(positionComponent.Position.X + (mTexture.Width * 86 / 100), positionComponent.Position.Y - mTexture.Width / 6 + mTexture.Height * 47 / 100),
                Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);



            base.Update();
        }
    }
}
