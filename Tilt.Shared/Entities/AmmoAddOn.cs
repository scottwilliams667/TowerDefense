﻿using System;
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
using Tilt.Shared.Entities;

namespace Tilt.EntityComponent.Entities
{
    public class AmmoAddOn : AddOn
    {
        private AmmoAnimationComponent mRenderComponent;
        private AmmoAddOnComponent mAddOnComponent;
        public AmmoAddOn(string texturePath, int x, int y, Rectangle sourceRectangle, float interval, int rows, int columns, IData data)
            : base(AddOnType.AmmoCapacity)
        {
            PositionComponent = new PositionComponent(x,y,this, new Vector2(x + TileMap.TileWidth / 2, y + TileMap.TileHeight / 2));
            BoundsCollisionComponent = new AddOnCollisionComponent(new Rectangle(x,y, TileMap.TileWidth, TileMap.TileHeight), this);
            HealthComponent = new ResistantHealthComponent((data as AddOnData).Health, this);
            ShieldRenderComponent = new ShieldRenderComponent("shield", "shielddamage", this);
            mRenderComponent = new AmmoAnimationComponent(texturePath, sourceRectangle, interval, rows, columns, this);
            mAddOnComponent = new AmmoAddOnComponent((data as AddOnData).FieldOfView, this);
            Data = data;
        }

        public override void UnRegister()
        {
            PositionComponent.UnRegister();
            BoundsCollisionComponent.UnRegister();
            HealthComponent.UnRegister();
            ShieldRenderComponent.UnRegister();
            mRenderComponent.UnRegister();
            mAddOnComponent.UnRegister();
            base.UnRegister();
        }
    }

    public class AmmoAnimationComponent : AnimationComponent
    {
        private Effect mTransparentEffect;

        private float mCurrentSpawnParticleTimer = 0.1f;
        private float kCurrentSpawnParticleTimer = 0.1f;
        private float mSmokeSpawnParticleTimer = 0.2f;
        private float kSmokeSpawnParticleTimer = 0.2f;


        public AmmoAnimationComponent(string texturePath, Rectangle sourceRectangle, float interval, int rows, int columns, Entity owner)
            : base(texturePath, sourceRectangle, interval, rows, columns, owner)
        {
            // mTransparentEffect = AssetOps.LoadSharedAsset<Effect>("TransparentEffect");
        }


        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GameTime gameTime = ServiceLocator.GetService<GameTime>();

            AmmoAddOn ammoAddOn = Owner as AmmoAddOn;
            PositionComponent positionComponent = ammoAddOn.PositionComponent;

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
            if (CurrentTime <= 0.0f)
            {
                CurrentColumnIndex++;
                CurrentTime = Interval;
            }
            if(CurrentColumnIndex >= Columns)
            {
                CurrentColumnIndex = 0;
            }

            CurrentRectangle = new Rectangle(SourceRectangle.X + (CurrentColumnIndex * SourceRectangle.Width), SourceRectangle.Y + (CurrentRowIndex * SourceRectangle.Height), SourceRectangle.Width, SourceRectangle.Height);

            Layer currentLayer = LayerManager.Layer;
            Layer gameLayer = LayerManager.GetLayer(LayerType.Game);

            LayerManager.Layer = gameLayer;


            if (ammoAddOn.ShowSmokeDamage)
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

    public class AmmoAddOnComponent : EventComponent
    {
        private float mFieldOfView;

        public AmmoAddOnComponent(float fieldOfView, Entity owner, bool register = true) : base(owner, register)
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
            AmmoAddOn addOn = Owner as AmmoAddOn;
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
                        AmmoCapacityComponent ammoCapacityComponent = tower.AmmoCapacityComponent;
                        ammoCapacityComponent.AmmoCapacity += (addIncrease) ? 1 : -1;
                        if (ammoCapacityComponent.Ammo > ammoCapacityComponent.AmmoCapacity)
                            ammoCapacityComponent.Ammo = ammoCapacityComponent.AmmoCapacity;
                    }
                }
            }
        }

        private void OnTowerAdded_(object sender, IGameEventArgs e)
        {
            AmmoAddOn addOn = Owner as AmmoAddOn;
            PositionComponent positionComponent = addOn.PositionComponent;
            Vector2 origin = positionComponent.Origin;

            if (sender is List<IPlaceable>)
            {
                List<IPlaceable> objects = sender as List<IPlaceable>;
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
                            AmmoCapacityComponent ammoCapacityComponent = tower.AmmoCapacityComponent;
                            ammoCapacityComponent.AmmoCapacity += 1;
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
                    AmmoCapacityComponent ammoCapacityComponent = tower.AmmoCapacityComponent;
                    ammoCapacityComponent.AmmoCapacity += 1;
                }
            }


        }
    }

}
