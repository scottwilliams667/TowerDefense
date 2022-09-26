using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Utilities
{
    public class UIOps
    {
        public static UIElement FindElementByName(string name)
        {
            List <UIElement> menuElements =
                LayerManager.Layers.SelectMany(l => l.EntitySystem.Entities).Where(e => e is UIElement).Cast<UIElement>().ToList();
            return menuElements.FirstOrDefault(element => element.Name == name);

        }

        public static void ResetPanelStatePositions(PanelState panelState, Vector2 currentPosition, Vector2 oldPosition)
        {
            foreach (UIElement element in panelState.Elements)
            {
               ResetElementPosition(element, currentPosition, oldPosition);
            }
        }

        public static void ResetElementPosition(UIElement element, Vector2 currentPosition, Vector2 oldPosition)
        {
            Vector2 difference = oldPosition - currentPosition;

             PositionComponent positionComponent = element.PositionComponent;
                positionComponent.Position = positionComponent.Position + difference;

            if (element is Button)
            {
                Button button = element as Button;
                button.TouchComponent.Bounds = new Rectangle((int)positionComponent.X, (int)positionComponent.Y,
                        button.TouchComponent.Bounds.Width, button.TouchComponent.Bounds.Height);
            }
        }

        public static void EnableDisableHudButtons(bool enable)
        {
            Layer hudLayer = LayerManager.GetLayer(LayerType.Hud);

            if (hudLayer == null)
                return;

            List<Button> buttons = hudLayer.EntitySystem.GetEntitiesByType<Button>();

            foreach (Button button in buttons)
            {
                button.AnimationComponent.IsEnabled = enable;
            }


        }
        
        public static Texture2D GetIconSource()
        {
            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            TowerSynchronizer towerSynchronizer = null;

            ObjectType objType = ObjectType.None;
            TowerType towerType = TowerType.None;
            AddOnType addOnType = AddOnType.None;


            if (towerSelectLayer != null)
            {
                towerSynchronizer = towerSelectLayer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

                if (towerSynchronizer == null)
                    return null;

                objType = towerSynchronizer.Type;
                towerType = towerSynchronizer.TowerType;
                addOnType = towerSynchronizer.AddOnType;
            }
            else
            {
                if (gameLayer == null)
                    return null;

                TileNode selectedTile = TileMap.SelectedTile;

                if (selectedTile == null)
                    return null;

                IPlaceable placeable = selectedTile.Object;
                objType = placeable.ObjectType;

                if (objType == ObjectType.Tower)
                {
                    Tower tower = placeable as Tower;
                    towerType = tower.Type;
                }
                if (objType == ObjectType.AddOn)
                {
                    AddOn addOn = placeable as AddOn;
                    addOnType = addOn.Type;
                }
            }

            Texture2D selectedTexture = null;
            if (objType == ObjectType.Tower)
            {
                switch(towerType)
                {
                    case TowerType.Bullet:
                        selectedTexture = mBulletTowerIcon;
                        break;
                    case TowerType.Heavy:
                        selectedTexture = mHeavyTowerIcon;
                        break;
                    case TowerType.Shotgun:
                        selectedTexture = mShotgunTowerIcon;
                        break;
                    case TowerType.Laser:
                        selectedTexture = mLaserTowerIcon;
                        break;
                    case TowerType.Nuclear:
                        selectedTexture = mNuclearTowerIcon;
                        break;
                    case TowerType.Rocket:
                        selectedTexture = mRocketTowerIcon;
                        break;
                }
            }

            if (objType == ObjectType.AddOn)
            {
                switch(addOnType)
                {
                    case AddOnType.Cooldown:
                        selectedTexture = mCoolingAddOnIcon;
                        break;
                    case AddOnType.AmmoCapacity:
                        selectedTexture = mAmmoAddOnIcon;
                        break;
                    case AddOnType.Refinery:
                        selectedTexture = mRefineryAddOnIcon;
                        break;
                    case AddOnType.RangeBooster:
                        selectedTexture = mRangeBoosterAddOnIcon;
                        break;
                }
            }

            if (objType == ObjectType.Barricade)
                selectedTexture = mBarricadeIcon;

            if (objType == ObjectType.ShieldGenerator)
                selectedTexture = mShieldGeneratorIcon;

            if (objType == ObjectType.Refinery)
                selectedTexture = mRefineryIcon;

            return selectedTexture;
        }
    
        

        public static Texture2D GetTextSource()
        {
            Layer towerSelectLayer = LayerManager.GetLayer(LayerType.TowerSelect);
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            TowerSynchronizer towerSynchronizer = null;

            ObjectType objType = ObjectType.None;


            if (towerSelectLayer != null)
            {
                towerSynchronizer = towerSelectLayer.EntitySystem.GetEntitiesByType<TowerSynchronizer>().FirstOrDefault();

                if (towerSynchronizer == null)
                    return null;

                objType = towerSynchronizer.Type;
            }
            else
            {
                if (gameLayer == null)
                    return null;

                TileNode selectedTile = TileMap.SelectedTile;

                if (selectedTile == null)
                    return null;

                IPlaceable placeable = selectedTile.Object;
                objType = placeable.ObjectType;
            }

            Texture2D selectedTexture = null;

            if (objType == ObjectType.Tower)
                selectedTexture = mTowerInfoText;

            if (objType == ObjectType.AddOn)
                selectedTexture = mAddOnInfoText;

            if (objType == ObjectType.Barricade)
                selectedTexture = mBarricadeInfoText;

            if (objType == ObjectType.ShieldGenerator)
                selectedTexture = mShieldGeneratorInfoText;

            if (objType == ObjectType.Refinery)
                selectedTexture = mRefineryInfoText;

            return selectedTexture;
        }
        
        public static Texture2D GetGraphSource()
        {
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);
            
            if (gameLayer == null)
                return null;

            ObjectType objType = ObjectType.None;
            TowerType towerType = TowerType.None;
            AddOnType addOnType = AddOnType.None;

            TileNode selectedTile = TileMap.SelectedTile;

            if (selectedTile == null)
                return null;

            IPlaceable placeable = selectedTile.Object;
            objType = placeable.ObjectType;

            if (objType == ObjectType.Tower)
            {
                Tower tower = placeable as Tower;
                towerType = tower.Type;
            }
            if (objType == ObjectType.AddOn)
            {
                AddOn addOn = placeable as AddOn;
                addOnType = addOn.Type;
            }
            

            Texture2D selectedTexture = null;
            if (objType == ObjectType.Tower)
            {
                switch (towerType)
                {
                    case TowerType.Bullet:
                        selectedTexture = mEfficiencyGraph5;
                        break;
                    case TowerType.Heavy:
                        selectedTexture = mEfficiencyGraph7;
                        break;
                    case TowerType.Shotgun:
                        selectedTexture = mEfficiencyGraph10;
                        break;
                    case TowerType.Laser:
                        selectedTexture = mEfficiencyGraph3;
                        break;
                    case TowerType.Nuclear:
                        selectedTexture = mEfficiencyGraph4;
                        break;
                    case TowerType.Rocket:
                        selectedTexture = mEfficiencyGraph9;
                        break;
                }
            }

            if (objType == ObjectType.AddOn)
            {
                switch (addOnType)
                {
                    case AddOnType.Cooldown:
                        selectedTexture = mEfficiencyGraph1;
                        break;
                    case AddOnType.AmmoCapacity:
                        selectedTexture = mEfficiencyGraph2;
                        break;
                    case AddOnType.Refinery:
                        selectedTexture = mEfficiencyGraph13;
                        break;
                    case AddOnType.RangeBooster:
                        selectedTexture = mEfficiencyGraph6;
                        break;
                }
            }

            if (objType == ObjectType.Barricade)
                selectedTexture = mEfficiencyGraph8;

            if (objType == ObjectType.ShieldGenerator)
                selectedTexture = mEfficiencyGraph12;

            if (objType == ObjectType.Refinery)
                selectedTexture = mEfficiencyGraph11;

            return selectedTexture;
        
        }



        private static Texture2D mEfficiencyGraph1 = AssetOps.LoadAsset<Texture2D>("efficiencygraph1");
        private static Texture2D mEfficiencyGraph2 = AssetOps.LoadAsset<Texture2D>("efficiencygraph2");
        private static Texture2D mEfficiencyGraph3 = AssetOps.LoadAsset<Texture2D>("efficiencygraph3");
        private static Texture2D mEfficiencyGraph4 = AssetOps.LoadAsset<Texture2D>("efficiencygraph4");
        private static Texture2D mEfficiencyGraph5 = AssetOps.LoadAsset<Texture2D>("efficiencygraph5");
        private static Texture2D mEfficiencyGraph6 = AssetOps.LoadAsset<Texture2D>("efficiencygraph6");
        private static Texture2D mEfficiencyGraph7 = AssetOps.LoadAsset<Texture2D>("efficiencygraph7");
        private static Texture2D mEfficiencyGraph8 = AssetOps.LoadAsset<Texture2D>("efficiencygraph8");
        private static Texture2D mEfficiencyGraph9 = AssetOps.LoadAsset<Texture2D>("efficiencygraph9");
        private static Texture2D mEfficiencyGraph10 = AssetOps.LoadAsset<Texture2D>("efficiencygraph10");
        private static Texture2D mEfficiencyGraph11 = AssetOps.LoadAsset<Texture2D>("efficiencygraph11");
        private static Texture2D mEfficiencyGraph12 = AssetOps.LoadAsset<Texture2D>("efficiencygraph12");
        private static Texture2D mEfficiencyGraph13 = AssetOps.LoadAsset<Texture2D>("efficiencygraph0");
        
        private static Texture2D mBulletTowerIcon = AssetOps.LoadAsset<Texture2D>("pistoltowericon");
        private static Texture2D mHeavyTowerIcon = AssetOps.LoadAsset<Texture2D>("heavytowericon");
        private static Texture2D mShotgunTowerIcon = AssetOps.LoadAsset<Texture2D>("shotguntowericon");
        private static Texture2D mLaserTowerIcon = AssetOps.LoadAsset<Texture2D>("lasertowericon");
        private static Texture2D mNuclearTowerIcon = AssetOps.LoadAsset<Texture2D>("nucleartowericon");
        private static Texture2D mCoolingAddOnIcon = AssetOps.LoadAsset<Texture2D>("coolingtowericon");
        private static Texture2D mRocketTowerIcon = AssetOps.LoadAsset<Texture2D>("rockettowericon");
        private static Texture2D mAmmoAddOnIcon = AssetOps.LoadAsset<Texture2D>("ammoaddonicon");
        private static Texture2D mRefineryAddOnIcon = AssetOps.LoadAsset<Texture2D>("refineryaddonicon");
        private static Texture2D mRangeBoosterAddOnIcon = AssetOps.LoadAsset<Texture2D>("rangeboostericon");
        private static Texture2D mBarricadeIcon = AssetOps.LoadAsset<Texture2D>("barricadeicon");
        private static Texture2D mShieldGeneratorIcon = AssetOps.LoadAsset<Texture2D>("shieldgeneratoricon");
        private static Texture2D mRefineryIcon = AssetOps.LoadAsset<Texture2D>("refineryicon");
        
        private static Texture2D mTowerInfoText = AssetOps.LoadAsset<Texture2D>("towerinfotext");
        private static Texture2D mAddOnInfoText = AssetOps.LoadAsset<Texture2D>("addoninfotext");
        private static Texture2D mShieldGeneratorInfoText = AssetOps.LoadAsset<Texture2D>("shieldgeneratorinfotext");
        private static Texture2D mRefineryInfoText = AssetOps.LoadAsset<Texture2D>("refineryinfotext");
        private static Texture2D mBarricadeInfoText = AssetOps.LoadAsset<Texture2D>("barricadeinfotext");

    }


}
