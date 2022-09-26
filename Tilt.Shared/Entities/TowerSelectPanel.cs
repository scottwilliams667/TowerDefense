using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Entities
{
    public class TowerSelectPanel : UIElement
    {
        public TowerSelectPanel(int x, int y, int xDest, int yDest, string texturePath, bool register = true, string name = null):  base(x, y, register, name)
        {
            
            RenderComponent = new TowerSelectPanelRenderComponent(texturePath, this);

            PanelState = new PanelState()
            {
                Elements = new List<UIElement>()
                {
                    //new MenuArgButton(x,0, "bullettowerbuttonblack", 1, 1, MenuManager.SelectBuildObject, ObjectType.Barricade),
                    //new MenuArgButton(x + 150,0, "bullettowerbuttonwhite",  1, 1, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Bullet),
                    //new MenuArgButton(x + 300,0, "bullettowerbutton",  1, 1, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Heavy),
                    //new MenuArgButton(x + 450,0, "menubuttonanimation", new Rectangle(0, 0, 90, 90), 1, 2, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Shotgun),
                    //new MenuArgButton(x + 600, 0, "menubuttonanimation", new Rectangle(0,0,90,90), 1, 2, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Laser),

                    //new MenuArgButton(x, 150, "shotguntowerbuttonblack",  1, 1, MenuManager.SelectBuildObject, ObjectType.Refinery),
                    //new MenuArgButton(x + 150,150, "shotguntowerbuttonwhite", 1, 1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.Refinery),
                    //new MenuArgButton(x + 300, 150, "shotguntowerbuttonsimple", 1, 1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.Cooldown),
                    //new MenuArgButton(x + 450, 150, "shotguntowerbutton", 1, 1, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.AmmoCapacity),
                    //new MenuArgButton(x + 600, 150, "menubuttonanimation", new Rectangle(0,0,90,90), 1, 2, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Nuclear),

                    //new MenuArgButton(x, 300, "menubuttonanimation", new Rectangle(0,0,90,90), 1, 2, MenuManager.SelectBuildObject, ObjectType.Tower, TowerType.Rocket),
                    //new MenuArgButton(x + 150, 300, "menubuttonanimation", new Rectangle(0,0,90,90), 1, 2, MenuManager.SelectBuildObject, ObjectType.AddOn, AddOnType.RangeBooster),
                    //new MenuArgButton(x + 300, 300, "menubuttonanimation", new Rectangle(0,0,90,90), 1, 2, MenuManager.SelectBuildObject, ObjectType.ShieldGenerator)
                }
            };

            PositionComponent = new TowerSelectPanelPositionComponent(x, y, xDest, yDest, this);
            
        }

        public PanelState PanelState { get; set; }

        public UIRenderComponent RenderComponent { get; set; }

        public void Reset()
        {
            TowerSelectPanelPositionComponent positionComponent = PositionComponent as TowerSelectPanelPositionComponent;
            positionComponent.Reset();
        }
    }

    public class TowerSelectPanelRenderComponent : UIRenderComponent
    {
        public TowerSelectPanelRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            TowerSelectPanel towerSelectPanel = Owner as TowerSelectPanel; ;
            PositionComponent positionComponent = towerSelectPanel.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.1f);
        }

    }

    public class TowerSelectPanelPositionComponent : PositionComponent
    {
        private Vector2 mDirection = Vector2.Zero;
        private Vector2 mOriginalPosition = Vector2.Zero;
        private Vector2 mDestination = Vector2.Zero;
        // speed has to be 2x the left panel otherwise there will be weird render bugs when the tower select layer is removed
        private int mSpeed = 2240;
        private const int kInitialSpeed = 2240;
        private float mSpeedScale = 0.925f;

        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        public TowerSelectPanelPositionComponent(int x, int y, int xDest, int yDest, Entity owner, Vector2 origin = new Vector2()) : base(x, y, owner, origin)
        {
            mOriginalPosition = Position;
            mDestination = new Vector2(xDest, yDest);
            mDirection = new Vector2(xDest - X, yDest - Y);
            mDirection.Normalize();
        }

        public bool IsSlidingIn
        {
            get { return mIsSlidingIn; }
            set
            {
                mIsSlidingIn = value;
                mDirection = new Vector2(mDestination.X - X, mDestination.Y - Y);
                mDirection.Normalize();

                if(float.IsNaN(mDirection.X) && float.IsNaN(mDirection.Y))
                {
                    mDirection = Vector2.Zero;
                    mIsSlidingIn = false;
                }
            }
        }

        public bool IsSlidingOut
        {
            get { return mIsSlidingOut;}
            set
            {
                mIsSlidingOut = value;
                mDirection = new Vector2(mOriginalPosition.X - X, mOriginalPosition.Y - Y);
                mDirection.Normalize();

                if(float.IsNaN(mDirection.X) && float.IsNaN(mDirection.Y))
                {
                    mDirection = Vector2.Zero;
                    mIsSlidingOut = false;
                }
            }
        }

        public void Reset()
        {
            TowerSelectPanel towerSelectPanel = Owner as TowerSelectPanel;

            mIsSlidingIn = false;
            mIsSlidingOut = false;

            UIOps.ResetPanelStatePositions(towerSelectPanel.PanelState, mPosition, mOriginalPosition);
            mPosition = mOriginalPosition;

        }

        public override void Update()
        {
            TowerSelectPanel panel = Owner as TowerSelectPanel;
            PanelState panelState = panel.PanelState;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if (mIsSlidingIn)
            {

                if (mDestination.X - mPosition.X < 400)
                    mSpeed = mSpeed > 300 ? (int)(mSpeed * mSpeedScale) : mSpeed;



                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X > mDestination.X)
                    xOffset.X = mDestination.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in panelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                    }
                }
            }

            if (mIsSlidingOut)
            {
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X < mOriginalPosition.X)
                    xOffset.X = mOriginalPosition.X - mPosition.X;

                mPosition += xOffset;

                foreach (UIElement element in panelState.Elements)
                {
                    PositionComponent positionComponent = element.PositionComponent;
                    positionComponent.Position += xOffset;

                    if (element is Button)
                    {
                        Button button = element as Button;
                        button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                            button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                    }
                }
            }

            if (mPosition == mDestination && mIsSlidingIn)
            {
                mIsSlidingIn = false;
                mSpeed = kInitialSpeed;
            }
            if (mPosition == mOriginalPosition && mIsSlidingOut)
            {
                mIsSlidingOut = false;
            }

        }
    }
}
