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
using Tilt.Shared.Entities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Entities
{

    public enum PanelAction
    {
        None,
        TowerSelecting,
        TowerSelected,
        TowerPlaced,
        PauseMenu,
        InfoPanel
    }

    public class PanelState
    {
        private PanelAction mAction;
        private List<UIElement> mElements = new List<UIElement>(); 

        public PanelState(PanelAction action)
        {
            mAction = action;
        }

        public PanelState()
        {
        }


        public PanelAction Action
        {
            get { return mAction; }
        }

        public List<UIElement> Elements
        {
            get { return mElements; }
            set { mElements = value; }
        }
    }

    public class SlidingPanel : UIElement
    {
        private PanelAction mPanelAction;
        private SlidingPanelRenderComponent mRenderComponent;
        private SlidingPanelPositionComponent mPositionComponent;

        private Button mUndoButton;
        private Button mSelectNewButton;
        private Button mBuildAllButton;
        private Button mCancelButton;

        public SlidingPanel(int x, int y, int xDest, int yDest, string texturePath, PanelAction panelAction, bool register = true, string name = null) : base(x, y, register, name)
        {

            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();
            //load a button texture so we can get dimensions from it
            Texture2D slidingPanelButtonTexture = AssetOps.LoadAsset<Texture2D>("selectnewbutton");

            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;
            int buttonXOffset = viewportWidth / 12;
            


            mSelectNewButton = new NormalButton(0, viewportHeight * 62 / 100, "selectnewbutton",
                new Rectangle(0, 0, 0, 0), 1, 2, MenuManager.SelectNewTower,
                Tuner.SFXButtonClick,
                "BuildPanelSelectNewButton");
            mUndoButton = new NormalButton(0, viewportHeight * 62 / 100, "undobutton",
                new Rectangle(0, 0, 0, 0), 1, 2, MenuManager.UndoTower,
                Tuner.SFXUndo,
                "BuildPanelUndoButton");
            mBuildAllButton = new NormalButton(0, viewportHeight * 4 / 5, "buildallbutton",
                new Rectangle(0, 0, 0, 0), 1, 2, MenuManager.ConfirmTowers,
                Tuner.SFXButtonClick,
                "BuildPanelConfirmButton");
            mCancelButton = new NormalButton(0, viewportHeight * 4 / 5, "cancelbutton",
                new Rectangle(0, 0, 0, 0), 1, 2, MenuManager.BuildPanelBack,
                Tuner.SFXCancel,
                "BuildPanelBackButton");

            mSelectNewButton.PositionComponent.X = viewportWidth + buttonXOffset;
            mSelectNewButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mSelectNewButton.AnimationComponent.Texture.Width / 2, mSelectNewButton.AnimationComponent.Texture.Height);
            mSelectNewButton.TouchComponent.Bounds = new Rectangle((int)mSelectNewButton.PositionComponent.X, (int)mSelectNewButton.PositionComponent.Y, mSelectNewButton.AnimationComponent.SourceRectangle.Width, mSelectNewButton.AnimationComponent.SourceRectangle.Height);

            mUndoButton.PositionComponent.X = viewportWidth + 2*buttonXOffset + mSelectNewButton.AnimationComponent.SourceRectangle.Width / 2;
            mUndoButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mUndoButton.AnimationComponent.Texture.Width/2, mUndoButton.AnimationComponent.Texture.Height);
            mUndoButton.TouchComponent.Bounds = new Rectangle((int)mUndoButton.PositionComponent.X, (int)mUndoButton.PositionComponent.Y, mUndoButton.AnimationComponent.SourceRectangle.Width, mUndoButton.AnimationComponent.SourceRectangle.Height);

            mBuildAllButton.PositionComponent.X = viewportWidth + buttonXOffset;
            mBuildAllButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mBuildAllButton.AnimationComponent.Texture.Width / 2, mBuildAllButton.AnimationComponent.Texture.Height);
            mBuildAllButton.TouchComponent.Bounds = new Rectangle((int)mBuildAllButton.PositionComponent.X, (int)mBuildAllButton.PositionComponent.Y, mBuildAllButton.AnimationComponent.SourceRectangle.Width, mBuildAllButton.AnimationComponent.SourceRectangle.Height);

            mCancelButton.PositionComponent.X = viewportWidth + 2 * buttonXOffset + mBuildAllButton.AnimationComponent.SourceRectangle.Width / 2;
            mCancelButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mCancelButton.AnimationComponent.Texture.Width / 2, mCancelButton.AnimationComponent.Texture.Height);
            mCancelButton.TouchComponent.Bounds = new Rectangle((int)mCancelButton.PositionComponent.X, (int)mCancelButton.PositionComponent.Y, mSelectNewButton.AnimationComponent.SourceRectangle.Width, mCancelButton.AnimationComponent.SourceRectangle.Height);

            mPanelAction = panelAction;

            PositionComponent = new SlidingPanelPositionComponent(x,y, xDest, yDest, this);
            
            mRenderComponent = new SlidingPanelRenderComponent(texturePath, this);
            mRenderComponent.UpdatePanelStates();

            
            
        }

        public SlidingPanelRenderComponent RenderComponent
        {
            get {return mRenderComponent; }
            set { mRenderComponent = value; }
        }

        public PanelAction PanelAction 
        { 
            get { return mPanelAction; }
            set { mPanelAction = value; }
        }

        public Button UndoButton
        {
            get { return mUndoButton; }
        }

        public Button SelectNewButton
        {
            get { return mSelectNewButton; }
        }

        public Button BuildAllButton
        {
            get { return mBuildAllButton; }
        }

        public Button CancelButton
        {
            get { return mCancelButton; }
        }

        public List<Button> Buttons
        {
            get { return new List<Button>() {UndoButton,SelectNewButton,BuildAllButton,CancelButton}; }
        }

        public void Reset()
        {
            PanelAction = PanelAction.TowerSelecting;
            RenderComponent.UpdatePanelStates();
            SlidingPanelPositionComponent positionComponent = PositionComponent as SlidingPanelPositionComponent;
            positionComponent.Reset();
        }
    }

    public class SlidingPanelRenderComponent : UIRenderComponent
    {
        private Texture2D mSelectingPanelTexture;
        private SpriteFont mDebugFont;
        private SpriteFont mTowerNameFont;
        private Texture2D mDescriptionPanelTexture;
        private Texture2D mNamePanelTexture;
        private Texture2D mPriceBar;

        public SlidingPanelRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mDebugFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
            mTowerNameFont = AssetOps.LoadAsset<SpriteFont>("TowerNameFont");
            mDescriptionPanelTexture = AssetOps.LoadAsset<Texture2D>("descriptionpanel");
            mNamePanelTexture = AssetOps.LoadAsset<Texture2D>("namepanel");
            mPriceBar = AssetOps.LoadAsset<Texture2D>("pricebar");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            SlidingPanel slidingPanel = Owner as SlidingPanel;
            PositionComponent positionComponent = slidingPanel.PositionComponent;

            Vector2 position = positionComponent.Position;

            UpdatePanelStates();
            
            Texture2D selectedIcon = UIOps.GetIconSource();
            Texture2D selectedInfoText = UIOps.GetTextSource();


            float iconPositionX = position.X + mTexture.Width / 16;
            float iconPositionY = position.Y + mTexture.Height / 4;

            float textPositionX = position.X + ((mTexture.Width * 13) / 32);
            float textPositionY = position.Y + mTexture.Height / 5;

            float descriptionPositionX = position.X;
            float descriptionPositionY = mTexture.Height / 2;

            float pricePositionX = 0.0f;
            float pricePositionY = 0.0f;

            if (selectedIcon != null)
            {
                pricePositionX = position.X + mTexture.Width / 16;
                pricePositionY = iconPositionY + selectedIcon.Height + (mTexture.Height / 64);
            }

            if(slidingPanel.PanelAction != PanelAction.TowerSelecting)
                spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.1f);

            if (mNamePanelTexture != null)
                spriteBatch.Draw(mNamePanelTexture, positionComponent.Position, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if(selectedIcon != null)
                spriteBatch.Draw(selectedIcon, new Vector2(iconPositionX, iconPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if(mPriceBar != null && slidingPanel.PanelAction != PanelAction.TowerSelecting)
                spriteBatch.Draw(mPriceBar, new Vector2(pricePositionX, pricePositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if(selectedInfoText != null)
                spriteBatch.Draw(selectedInfoText, new Vector2(textPositionX, textPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if(mDescriptionPanelTexture != null)
                spriteBatch.Draw(mDescriptionPanelTexture, new Vector2(descriptionPositionX, descriptionPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            TowerSynchronizer towerSynchronizer = towerSelectLayer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

            if (towerSynchronizer == null)
                return;

            int yTextPadding = mTexture.Height / 192;
            int yTextIncrement = mTexture.Height / 21;
            float xTextStatOffset = position.X + mTexture.Width * 17 / 20;
            float yTextStatOffset = position.Y + mTexture.Height / 5;
            float xTextUnitOffset = position.X + (mTexture.Width * 7/8) + yTextIncrement / 2;
            float yTextUnitOffset = position.Y + mTexture.Height / 5 + yTextIncrement;

            if (towerSynchronizer.Type == ObjectType.Tower)
            {
                TowerData towerData = ObjectFactory.GetDataForTower(towerSynchronizer.TowerType);
                Tower tower = ObjectFactory.Make(towerSynchronizer, 0, 0) as Tower;
                tower.UnRegister();
                float damage = towerData.Damage;
                float vision = towerData.FieldOfView;
                float fireRate = towerData.FireRate;
                int health = tower.HealthComponent.Health;
                int ammo = tower.AmmoCapacityComponent.AmmoCapacity;
                float cooldown = tower.CooldownComponent.TimeSet;
                string description = towerData.Description;
                int price = towerData.PriceToBuy;

                textPositionX = xTextStatOffset;

                spriteBatch.DrawString(mTowerNameFont, towerData.TowerName, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 3, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(towerData.TowerName).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, towerData.Description, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(towerData.Description).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, "$" + towerData.PriceToBuy, new Vector2(pricePositionX + mPriceBar.Width / 2, pricePositionY + mPriceBar.Height / 8), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString("$" + towerData.PriceToBuy).X / 2, mDebugFont.MeasureString("$" + towerData.PriceToBuy).Y / 6), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, damage.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, fireRate.ToString("0.0"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, ((int)(vision / TileMap.TileWidth)).ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, ammo.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, cooldown.ToString("0.0"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                


                textPositionX = xTextUnitOffset;
                textPositionY = yTextUnitOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "SEC", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += 2*yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "SEC", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (towerSynchronizer.Type == ObjectType.AddOn)
            {
                textPositionX = xTextStatOffset;
                textPositionY += yTextIncrement;

                IPlaceable placeable = ObjectFactory.Make(towerSynchronizer, 0, 0);
                AddOn addOn = placeable as AddOn;
                addOn.UnRegister();
                AddOnData addOnData = ObjectFactory.GetDataForAddOn(towerSynchronizer.AddOnType);

                int health = addOn.HealthComponent.Health;
                //float vision = addOn.FieldOfViewShaderComponent.FieldOfView;
                spriteBatch.DrawString(mTowerNameFont, addOnData.AddOnName, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 3, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(addOnData.AddOnName).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, addOnData.Description, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(addOnData.Description).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, ((int)(96 / TileMap.TileWidth)).ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextUnitOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, addOnData.Increase.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, "$" + addOnData.PriceToBuy, new Vector2(pricePositionX + mPriceBar.Width / 2, pricePositionY + mPriceBar.Height / 8), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString("$" + addOnData.PriceToBuy).X / 2, mDebugFont.MeasureString("$" + addOnData.PriceToBuy).Y /6), 1.0f, SpriteEffects.None, 0.14f);

                if (towerSynchronizer.AddOnType == AddOnType.AmmoCapacity)
                    spriteBatch.DrawString(mDebugFont, "CAP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (towerSynchronizer.AddOnType == AddOnType.Cooldown)
                    spriteBatch.DrawString(mDebugFont, "SEC", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (towerSynchronizer.AddOnType == AddOnType.RangeBooster)
                    spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (towerSynchronizer.AddOnType == AddOnType.Refinery)
                    spriteBatch.DrawString(mDebugFont, "MNRL", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
            }

            if (towerSynchronizer.Type == ObjectType.Barricade)
            {
                Barricade barricade = new Barricade("buildings", 0, 0, new Rectangle(0, 0, 0, 0), 0.0f, 1, 1);
                barricade.UnRegister();

                int health = barricade.HealthComponent.Health;
                BarricadeData barricadeData = barricade.Data as BarricadeData;
                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;
                spriteBatch.DrawString(mTowerNameFont, barricadeData.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width * 2/5, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(barricadeData.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, barricadeData.Description, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(barricadeData.Description).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, "$" + barricadeData.PriceToBuy, new Vector2(pricePositionX + mPriceBar.Width / 2, pricePositionY + mPriceBar.Height / 8), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString("$" + barricadeData.PriceToBuy).X / 2, mDebugFont.MeasureString("$" + barricadeData.PriceToBuy).Y / 6), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (towerSynchronizer.Type == ObjectType.ShieldGenerator)
            {
                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                ShieldGenerator shieldGenerator = (ShieldGenerator)ObjectFactory.Make(towerSynchronizer, 0, 0);  
                ShieldGeneratorData data  = shieldGenerator.Data as ShieldGeneratorData;

                shieldGenerator.UnRegister();

                spriteBatch.DrawString(mTowerNameFont, data.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 4, positionComponent.Position.Y ), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(data.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, data.Description, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(data.Description).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, "$" + data.PriceToBuy, new Vector2(pricePositionX + mPriceBar.Width / 2, pricePositionY + mPriceBar.Height / 8), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString("$" + data.PriceToBuy).X / 2, mDebugFont.MeasureString("$" + data.PriceToBuy).Y / 6), 1.0f, SpriteEffects.None, 0.14f);
                int healthBoost = data.HealthBoost;
                int health = shieldGenerator.HealthComponent.Health;
                int vision = 2;

                spriteBatch.DrawString(mDebugFont, healthBoost.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, vision.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (towerSynchronizer.Type == ObjectType.Refinery)
            {
                Refinery refinery = (Refinery) ObjectFactory.Make(towerSynchronizer, 0, 0);
                RefineryData refineryData = refinery.Data as RefineryData;

                refinery.UnRegister();

                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                int numUnitsPerSec = refineryData.NumUnitsPerSecond;
                int health = refinery.HealthComponent.Health;
                int vision = 1;
                spriteBatch.DrawString(mTowerNameFont, refineryData.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width * 2/5, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(refineryData.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, refineryData.Description, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(refineryData.Description).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
                spriteBatch.DrawString(mDebugFont, "$" + refineryData.PriceToBuy, new Vector2(pricePositionX + mPriceBar.Width / 2, pricePositionY + mPriceBar.Height / 8), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString("$" + refineryData.PriceToBuy).X / 2, mDebugFont.MeasureString("$" + refineryData.PriceToBuy).Y / 6), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, numUnitsPerSec.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, vision.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
            }


        }

        public void UpdatePanelStates()
        {
            SlidingPanel slidingPanel = Owner as SlidingPanel;

            if (slidingPanel.PanelAction == PanelAction.TowerSelected)
            {
                slidingPanel.UndoButton.AnimationComponent.IsEnabled = false;
                slidingPanel.UndoButton.AnimationComponent.IsVisible = true;

                slidingPanel.SelectNewButton.AnimationComponent.IsEnabled = true;
                slidingPanel.SelectNewButton.AnimationComponent.IsVisible = true;

                slidingPanel.CancelButton.AnimationComponent.IsEnabled = true;
                slidingPanel.CancelButton.AnimationComponent.IsVisible = true;

                slidingPanel.BuildAllButton.AnimationComponent.IsEnabled = false;
                slidingPanel.BuildAllButton.AnimationComponent.IsVisible = true;
            }
            else if(slidingPanel.PanelAction == PanelAction.TowerPlaced)
            {
                foreach (Button button in slidingPanel.Buttons)
                {
                    button.AnimationComponent.IsEnabled = true;
                    button.AnimationComponent.IsVisible = true;
                }
            }
            else
            {
                foreach (Button button in slidingPanel.Buttons)
                {
                    button.AnimationComponent.IsEnabled = false;
                    button.AnimationComponent.IsVisible = false;
                }
            }


        }
    }

    public class SlidingPanelPositionComponent : PositionComponent
    {
        private Vector2 mDestination;
        private Vector2 mOriginalPosition;
        private Vector2 mDirection = Vector2.Zero;
        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        private int kInitialSpeed;
        private int mSpeed;
        private float mSpeedScale = 0.925f;
        private GraphicsDevice mGraphicsDevice;

        public SlidingPanelPositionComponent(int x, int y, int xDest, int yDest, Entity owner, Vector2 origin = new Vector2()) : base(x, y, owner, origin)
        {
            mOriginalPosition = Position;
            mDestination = new Vector2(xDest, yDest);
            mDirection = new Vector2(xDest - x, yDest - y);
            mDirection.Normalize();

            mGraphicsDevice = ServiceLocator.GetService<GraphicsDevice>();

            kInitialSpeed = mGraphicsDevice.Viewport.Width;
            mSpeed = mGraphicsDevice.Viewport.Width;

        }

        public bool IsOpen
        {
            get { return mPosition == mDestination; }
        }

        public bool IsSlidingIn
        {
            get { return mIsSlidingIn;}
            set
            {
                if (mIsSlidingIn == value) return;

                mIsSlidingIn = value;
                mDirection = new Vector2(mDestination.X - X, mDestination.Y - Y);
                mDirection.Normalize();
            }
        }

        public bool IsSlidingOut
        {
            get { return mIsSlidingOut; }
            set
            {
                if (mIsSlidingOut == value) return;

                mIsSlidingOut = value;
                mDirection = new Vector2(mOriginalPosition.X - X, mOriginalPosition.Y - Y);
                mDirection.Normalize();
            }
        }

        public void Reset()
        {
            SlidingPanel slidingPanel = Owner as SlidingPanel;

            mIsSlidingIn = false;
            mIsSlidingOut = false;
            foreach(Button button in slidingPanel.Buttons)
                UIOps.ResetElementPosition(button, mPosition, mOriginalPosition);

            mPosition = mOriginalPosition;
        }

        public override void Update()
        {
            SlidingPanel slidingPanel = Owner as SlidingPanel;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if(mIsSlidingIn)
            {

                if (mPosition.X - mDestination.X < mGraphicsDevice.Viewport.Width * 17/100)
                    mSpeed = mSpeed > mGraphicsDevice.Viewport.Width * 13/100 ? (int)(mSpeed * mSpeedScale) : mSpeed;



                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X - xOffset.X < mDestination.X)
                    xOffset.X = mDestination.X - mPosition.X;

                mPosition += xOffset;

                foreach(Button button in slidingPanel.Buttons)
                {
                    PositionComponent positionComponent = button.PositionComponent;
                    positionComponent.Position += xOffset;

                    button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                        button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                }
            }

            if(mIsSlidingOut)
            {
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X > mOriginalPosition.X)
                    xOffset.X = mOriginalPosition.X - mPosition.X;

                mPosition += xOffset;

                foreach (Button button in slidingPanel.Buttons)
                {
                    PositionComponent positionComponent = button.PositionComponent;
                    positionComponent.Position += xOffset;

                    button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                        button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
                }
            }

            if(mPosition == mDestination && mIsSlidingIn)
            {
                mIsSlidingIn = false;
                mSpeed = kInitialSpeed;
            }
            if (mPosition == mOriginalPosition && mIsSlidingOut)
            {
                mIsSlidingOut = false;
                slidingPanel.PanelAction = PanelAction.TowerSelecting;
                EventSystem.EnqueueEvent(EventType.SlidingPanelClose);
            }



        }
    }
}
