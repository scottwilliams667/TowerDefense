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
using Tilt.Shared.Structures;
using Tilt.Shared.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class DialogueTextRenderer : Entity
    {
        private PositionComponent mPositionComponent;
        private TextRendererComponent mTextRendererComponent;
        private DialogueTextTouchComponent mTouchComponent;
        private LoopingAudioComponent mAudioComponent;

        public DialogueTextRenderer(int x, int y, string font, string text)
        {
            mTextRendererComponent = new TextRendererComponent(text, font, this);
            mPositionComponent = new PositionComponent(x,y, this);

            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();

            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;

            mTouchComponent = new DialogueTextTouchComponent(
                new Rectangle(0,0, viewportWidth, viewportHeight), this );

            mAudioComponent = new LoopingAudioComponent(Tuner.SFXUIDialogueText, this);
        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            TextRenderComponent.UnRegister();
            AudioComponent.UnRegister();
            TouchComponent.UnRegister();
            base.UnRegister();
        }

        public PositionComponent PositionComponent
        {
            get { return mPositionComponent;}
            set { mPositionComponent = value; }
        }

        public TextRendererComponent TextRenderComponent
        {
            get { return mTextRendererComponent; }
            set { mTextRendererComponent = value; }
        }

        public LoopingAudioComponent AudioComponent
        {
            get { return mAudioComponent; }
            set { mAudioComponent = value; }
        }

        public DialogueTextTouchComponent TouchComponent
        {
            get { return mTouchComponent; }
            set { mTouchComponent = value; }
        }
    }

    public class TextRendererComponent : Component
    {
        private float mInterval;
        private float mTimeLeft;

        private string mText;

        private int mIndex = 1;

        private bool mIsWritingText = true;

        private LayerType mRegisteredLayer;

        private SpriteFont mFont;
        public TextRendererComponent(string text, string font, Entity owner, bool register = true) : base(owner, register)
        {
            mText = text;
            mFont = AssetOps.LoadAsset<SpriteFont>(font);
            mInterval = 0.03f;
            mTimeLeft = 0.03f;
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
        }

        public void WriteAllText()
        {
            mIndex = mText.Length - 1;
            mIsWritingText = false;
        }

        public void ResetText()
        {
            mIndex = 1;
            mIsWritingText = true;

            DialogueTextRenderer textRenderer = Owner as DialogueTextRenderer;
            LoopingAudioComponent audioComponent = textRenderer.AudioComponent;
            audioComponent.Play();

        }

        public void SetText(string text)
        {
            ResetText();
            mText = StringOps.GetString(text);
        }

        public override void Update()
        {
            DialogueTextRenderer renderer = Owner as DialogueTextRenderer;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            PositionComponent positionComponent = renderer.PositionComponent;
            LoopingAudioComponent audioComponent = renderer.AudioComponent;

            string[] splitString = mText.Split(new [] {"\\n" }, StringSplitOptions.None);

            Vector2 position = positionComponent.Position;

            int index = mIndex;

            foreach (string str in splitString)
            {
                //if we want to move the cursor down and leave an empty line
                // measure a place holder string and move the cursor down by Y units
                if (str == string.Empty)
                {
                    Vector2 measure = mFont.MeasureString("X");
                    position.Y += measure.Y;
                }

                if (index > str.Length)
                {
                    spriteBatch.DrawString(mFont, str, position, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                    Vector2 measure = mFont.MeasureString(str);
                    position.Y += measure.Y;
                    index -= str.Length;
                }
                
                else
                {
                    string text = str.Substring(0, index);
                    spriteBatch.DrawString(mFont, text, position, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                    break;    
                }
            }

            if (mIsWritingText && !audioComponent.IsPlaying)
            {
                audioComponent.Play();
            }

            if (mIndex >= splitString.Sum(s => s.Length))
            {
                mIsWritingText = false;
                audioComponent.Stop();
            }

            if (!mIsWritingText)
                return;


            mTimeLeft -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (mTimeLeft <= 0.0f)
            {
                mTimeLeft = mInterval;
                mIndex++;
            }
        }

       
    }

    public class DialogueTextTouchComponent : TouchAreaComponent
    {
        public DialogueTextTouchComponent(Rectangle bounds, Entity owner, bool register = true)
            : base(bounds, owner, register)
        {
        }

        public override void Update()
        {
#if WINDOWS
            if(MouseOps.ContainsPoint(mBounds) && MouseOps.IsClick())
#else
            if (TouchOps.ContainsPoint(mBounds) && TouchOps.IsTap())
#endif
            {
                DialogueTextRenderer textRenderer = Owner as DialogueTextRenderer;

                TextRendererComponent textComponent = textRenderer.TextRenderComponent;
                textComponent.WriteAllText();
            }
        }
    }
}
