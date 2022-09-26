using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Entities
{
    public class CooldownAddOn : AddOn
    {
        private CooldownAddOnAnimationComponent mRenderComponent;
        private CooldownAddOnComponent mAddOnComponent;

        public CooldownAddOn(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, IData data) : base(AddOnType.Cooldown)
        {

            PositionComponent = new PositionComponent(x, y, this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new AddOnCollisionComponent(new Rectangle(x, y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new ResistantHealthComponent((data as AddOnData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            Data = data;
            mRenderComponent = new CooldownAddOnAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mAddOnComponent = new CooldownAddOnComponent((data as AddOnData).FieldOfView, this);
        }

        public override void UnRegister()
        {
            mAddOnComponent.UnRegister();
            mRenderComponent.UnRegister();
            PositionComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            HealthComponent.UnRegister();
            ShieldRenderComponent.UnRegister();
            Data = null;

            base.UnRegister();

        }
    }

    public class CooldownAddOnAnimationComponent : AnimationComponent
    {
        private Effect mTransparentEffect;
        
        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;

        
        public CooldownAddOnAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner, bool register = true) 
            : base(texturePath, sourceRectangle, interval, rows,columns, owner)
        {
            // mTransparentEffect = AssetOps.LoadSharedAsset<Effect>("TransparentEffect");
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();
            CooldownAddOn cooldownAddOn = Owner as CooldownAddOn;
            PositionComponent positionComponent = cooldownAddOn.PositionComponent;

            TileNode tile = TileMap.GetTileForPosition(positionComponent.X, positionComponent.Y);
            if (tile != null && tile.Type == TileType.Placed)
            {
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.DarkSlateGray, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);
            }
            else
                spriteBatch.Draw(mTexture, new Vector2(positionComponent.X, positionComponent.Y), CurrentRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.3f);

            if (SystemsManager.Instance.IsPaused)
                return;

            CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }

            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
            }

            CurrentRectangle = new Rectangle(CurrentColumnIndex * SourceRectangle.Width, CurrentRowIndex * SourceRectangle.Height, SourceRectangle.Width, SourceRectangle.Height);

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;


            if (cooldownAddOn.ShowSmokeDamage)
            {
                mCurrentSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                mSmokeSpawnParticleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (mCurrentSpawnParticleTimer <= 0.0f)
                {
                    mCurrentSpawnParticleTimer = kCurrentSpawnParticleTimer;

                    FireParticle fireParticle = new FireParticle(
                        (int)positionComponent.Position.X,
                        (int)positionComponent.Position.Y,
                        "Fire",
                        new Rectangle(0, 0, 32, 32),
                        0.13f,
                        1, 4);

                    if (mSmokeSpawnParticleTimer <= 0.0f)
                    {
                        mSmokeSpawnParticleTimer = kSmokeSpawnParticleTimer;

                        SmokeParticle smokeParticle = new SmokeParticle(
                                (int)fireParticle.PositionComponent.Position.X,
                                (int)fireParticle.PositionComponent.Position.Y - 4,
                           "Simple Black Smoke",
                           new Rectangle(0, 0, 32, 32),
                           0.2f,
                           1, 4);
                    }
                }
            }

            LayerManager.Layer = currentLayer;
        }
    }

    public class CooldownAddOnComponent : EventComponent
    {
        private float mFieldOfView;
        public CooldownAddOnComponent(float fieldOfView, Entity owner, bool register = true) : base(owner, register)
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
            CooldownAddOn addOn = Owner as CooldownAddOn;
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
                        towerData.FireRate = (addIncrease) ? 
                            towerData.FireRate -= data.Increase : 
                            towerData.FireRate += data.Increase;
                        if (tower.CooldownComponent != null && addIncrease)
                        {
                            tower.CooldownComponent.TimeSet -= data.Increase;
                            tower.CooldownComponent.TimeLeft -= data.Increase;
                        }
                        else if(tower.CooldownComponent != null && !addIncrease)
                        {
                            tower.CooldownComponent.TimeSet += data.Increase;
                            tower.CooldownComponent.TimeLeft += data.Increase;
                        }
                    }
                }
            }
        }

        private void OnTowerAdded_(object sender, IGameEventArgs e)
        {
            CooldownAddOn addOn = Owner as CooldownAddOn;
            AddOnData data = addOn.Data as AddOnData;
            PositionComponent positionComponent = addOn.PositionComponent;
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
                            towerData.FireRate -= data.Increase;
                            if (tower.CooldownComponent != null)
                            {
                                tower.CooldownComponent.TimeSet -= data.Increase;
                                tower.CooldownComponent.TimeLeft -= data.Increase;
                            }
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
                    towerData.FireRate -= data.Increase;
                    if (tower.CooldownComponent != null)
                    {
                        tower.CooldownComponent.TimeSet -= data.Increase;
                        tower.CooldownComponent.TimeLeft -= data.Increase;
                    }
                }
            }


        }
    }
}
