using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class NotificationWindow : UIElement
    {
        private Button mDismissButton;
        private NotificationWindowRenderComponent mRenderComponent;
        private EventTimerComponent mEventTimer;
        public NotificationWindow(string texturePath, string fontPath, int x, int y, int xDest, int yDest) : base(x, y)
        {
            PositionComponent = new NotificationWindowPositionComponent(x, y, xDest, yDest, this);
            mRenderComponent = new NotificationWindowRenderComponent(texturePath, fontPath, this);
            mEventTimer = new EventTimerComponent(2.0f, EventType.NotificationWindowClosed, this);
            mEventTimer.Stop();
        }

        public NotificationWindowRenderComponent RenderComponent
        {
            get { return mRenderComponent; }
            set { mRenderComponent = value; }
        }

        public EventTimerComponent TimerComponent
        {
            get { return mEventTimer; }
            set { mEventTimer = value; }
        }

        public Button DismissButton
        {
            get { return mDismissButton; }
            set { mDismissButton = value; }
        }
    }

    public class NotificationWindowPositionComponent : PositionComponent
    {
        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        private int mXStart;
        private int mYStart;
        private int mXDest;
        private int mYDest;
        private Vector2 mDirection = Vector2.Zero;
        private const int kSpeed = 750;

        public NotificationWindowPositionComponent(int x, int y, int xDest, int yDest, Entity owner) : base(x, y, owner)
        {
            mXDest = xDest;
            mYDest = yDest;
            mXStart = x;
            mYStart = y;
        }

        public override void Register()
        {
            EventSystem.SubScribe(EventType.NotificationWindowOpened, OnOpened_);
            EventSystem.SubScribe(EventType.NotificationWindowClosed, OnClosed_);
            base.Register();
        }

        public override void UnRegister()
        {
            EventSystem.UnSubScribe(EventType.NotificationWindowOpened, OnOpened_);
            EventSystem.UnSubScribe(EventType.NotificationWindowClosed, OnClosed_);
            base.UnRegister();
        }

        public int XDest
        {
            get { return mXDest;}
            set
            {
                mXDest = value;
            }
        }

        public int YDest
        {
            get { return mYDest; }
            set { mYDest = value; }
        }

        public bool IsSlidingIn
        {
            get { return mIsSlidingIn; }
            set 
            { 
                mIsSlidingIn = value;
                mDirection = new Vector2(0, -1);
            }
        }
        
        public bool IsSlidingOut
        {
            get { return mIsSlidingOut; }
            set 
            {
                mIsSlidingOut = value;
                mDirection = new Vector2(0, 1);
            }
        }

        private void OnOpened_(object sender, IGameEventArgs e)
        {
            NotificationArgs notification = e as NotificationArgs;
            if (notification == null)
                return;

            NotificationWindow window = Owner as NotificationWindow;
            window.RenderComponent.Text = notification.Text;
            window.TimerComponent.Reset();

            if (Y > mYDest)
            {
                IsSlidingIn = true;
                window.TimerComponent.Start();    
            }

        }

        private void OnClosed_(object sender, IGameEventArgs e)
        {
            NotificationWindow window = Owner as NotificationWindow;

            if (Y < mYStart)
            {
                IsSlidingOut = true;
                window.TimerComponent.Reset();
                window.TimerComponent.Stop();
            }
        }

        public override void Update()
        {
            NotificationWindow window = Owner as NotificationWindow;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if(mIsSlidingIn && Y > mYDest)
            {
                Position += mDirection * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                window.DismissButton.PositionComponent.Position += mDirection * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                window.DismissButton.TouchComponent.Bounds = new Rectangle((int)window.DismissButton.PositionComponent.Position.X,
                                                                           (int)window.DismissButton.PositionComponent.Position.Y,
                                                                           window.DismissButton.TouchComponent.Bounds.Width,
                                                                           window.DismissButton.TouchComponent.Bounds.Height);
            }

            if(mIsSlidingOut && Y < mYStart)
            {
                Position += mDirection * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                window.DismissButton.PositionComponent.Position += mDirection * kSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                window.DismissButton.TouchComponent.Bounds = new Rectangle((int)window.DismissButton.PositionComponent.Position.X,
                                                                           (int)window.DismissButton.PositionComponent.Position.Y,
                                                                           window.DismissButton.TouchComponent.Bounds.Width,
                                                                           window.DismissButton.TouchComponent.Bounds.Height);
            }

            
            

            if (mIsSlidingIn && Y < mYDest)
            {
                mIsSlidingIn = false;
                mDirection = Vector2.Zero;
            }
            if (mIsSlidingOut && Y > mYStart)
            {
                mIsSlidingOut = false;
                mDirection = Vector2.Zero;
            }
        }
    }

    public class NotificationWindowRenderComponent : UIRenderComponent
    {
        private string mNotification =  string.Empty;
        private SpriteFont mFont;

        public NotificationWindowRenderComponent(string texturePath, string fontPath, Entity owner) : base(texturePath, owner)
        {
            mFont = AssetOps.LoadAsset<SpriteFont>(fontPath);
        }

        public string Text
        {
            get { return mNotification; }
            set { mNotification = value; }
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            NotificationWindow window = Owner as NotificationWindow;
            NotificationWindowPositionComponent positionComponent = window.PositionComponent as NotificationWindowPositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.14f);
            spriteBatch.DrawString(mFont, mNotification, new Vector2(positionComponent.Position.X + 12, positionComponent.Position.Y + 5), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.15f);
        }
    }

}
