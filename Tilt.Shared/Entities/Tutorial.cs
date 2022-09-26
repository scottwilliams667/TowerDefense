using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;
using Tilt.Shared.Structures;

namespace Tilt.Shared.Entities
{
    public class Tutorial : UIElement
    {
        private NormalButton mContinueButton;
        private DialogueTextRenderer mTextRenderer;
        private TutorialRenderComponent mRenderComponent;
        private TextObject mWelcomeText;

        public Tutorial(int viewportWidth, int viewportHeight, string texturePath, bool register = true, string name = "") : base(viewportWidth, viewportHeight, register, name)
        {
            mContinueButton = new NormalButton(viewportWidth * 13/16, viewportHeight * 7/8, "continuebutton", 1, 1, () => { MenuManager.HideTutorial(); }, Tuner.SFXButtonClick);
            mRenderComponent = new TutorialRenderComponent(texturePath, this);
            mWelcomeText = new TextObject(viewportWidth / 15, viewportHeight / 10,StringOps.GetString("tutorial_text_welcome"), "TutorialFont");
        }

        public override void UnRegister()
        {
            mContinueButton.UnRegister();
            mRenderComponent.UnRegister();
            mWelcomeText.UnRegister();
            base.UnRegister();
        }
    }

    public class TutorialRenderComponent : UIRenderComponent
    {
        private Texture2D mBuildButtonTexture;
        private Texture2D mPistolTowerTexture;
        private Texture2D mSelectNewTexture;
        private Texture2D mPlayButtonTexture;
        private Texture2D mBuildAllTexture;
        private Texture2D mArrow;


        private string mTextBuild;
        private string mTextSelectObject;
        private string mTextSelectOther;
        private string mTextBuildAll;
        private string mTextPlay;

        private SpriteFont mTutorialFont;


        public TutorialRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mBuildButtonTexture = AssetOps.LoadAsset<Texture2D>("buildbutton");
            mPistolTowerTexture = AssetOps.LoadAsset<Texture2D>("pistoltowerbutton");
            mSelectNewTexture = AssetOps.LoadAsset<Texture2D>("selectnewbutton");
            mBuildAllTexture = AssetOps.LoadAsset<Texture2D>("buildallbutton");
            mPlayButtonTexture = AssetOps.LoadAsset<Texture2D>("playbutton");
            mArrow = AssetOps.LoadAsset<Texture2D>("tutorialarrow");
            mTutorialFont = AssetOps.LoadAsset<SpriteFont>("TutorialFont");

            mTextBuild = StringOps.GetString("tutorial_text_build");
            mTextSelectObject = StringOps.GetString("tutorial_text_select_object");
            mTextSelectOther = StringOps.GetString("tutorial_text_select_other");
            mTextBuildAll = StringOps.GetString("tutorial_text_build_all");
            mTextPlay = StringOps.GetString("tutorial_text_play");
    }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();
            Viewport viewport = graphicsDevice.Viewport;

            int viewportWidth = viewport.Width;
            int viewportHeight = viewport.Height;

            if(string.IsNullOrEmpty(mTextBuild))
                mTextBuild = StringOps.GetString("tutorial_text_build");

            if (string.IsNullOrEmpty(mTextSelectObject))
                mTextSelectObject = StringOps.GetString("tutorial_text_select_object");

            if (string.IsNullOrEmpty(mTextSelectOther))
                mTextSelectOther = StringOps.GetString("tutorial_text_select_other");

            if (string.IsNullOrEmpty(mTextBuildAll))
                mTextBuildAll = StringOps.GetString("tutorial_text_build_all");

            if (string.IsNullOrEmpty(mTextPlay))
                mTextPlay = StringOps.GetString("tutorial_text_play");

            //spriteBatch.DrawString(mTutorialFont, mTextWelcome, new Vector2(viewportWidth / 15, viewportHeight / 10), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mTexture, Vector2.Zero, null, Color.White, 0.0f,
                Vector2.Zero, 1.0f, SpriteEffects.None, 0.08f);

            spriteBatch.Draw(mBuildButtonTexture, new Vector2(viewportWidth / 15, viewportHeight / 2), null, Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.15f);

            spriteBatch.DrawString(mTutorialFont, mTextBuild, new Vector2(viewportWidth / 15 + mTutorialFont.MeasureString(mTextBuild).X / 4, viewportHeight * 65 / 100), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);


            spriteBatch.Draw(mArrow, new Vector2(viewportWidth * 18/100, viewportHeight / 2 + mBuildButtonTexture.Height / 4), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mPistolTowerTexture, new Vector2(viewportWidth  / 4, viewportHeight / 2), new Rectangle(mPistolTowerTexture.Width / 50, mPistolTowerTexture.Height / 20, mPistolTowerTexture.Width * 2/3, mPistolTowerTexture.Height - mPistolTowerTexture.Height / 20), Color.White, 0.0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.15f);

            spriteBatch.DrawString(mTutorialFont, mTextSelectObject, new Vector2(viewportWidth / 4, viewportHeight * 65 / 100), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mArrow, new Vector2(viewportWidth * 42 / 100, viewportHeight / 2 + mBuildButtonTexture.Height / 4), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mSelectNewTexture, new Vector2(viewportWidth * 3 / 6, viewportHeight / 2), new Rectangle(mSelectNewTexture.Width / 2, 0, mSelectNewTexture.Width / 2, mSelectNewTexture.Height), Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.15f);

            spriteBatch.DrawString(mTutorialFont, mTextSelectOther, new Vector2(viewportWidth * 3 / 6 - mTutorialFont.MeasureString(mTextSelectOther).X / 4, viewportHeight * 65 / 100), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mArrow, new Vector2(viewportWidth * 59 / 100, viewportHeight / 2 + mBuildButtonTexture.Height / 4), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mBuildAllTexture, new Vector2(viewportWidth * 4 / 6, viewportHeight / 2), new Rectangle(mBuildAllTexture.Width / 2, 0, mBuildAllTexture.Width / 2, mBuildAllTexture.Height), Color.White, 0.0f, Vector2.Zero, 0.77f, SpriteEffects.None, 0.15f);

            spriteBatch.DrawString(mTutorialFont, mTextBuildAll, new Vector2(viewportWidth * 4 / 6 - mTutorialFont.MeasureString(mTextSelectOther).X / 8, viewportHeight * 65 / 100), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mArrow, new Vector2(viewportWidth * 75 / 100, viewportHeight / 2 + mBuildButtonTexture.Height / 4), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.Draw(mPlayButtonTexture, new Vector2(viewportWidth * 5 / 6, viewportHeight / 2), new Rectangle(mPlayButtonTexture.Width / 2, 0, mPlayButtonTexture.Width / 2, mPlayButtonTexture.Height), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

            spriteBatch.DrawString(mTutorialFont, mTextPlay, new Vector2(viewportWidth * 5 / 6 - mTutorialFont.MeasureString(mTextSelectOther).X / 4, viewportHeight * 65 / 100), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);

        }
    }
}
