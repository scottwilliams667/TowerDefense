using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Resources = Tilt.EntityComponent.Structures.Resources;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Components
{
    public class TextRenderComponent : Component
    {
        protected string mText;
        private SpriteFont mFont;
        private bool mIsVisible = true;
        private LayerType mRegisteredLayer;

        public TextRenderComponent(string text, string font, Entity owner) : base(owner)
        {
            mText = text;
            mFont = AssetOps.LoadAsset<SpriteFont>(font);
        }

        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; }
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

        public override void Update()
        {
            if (!mIsVisible)
                return;

            string[] splitString = mText.Split(new[] { "\\n" }, StringSplitOptions.None);

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            TextObject textObject = Owner as TextObject;
            PositionComponent positionComponent = textObject.PositionComponent;
            Vector2 position = positionComponent.Position;

            foreach (string str in splitString)
            {
                if (str == string.Empty)
                {
                    Vector2 measure = mFont.MeasureString("X");
                    position.Y += measure.Y;
                }
                spriteBatch.DrawString(mFont, str, position, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);
            }
        }
    }
    
}
