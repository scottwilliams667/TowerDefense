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

namespace Tilt.EntityComponent.Entities
{
    public class DamageAddOn 
        : AddOn

    {
        private DamageAddOnRenderComponent mRenderComponent;
        private DamageAddOnComponent mAddOnComponent;
        public DamageAddOn(string texturePath, int x, int y, IData data) : base(AddOnType.Damage)
        {
            PositionComponent = new PositionComponent(x,y, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            mRenderComponent = new DamageAddOnRenderComponent(texturePath, this);
            BoundsCollisionComponent = new AddOnCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            mAddOnComponent = new DamageAddOnComponent((data as AddOnData).FieldOfView, this);
            HealthComponent = new ResistantHealthComponent((data as AddOnData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            Data = data;
        }

        public override void UnRegister()
        {
            mAddOnComponent?.UnRegister();
            mRenderComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            PositionComponent.UnRegister();
            HealthComponent.UnRegister();
            ShieldRenderComponent.UnRegister();
            Data = null;
        }
    }

    public class DamageAddOnRenderComponent : RenderComponent
    {
        private static Texture2D mSellTexture = AssetOps.LoadSharedAsset<Texture2D>("sell");
        public DamageAddOnRenderComponent(string texturePath, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
        }


        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            DamageAddOn damageAddOn = Owner as DamageAddOn;
            PositionComponent positionComponent = damageAddOn.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, null, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.3f);

            if(damageAddOn.ShowSmokeDamage)
            {
                //smoke
            }
        }
    }

    public class DamageAddOnComponent : EventComponent
    {
        private float mFieldOfView;
        public DamageAddOnComponent(float fieldOfView, Entity owner, bool register = true) : base(owner)
        {
            mFieldOfView = fieldOfView;
        }

        public override void Register()
        {
            EventSystem.SubScribe(EventType.TowerAdded, OnTowerAdded_);
            base.Register();
        }

        public override void UnRegister()
        {
            EventSystem.UnSubScribe(EventType.TowerAdded, OnTowerAdded_);
            QueryTowers_(false);
            base.UnRegister();
        }

        private void QueryTowers_(bool addIncrease)
        {
            DamageAddOn addOn = Owner as DamageAddOn;
            AddOnData data = addOn.Data as AddOnData;
            CollisionComponent collisionComponent = addOn.BoundsCollisionComponent;
            PositionComponent positionComponent = addOn.PositionComponent;

            if (collisionComponent.Cells == null || 
                collisionComponent.Cells.Count == 0)
                return;

            List<int> surroundingCells = CollisionHelper.GetSurroundingCells(collisionComponent.Cells.First());
            foreach (int cell in surroundingCells)
            {

                List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(cell);
                foreach (CollisionComponent component in nearbyComponents)
                {
                    if (!(component.Owner is Tower))
                        continue;

                    Tower tower = component.Owner as Tower;
                    if (Vector2.Distance(positionComponent.Origin, tower.PositionComponent.Origin) < mFieldOfView)
                    {
                        TowerData towerData = tower.Data as TowerData;
                        towerData.Damage *= (addIncrease) ? data.Increase : (1/data.Increase);
                    }

                }
            }

        }

        private void OnTowerAdded_(object sender, IGameEventArgs e)
        {
            DamageAddOn addOn = Owner as DamageAddOn;
            AddOnData data = addOn.Data as AddOnData;
            PositionComponent positionComponent = addOn.PositionComponent;
            CollisionComponent collisionComponent = addOn.BoundsCollisionComponent;
            Vector2 origin = positionComponent.Origin;
            if (sender is List<IPlaceable>)
            {
                 List<IPlaceable> objects = sender as List<IPlaceable>;
                //we have added ourselves, lets query what towers we have around us and 
                //add the inc.
                if (objects.Any(o => o == Owner))
                {
                    QueryTowers_(true);
                }
                else
                {
                    foreach (IPlaceable obj in objects)
                    {
                        if (Vector2.Distance(origin, obj.PositionComponent.Origin) < mFieldOfView && obj is Tower)
                        {
                            Tower tower = obj as Tower;
                            TowerData towerData = tower.Data as TowerData;
                            towerData.Damage *= data.Increase;
                        }
                    }
                }
            }
            else
            {
                IPlaceable placeable = sender as IPlaceable;
                if (Vector2.Distance(origin, placeable.PositionComponent.Origin) < mFieldOfView && placeable is Tower)
                {

                    Tower tower = placeable as Tower;
                    TowerData towerData = tower.Data as TowerData;
                    towerData.Damage *= data.Increase;
                }
            }
        }

    }
}
