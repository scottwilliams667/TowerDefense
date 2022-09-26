using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Entities
{
    public class UIElement : Entity
    {
        private string mName;
        private PositionComponent mPositionComponent;
        private UIElement mParent;

        public UIElement(int x, int y, bool register = true, string name = default(string))
        {
            mName = name;
        }

        public override void UnRegister()
        {
            mParent = null;
            base.UnRegister();
        }

        public PositionComponent PositionComponent 
        { 
            get { return mPositionComponent; } 
            set { mPositionComponent = value;} 
        }

        public UIElement Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public string Name
        {
            get { return mName; }
            protected set { mName = value; }
        }
    }

    public class Button : UIElement
    {
        private ActionComponent mActionComponent;
        private ButtonAnimationComponent mAnimationComponent;
        private ButtonTouchComponent mButtonTouchComponent;
        private AudioComponent mAudioComponent;
        private UIElement mChild;

        public Button(int x, int y, bool register = true, string name = default(string)) : base(x,y, register, name)
        {
            PositionComponent = new PositionComponent(x, y, this);
        }

        public override void UnRegister()
        {
            
            PositionComponent.UnRegister();
            mChild = null;
            base.UnRegister();
        }

        public ActionComponent ActionComponent
        {
            get { return mActionComponent; }
            set { mActionComponent = value; }
        }

        public ButtonTouchComponent TouchComponent
        {
            get { return mButtonTouchComponent; }
            set { mButtonTouchComponent = value; }
        }

        public ButtonAnimationComponent AnimationComponent
        {
            get { return mAnimationComponent; }
            set { mAnimationComponent = value; }
        }

        public AudioComponent AudioComponent
        {
            get { return mAudioComponent; }
            set { mAudioComponent = value; }
        }

        public UIElement Child
        {
            get { return mChild; }
            set { mChild = value; }
        }
    }

    public class MenuButton : Button
    {
        public MenuButton(int x, int y, Action action, string name = default(string))
            : base(x, y, true, name)
        {
            ActionComponent = new MenuActionComponent(action, this);
        }

        public override void UnRegister()
        {
            ActionComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class MenuArgButton : Button
    {
        public MenuArgButton(int x, int y, string texturePath, Rectangle sourceRectangle, int rows, int columns,
            Action<object> action, object obj, string soundEffect, bool register = true, string name = null)
            : base(x, y, register, name)
        {
            ActionComponent = new MenuActionArgComponent(action, obj, this);
            AnimationComponent = new ButtonAnimationComponent(texturePath, sourceRectangle, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x,y, AnimationComponent.Texture.Width / 2, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public MenuArgButton(int x, int y, string texturePath, int rows, int columns,
            Action<object> action, object obj, string soundEffect, bool register = true, string name = null)
            : base(x, y, register, name)
        {
            ActionComponent = new MenuActionArgComponent(action, obj, this);
            AnimationComponent = new ButtonAnimationComponent(texturePath, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width , AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public MenuArgButton(int x, int y, string texturePath, Rectangle sourceRectangle, int rows, int columns, 
            Action<object,object> action, object obj1, object obj2, string soundEffect, bool register = true, string name = null)
            : base(x, y, register, name)
        {
            ActionComponent = new MenuActionArgComponent(action, obj1, obj2, this);
            AnimationComponent = new ButtonAnimationComponent(texturePath, sourceRectangle, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width / 2, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            TouchComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class TowerSelectButton : Button
    {
        public TowerSelectButton(int x, int y, string texturePath, int rows, int columns,
            Action<object> action, object obj, string soundEffect, bool register = true, string name = null)
            : base(x, y, register, name)
        {
            ActionComponent = new MenuActionArgComponent(action, obj, this);
            AnimationComponent = new TowerSelectButtonAnimationComponent(texturePath, 0.0f, rows, columns, this, (ObjectType)obj);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public TowerSelectButton(int x, int y, string texturePath, int rows, int columns,
            Action<object, object> action, object obj1, object obj2, string soundEffect, bool register = true, string name = null)
            : base(x, y, register, name)
        {
            ActionComponent = new MenuActionArgComponent(action, obj1, obj2, this);
            if (obj2 is AddOnType)
            {
                AnimationComponent = new TowerSelectButtonAnimationComponent(texturePath, 0.0f, rows, columns, this, (ObjectType)obj1, addOnType:(AddOnType)obj2);
            }
            else
            {
                AnimationComponent = new TowerSelectButtonAnimationComponent(texturePath, 0.0f, rows, columns, this, (ObjectType)obj1, towerType: (TowerType)obj2);

            }
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }
    }

    public class PauseButton : Button
    {
        public PauseButton(int x, int y, string texturePath, int rows, int columns, Action action, string soundEffect, string name = default(string))
            : base(x, y, true, name)
        {
            ActionComponent = new MenuActionComponent(action, this);
            AnimationComponent = new PauseButtonRenderComponent(texturePath, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width / 3, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            TouchComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class SoundFXButton : Button
    {
        public SoundFXButton(int x, int y, string texturePath, int rows, int columns, Action action, string soundEffect, string name = null)
            : base(x, y, true, name)
        {
            ActionComponent = new MenuActionComponent(action, this);
            AnimationComponent = new SoundFXButtonRenderComponent(texturePath, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.SourceRectangle.Width, AnimationComponent.SourceRectangle.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }
    }

    public class MusicButton : Button
    {
        public MusicButton(int x, int y, string texturePath, int rows, int columns, Action action, string soundEffect, string name = null)
            : base(x, y, true, name)
        {
            ActionComponent = new MenuActionComponent(action, this);
            AnimationComponent = new MusicButtonRenderComponent(texturePath, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.SourceRectangle.Width, AnimationComponent.SourceRectangle.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }
    }

    public class NormalButton : MenuButton
    {
        public NormalButton(int x, int y, string texturePath, Rectangle sourceRectangle, int rows, int columns, Action action, string soundEffect, string name = null) : base(x, y, action, name)
        {
            AnimationComponent = new ButtonAnimationComponent(texturePath, sourceRectangle, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y,sourceRectangle.Width, sourceRectangle.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public NormalButton(int x, int y, string texturePath, int rows, int columns, Action action, string soundEffect, string name = null)
            : base(x, y, action, name)
        {
            AnimationComponent = new ButtonAnimationComponent(texturePath, 0.0f, rows, columns, this);
            TouchComponent = new ButtonTouchComponent(new Rectangle(x, y, AnimationComponent.Texture.Width, AnimationComponent.Texture.Height), this);
            AudioComponent = new SimpleAudioComponent(soundEffect, this);
        }

        public override void UnRegister()
        {
            AnimationComponent.UnRegister();
            TouchComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class TextObject : UIElement
    {
        protected TextRenderComponent mTextRenderComponent;

        public TextObject(int x, int y, string text, string font, bool register = true, string name = default(string)) : base(x,y, register, name)
        {
            PositionComponent = new PositionComponent(x, y, this);
            mTextRenderComponent = new TextRenderComponent(text, font, this);
        }

        public TextRenderComponent TextRenderComponent
        {
            get { return mTextRenderComponent; }
            set { mTextRenderComponent = value; }
        }

        public override void UnRegister()
        {
            mTextRenderComponent.UnRegister();
            base.UnRegister();
        }
    }
}
