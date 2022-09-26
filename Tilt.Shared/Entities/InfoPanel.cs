using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Entities
{
    public class InfoPanel : UIElement
    {
        private InfoPanelRenderComponent mRenderComponent;
        private Button mSellButton;
        private Button mCancelButton;

        public InfoPanel(int x, int y, int xDest, int yDest, string texturePath, Action action = null, bool register = true, string name = null) :
            base(x, y, register, name)
        {
            mRenderComponent = new InfoPanelRenderComponent(texturePath, this);
            PositionComponent = new InfoPanelPositionComponent(x, y, xDest, yDest, action, this);

            GraphicsDeviceManager deviceManager = ServiceLocator.GetService<GraphicsDeviceManager>();
            int viewportWidth = deviceManager.PreferredBackBufferWidth;
            int viewportHeight = deviceManager.PreferredBackBufferHeight;
            int buttonXOffset = viewportWidth / 12;

            mSellButton = new NormalButton(0, viewportHeight * 4/5, "sellbutton",
                new Rectangle(0,0, 0,0 ), 1, 2, () => { EventSystem.EnqueueEvent(EventType.SellObject); }, Tuner.SFXUISell, "InfoPanelSellButton");
            mCancelButton = new NormalButton(0, viewportHeight * 4/5, "cancelbutton",
                new Rectangle(0,0, 0, 0), 1, 2, () =>
            {
                InfoPanel info = LayerManager.Layer.EntitySystem.GetEntitiesByType<InfoPanel>().FirstOrDefault();
                InfoPanelPositionComponent panelPositionComponent = info.PositionComponent as InfoPanelPositionComponent;
                panelPositionComponent.IsSlidingOut = true;
                TileMap.SelectedTile = null;

            }, Tuner.SFXCancel, "InfoPanelCancelButton");

            mSellButton.PositionComponent.X = viewportWidth + buttonXOffset;
            mSellButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mSellButton.AnimationComponent.Texture.Width / 2, mSellButton.AnimationComponent.Texture.Height);
            mSellButton.TouchComponent.Bounds = new Rectangle((int)mSellButton.PositionComponent.X, (int)mSellButton.PositionComponent.Y, mSellButton.AnimationComponent.SourceRectangle.Width, mSellButton.AnimationComponent.SourceRectangle.Height);

            mCancelButton.PositionComponent.X = viewportWidth + 2 * buttonXOffset + mSellButton.AnimationComponent.SourceRectangle.Width / 2;
            mCancelButton.AnimationComponent.SourceRectangle = new Rectangle(0, 0, mCancelButton.AnimationComponent.Texture.Width / 2, mCancelButton.AnimationComponent.Texture.Height);
            mCancelButton.TouchComponent.Bounds = new Rectangle((int)mCancelButton.PositionComponent.X, (int)mCancelButton.PositionComponent.Y, mCancelButton.AnimationComponent.SourceRectangle.Width, mCancelButton.AnimationComponent.SourceRectangle.Height);

        }

        public List<Button> Buttons
        {
            get {  return new List<Button>() { mSellButton, mCancelButton};}
        }

        public void Reset()
        {
            InfoPanelPositionComponent positionComponent = PositionComponent as InfoPanelPositionComponent;
            positionComponent.Reset();
        }
    }

    public class InfoPanelRenderComponent : UIRenderComponent
    {
        private SpriteFont mDebugFont;
        private SpriteFont mTowerNameFont;
        private Texture2D mNamePanelTexture;
        private Texture2D mDescriptionPanelTexture;
        

        public InfoPanelRenderComponent(string texturePath, Entity owner)
            : base(texturePath, owner)
        {
            mDebugFont = AssetOps.LoadAsset<SpriteFont>("DebugFont");
            mTowerNameFont = AssetOps.LoadAsset<SpriteFont>("TowerNameFont");
            mNamePanelTexture = AssetOps.LoadAsset<Texture2D>("namepanel");
            mDescriptionPanelTexture = AssetOps.LoadAsset<Texture2D>("descriptionpanel");

        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            InfoPanel panel = Owner as InfoPanel;
            InfoPanelPositionComponent positionComponent = panel.PositionComponent as InfoPanelPositionComponent;

            Vector2 position = positionComponent.Position;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.1f);
            
            Texture2D selectedIcon = UIOps.GetIconSource();
            Texture2D selectedInfoText = UIOps.GetTextSource();
            Texture2D selectedGraph = UIOps.GetGraphSource();

            float iconPositionX = position.X + mTexture.Width / 16;
            float iconPositionY = position.Y + mTexture.Height / 4;

            float textPositionX = position.X + ((mTexture.Width * 13) / 32);
            float textPositionY = position.Y + mTexture.Height / 5;

            float statusPositionX = position.X;
            float statusPositionY = mTexture.Height / 2;

            float graphPositionX = position.X + mTexture.Width / 48;
            float graphPositionY = position.Y + mTexture.Height * 58 / 100;

            float descriptionPositionX = position.X;
            float descriptionPositionY = mTexture.Height / 2;


            if (mNamePanelTexture != null)
                spriteBatch.Draw(mNamePanelTexture, positionComponent.Position, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if (selectedIcon != null)
                spriteBatch.Draw(selectedIcon, new Vector2(iconPositionX, iconPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if (selectedInfoText != null)
                spriteBatch.Draw(selectedInfoText, new Vector2(textPositionX, textPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
            
            if(selectedGraph != null)
                spriteBatch.Draw(selectedGraph, new Vector2(graphPositionX, graphPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            if (mDescriptionPanelTexture != null)
                spriteBatch.Draw(mDescriptionPanelTexture, new Vector2(descriptionPositionX, descriptionPositionY), null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            TileNode tileNode = TileMap.SelectedTile;
            

            if (tileNode == null)
                return;

            
            ObjectType objectType = tileNode.Object.ObjectType;
            IPlaceable placeable = tileNode.Object;
            ICollideable collideable = tileNode.Object as ICollideable;
            IHealable healable = tileNode.Object as IHealable;

            int yTextPadding = mTexture.Height / 192;
            int yTextIncrement = mTexture.Height / 21;
            float xTextStatOffset = position.X + mTexture.Width * 17 / 20;
            float yTextStatOffset = position.Y + mTexture.Height / 5;
            float xTextUnitOffset = position.X + (mTexture.Width * 7 / 8) + yTextIncrement / 2;
            float yTextUnitOffset = position.Y + mTexture.Height / 5 + yTextIncrement;
            float xTextUnitCountOffset = position.X + (mTexture.Width * 21 / 26);

            int countRangeBooster = 0;
            int countCooldown = 0;
            int countAmmoCapacity = 0;
            int countShieldGenerator = 0;
            int countRefineryAddOn = 0;

            if (collideable != null)
            {
                BoundsCollisionComponent collisionComponent = collideable.BoundsCollisionComponent;
                List<int> surroundingCells = CollisionHelper.GetSurroundingCells(collisionComponent.Cells.FirstOrDefault());
                foreach (int surroundingCell in surroundingCells)
                {
                    List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(surroundingCell);
                    List<RangeBoosterAddOn> rangeBoosters = nearbyComponents.Where(c => c.Owner is RangeBoosterAddOn).Select(c => c.Owner).Cast<RangeBoosterAddOn>().ToList();
                    foreach (RangeBoosterAddOn rangeBooster in rangeBoosters)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, rangeBooster.PositionComponent.Position) < (rangeBooster.Data as AddOnData).FieldOfView)
                        {
                            countRangeBooster++;
                        }
                    }

                    List<CooldownAddOn> cooldownAddOns = nearbyComponents.Where(c => c.Owner is CooldownAddOn).Select(c => c.Owner).Cast<CooldownAddOn>().ToList();
                    foreach (CooldownAddOn cooldownAddOn in cooldownAddOns)
                    {
                        AddOnData addOnData = cooldownAddOn.Data as AddOnData;
                        if (addOnData != null && Vector2.Distance(placeable.PositionComponent.Position, cooldownAddOn.PositionComponent.Position) < addOnData.FieldOfView)
                        {
                            countCooldown++;
                        }
                    }

                    List<AmmoAddOn> ammoAddOns = nearbyComponents.Where(c => c.Owner is AmmoAddOn).Select(c => c.Owner).Cast<AmmoAddOn>().ToList();
                    foreach (AmmoAddOn ammoAddOn in ammoAddOns)
                    {
                        AddOnData addOnData = ammoAddOn.Data as AddOnData;
                        if (addOnData != null && Vector2.Distance(placeable.PositionComponent.Position, ammoAddOn.PositionComponent.Position) < (ammoAddOn.Data as AddOnData).FieldOfView)
                        {
                            countAmmoCapacity++;
                        }
                    }
                    List<ShieldGenerator> shieldGenerators = nearbyComponents.Where(c => c.Owner is ShieldGenerator).Select(c => c.Owner).Cast<ShieldGenerator>().ToList();
                    foreach (ShieldGenerator shieldGenerator in shieldGenerators)
                    {
                        ShieldGeneratorData shieldGeneratorData = shieldGenerator.Data as ShieldGeneratorData;
                        if (shieldGeneratorData != null && Vector2.Distance(placeable.PositionComponent.Position, shieldGenerator.PositionComponent.Position) < shieldGeneratorData.FieldOfView)
                        {
                            countShieldGenerator++;
                        }
                    }

                    List<RefineryAddOn> refineryAddOns = nearbyComponents.Where(c => c.Owner is RefineryAddOn).Select(c => c.Owner).Cast<RefineryAddOn>().ToList();
                    foreach (RefineryAddOn refineryAddOn in refineryAddOns)
                    {
                        AddOnData refineryData = refineryAddOn.Data as AddOnData;
                        if (refineryData != null && Vector2.Distance(placeable.PositionComponent.Position, refineryAddOn.PositionComponent.Position) < refineryData.FieldOfView)
                        {
                            countRefineryAddOn++;
                        }
                    }
                }

            }

            if (objectType == ObjectType.Tower)
            {
                Tower tower = placeable as Tower;
                TowerData towerData = tower.Data as TowerData;

                float damage = towerData.Damage;
                float vision = towerData.FieldOfView;
                float fireRate = towerData.FireRate;
                int health = tower.HealthComponent.Health;
                int ammo = tower.AmmoCapacityComponent.AmmoCapacity;
                float cooldown = tower.CooldownComponent.TimeSet;


                textPositionX = xTextUnitCountOffset;
                textPositionY += yTextIncrement;

                if(countShieldGenerator != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countShieldGenerator, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionY += yTextIncrement;

                if (countCooldown != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countCooldown, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionY += yTextIncrement;

                if(countRangeBooster != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countRangeBooster, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionY += yTextIncrement;

                if(countAmmoCapacity != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countAmmoCapacity, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionY += yTextIncrement;

                if (countCooldown != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countCooldown, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                spriteBatch.DrawString(mTowerNameFont, towerData.TowerName, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 3, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(towerData.TowerName).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, damage.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
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
                textPositionY += 2 * yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "SEC", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (objectType == ObjectType.AddOn)
            {
                


                AddOn addOn = placeable as AddOn;
                AddOnData addOnData = addOn.Data as AddOnData;

                textPositionX = xTextUnitCountOffset;
                textPositionY += yTextIncrement;

                if (countShieldGenerator != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countShieldGenerator, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextStatOffset;
                

                int health = addOn.HealthComponent.Health;
                //float vision = addOn.FieldOfViewShaderComponent.FieldOfView;

                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, ((int)(addOnData.FieldOfView / TileMap.TileWidth)).ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

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

                spriteBatch.DrawString(mTowerNameFont, addOnData.AddOnName, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 3, positionComponent.Position.Y ), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(addOnData.AddOnName).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                if (addOn.Type == AddOnType.AmmoCapacity)
                    spriteBatch.DrawString(mDebugFont, "CAP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (addOn.Type == AddOnType.Cooldown)
                    spriteBatch.DrawString(mDebugFont, "SEC", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (addOn.Type == AddOnType.RangeBooster)
                    spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                if (addOn.Type == AddOnType.Refinery)
                    spriteBatch.DrawString(mDebugFont, "MNRL", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
            }

            if (objectType == ObjectType.Barricade)
            {
                Barricade barricade = placeable as Barricade;
                BarricadeData barricadeData = barricade.Data as BarricadeData;

                textPositionX = xTextUnitCountOffset;

                spriteBatch.DrawString(mTowerNameFont, barricadeData.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width * 2/5, positionComponent.Position.Y ), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(barricadeData.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                if (countShieldGenerator != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countShieldGenerator, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);


                int health = barricade.HealthComponent.Health;

                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (objectType == ObjectType.ShieldGenerator)
            {
                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                ShieldGenerator shieldGenerator = placeable as ShieldGenerator;
                ShieldGeneratorData shieldGeneratorData = shieldGenerator.Data as ShieldGeneratorData;

                int healthBoost = shieldGeneratorData.HealthBoost;
                int health = shieldGenerator.HealthComponent.Health;
                float vision = shieldGeneratorData.FieldOfView;

                spriteBatch.DrawString(mTowerNameFont, shieldGeneratorData.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width / 4, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(shieldGeneratorData.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                spriteBatch.DrawString(mDebugFont, healthBoost.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, health.ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, ((int)vision / TileMap.TileWidth).ToString(), new Vector2(textPositionX, textPositionY - yTextPadding), Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

                textPositionX = xTextUnitOffset;
                textPositionY = yTextStatOffset;

                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "HP", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);
                textPositionY += yTextIncrement;
                spriteBatch.DrawString(mDebugFont, "TILES", new Vector2(textPositionX, textPositionY - yTextPadding), Color.Gray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);

            }

            if (objectType == ObjectType.Refinery)
            {
                Refinery refinery = placeable as Refinery;
                RefineryData refineryData = refinery.Data as RefineryData;

                if (refinery == null || refinery.Data == null)
                    return;

                textPositionX = xTextUnitCountOffset;
                textPositionY += yTextIncrement;

                spriteBatch.DrawString(mTowerNameFont, refineryData.Name, new Vector2(positionComponent.Position.X + mNamePanelTexture.Width *2/5, positionComponent.Position.Y), Color.Black, 0.0f, new Vector2(mDebugFont.MeasureString(refineryData.Name).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);

                if (countShieldGenerator != 0)
                    spriteBatch.DrawString(mDebugFont, string.Format("{0}{1}", countShieldGenerator, "+"), new Vector2(textPositionX, textPositionY - yTextPadding), Color.Red, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.11f);


                textPositionX = xTextStatOffset;
                textPositionY = yTextStatOffset;

                int numUnitsPerSec = refineryData.NumUnitsPerSecond;
                int health = refinery.HealthComponent.Health;
                int vision = 1;

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
            
            if (healable.HealthComponent.HealthPercentage > 0.8f)
            {
                spriteBatch.DrawString(mDebugFont, Tuner.OperatingNormallyDescription, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(Tuner.OperatingNormallyDescription).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
            }
            else if (healable.HealthComponent.HealthPercentage > 0.6f)
            {
                spriteBatch.DrawString(mDebugFont, Tuner.DamageDetectedDescription, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(Tuner.DamageDetectedDescription).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
            }
            else
            {
                spriteBatch.DrawString(mDebugFont, Tuner.CriticallyDamagedDescription, new Vector2(descriptionPositionX + mDescriptionPanelTexture.Width / 2, descriptionPositionY + mDescriptionPanelTexture.Height / 4), Color.DarkRed, 0.0f, new Vector2(mDebugFont.MeasureString(Tuner.CriticallyDamagedDescription).X / 2, 0), 1.0f, SpriteEffects.None, 0.14f);
            }
        }
            

        
    }

    public class InfoPanelPositionComponent : PositionComponent
    {
        private Vector2 mDestination;
        private Vector2 mOriginalPosition;
        private Vector2 mDirection = Vector2.Zero;
        private bool mIsSlidingIn;
        private bool mIsSlidingOut;
        private int mSpeed;
        private int kInitialSpeed;
        private float mSpeedScale = 0.925f;
        
        private Action mAction;

        private GraphicsDevice mGraphicsDevice;

        public InfoPanelPositionComponent(int x, int y, int xDest, int yDest, Action action, Entity owner)
            : base(x, y, owner)
        {
            mOriginalPosition = mPosition;
            mDestination = new Vector2(xDest, yDest);
            mDirection = new Vector2(xDest - X, yDest - Y);
            mDirection.Normalize();
            mAction = action;

            mGraphicsDevice = ServiceLocator.GetService<GraphicsDevice>();


            mSpeed = mGraphicsDevice.Viewport.Width;
            kInitialSpeed = mGraphicsDevice.Viewport.Width;
        }

        public bool IsSlidingIn
        {
            get { return mIsSlidingIn; }
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
            InfoPanel infoPanel = Owner as InfoPanel;
            mIsSlidingIn = false;
            mIsSlidingOut = false;

            foreach(Button button in infoPanel.Buttons)
                UIOps.ResetElementPosition(button, mPosition, mOriginalPosition);

            mPosition = mOriginalPosition;
        }

        public override void Update()
        {
            InfoPanel infoPanel = Owner as InfoPanel;
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            if(mIsSlidingIn)
            {

                if (mPosition.X - mDestination.X < mGraphicsDevice.Viewport.Width * 17/100)
                    mSpeed = mSpeed > mGraphicsDevice.Viewport.Width * 13/100 ? (int)(mSpeed * mSpeedScale) : mSpeed;



                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X - xOffset.X < mDestination.X)
                    xOffset.X = mDestination.X - mPosition.X;

                mPosition += xOffset;

                foreach(Button button in infoPanel.Buttons)
                {
                    PositionComponent positionComponent = button.PositionComponent;
                    positionComponent.Position += xOffset;

                    button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                        button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);

                    button.AnimationComponent.IsEnabled = false;
                }
            }

            if(mIsSlidingOut)
            {
                Vector2 xOffset = mSpeed * mDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mPosition.X + xOffset.X > mOriginalPosition.X)
                    xOffset.X = mOriginalPosition.X - mPosition.X;

                mPosition += xOffset;

                foreach (Button button in infoPanel.Buttons)
                {
                    PositionComponent positionComponent = button.PositionComponent;
                    positionComponent.Position += xOffset;

                    button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                        button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);

                    button.AnimationComponent.IsEnabled = false;
                }
            }

            if(mPosition == mDestination && mIsSlidingIn)
            {
                mIsSlidingIn = false;
                mSpeed = kInitialSpeed;

                foreach(Button button in infoPanel.Buttons)
                {
                    button.AnimationComponent.IsEnabled = true;
                }
            }
            if (mPosition == mOriginalPosition && mIsSlidingOut)
            {
                mIsSlidingOut = false;
                if (mAction != null)
                    mAction();
            }
        }

    }
}
