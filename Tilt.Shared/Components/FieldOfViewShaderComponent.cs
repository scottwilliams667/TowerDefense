using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Components;

namespace Tilt.EntityComponent.Components
{
    /// <summary>
    /// SHADERS DONT COMPILE PROPERLY ON IOS. THIS CLASS IS DEAD
    /// </summary>
    public class FieldOfViewShaderComponent : ShaderComponent
    {
        private RenderTarget2D renderTarget;

        private Texture2D mSelc;
        private Texture2D mSelBc;
        private Texture2D mSelR;
        private const int kTexturePadding = 0;
        private float mFieldOfView = 0;

        public FieldOfViewShaderComponent(string shaderName, float fieldOfView, Entity owner, bool register = true)
            : base(shaderName, owner, register)
        {
            mFieldOfView = fieldOfView;

            mSelc = AssetOps.LoadSharedAsset<Texture2D>("selectionsmall");
            mSelBc = AssetOps.LoadSharedAsset<Texture2D>("selectionbig");
            mSelR = AssetOps.LoadSharedAsset<Texture2D>("selectionrectangle");
        }

        public float FieldOfView
        {
            get { return mFieldOfView; }
        }

        public override void Update()
        {
            if (TileMap.SelectedTile == null || TileMap.SelectedTile.Object != Owner)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();

            IPlaceable placeable = Owner as IPlaceable;
            Layer layer = LayerManager.GetLayerOfEntity(Owner);


            spriteBatch.End();

            Texture2D texture = new Texture2D(graphicsDevice, (int) (2 * mFieldOfView) + kTexturePadding,
                (int) (2 * mFieldOfView) + kTexturePadding);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

            mEffect.Parameters["TowerFOV"].SetValue(mFieldOfView);
            mEffect.Parameters["CenterX"].SetValue(texture.Width / 2);
            mEffect.Parameters["CenterY"].SetValue(texture.Height / 2);
            mEffect.Parameters["Width"].SetValue(texture.Width);
            mEffect.Parameters["Height"].SetValue(texture.Height);

            mEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(texture,
                placeable.PositionComponent.Position +
                new Vector2(TileMap.TileWidth / 2 - 2, TileMap.TileHeight / 2 - 2),
                null, Color.White, 0.0f, new Vector2(texture.Width / 2, texture.Height / 2), 1.0f,
                SpriteEffects.None, 0.25f);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

            if (!(placeable is Tower))
                return;

            

            List<RangeBoosterAddOn> nearbyRangeBoosters = new List<RangeBoosterAddOn>();
            List<CooldownAddOn> nearbyCooldownAddOns = new List<CooldownAddOn>();
            List<AmmoAddOn> nearbyAmmoAddOns = new List<AmmoAddOn>();
            List<ShieldGenerator> nearbyShieldGenerators = new List<ShieldGenerator>();
            List<RefineryAddOn> nearbyRefineryAddOns = new List<RefineryAddOn>();

            ICollideable collideable = Owner as ICollideable;


            if (collideable != null)
            {
                BoundsCollisionComponent collisionComponent = collideable.BoundsCollisionComponent;
                List<int> surroundingCells =
                    CollisionHelper.GetSurroundingCells(collisionComponent.Cells.FirstOrDefault());

                foreach (int surroundingCell in surroundingCells)
                {
                    List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(surroundingCell);
                    List<RangeBoosterAddOn> rangeBoosters = nearbyComponents.Where(c => c.Owner is RangeBoosterAddOn).Select(c => c.Owner).Cast<RangeBoosterAddOn>().ToList();
                    foreach (RangeBoosterAddOn rangeBooster in rangeBoosters)
                    {
                        if ( Vector2.Distance(placeable.PositionComponent.Position, rangeBooster.PositionComponent.Position) < (rangeBooster.Data as AddOnData).FieldOfView)
                        {
                            nearbyRangeBoosters.Add(rangeBooster);
                        }
                    }
                    List<CooldownAddOn> cooldownAddOns = nearbyComponents.Where(c => c.Owner is CooldownAddOn).Select(c => c.Owner).Cast<CooldownAddOn>().ToList();
                    foreach (CooldownAddOn cooldownAddOn in cooldownAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position,cooldownAddOn.PositionComponent.Position) < (cooldownAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyCooldownAddOns.Add(cooldownAddOn);
                        }
                    }

                    List<AmmoAddOn> ammoAddOns = nearbyComponents.Where(c => c.Owner is AmmoAddOn).Select(c => c.Owner).Cast<AmmoAddOn>().ToList();
                    foreach (AmmoAddOn ammoAddOn in ammoAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, ammoAddOn.PositionComponent.Position) < (ammoAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyAmmoAddOns.Add(ammoAddOn);
                        }
                    }
                    List<ShieldGenerator> shieldGenerators = nearbyComponents.Where(c => c.Owner is ShieldGenerator).Select(c => c.Owner).Cast<ShieldGenerator>().ToList();
                    foreach (ShieldGenerator shieldGenerator in shieldGenerators)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, shieldGenerator.PositionComponent.Position) < (shieldGenerator.Data as ShieldGeneratorData).FieldOfView)
                        {
                            nearbyShieldGenerators.Add(shieldGenerator);
                        }
                    }

                    List<RefineryAddOn> refineryAddOns = nearbyComponents.Where(c => c.Owner is RefineryAddOn).Select(c => c.Owner).Cast<RefineryAddOn>().ToList();
                    foreach (RefineryAddOn refineryAddOn in refineryAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, refineryAddOn.PositionComponent.Position) < (refineryAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyRefineryAddOns.Add(refineryAddOn);
                        }
                    }


                    List<IPlaceable> nearbyObjects = new List<IPlaceable>();
                    nearbyObjects.AddRange(nearbyAmmoAddOns);
                    nearbyObjects.AddRange(nearbyCooldownAddOns);
                    nearbyObjects.AddRange(nearbyRangeBoosters);
                    nearbyObjects.AddRange(nearbyShieldGenerators);
                    nearbyObjects.AddRange(nearbyRefineryAddOns);


                    foreach (IPlaceable place in nearbyObjects)
                    {
                        PositionComponent placePosition = place.PositionComponent;
                        PositionComponent selectedObjPosition = placeable.PositionComponent;

                        double angle = GeometryOps.AngleBetweenTwoVectors(placePosition.Position, selectedObjPosition.Position);

                        spriteBatch.Draw(mSelc, selectedObjPosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2),null,
                                Color.White, 0.0f, new Vector2(mSelc.Width / 2, mSelc.Height / 2), 1.0f, SpriteEffects.None, 0.4f);

                        spriteBatch.Draw(mSelBc, placePosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), null,
                                Color.White, 0.0f, new Vector2(mSelBc.Width / 2, mSelBc.Height / 2), 1.0f, SpriteEffects.None, 0.4f);

                        double distance = Vector2.Distance(placePosition.Position, selectedObjPosition.Position);

                        decimal sizeX = Decimal.Divide((decimal)distance, mSelR.Width);

                        spriteBatch.Draw(mSelR, selectedObjPosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), null,
                            Color.White, (float)angle, new Vector2(0, mSelR.Height / 2), new Vector2((float)sizeX, 1.0f),  SpriteEffects.None, 0.4f);



                    }
                }
            }
        }
    }

    public class FieldOfViewRenderComponent : CircleRenderComponent
    {
        private Texture2D mSelc;
        private Texture2D mSelBc;
        private Texture2D mSelR;

        public FieldOfViewRenderComponent(int sides, Color color, Entity owner, bool register = true) : base(sides, color, owner, register)
        {
            mSelc = AssetOps.LoadSharedAsset<Texture2D>("selectionsmall");
            mSelBc = AssetOps.LoadSharedAsset<Texture2D>("selectionbig");
            mSelR = AssetOps.LoadSharedAsset<Texture2D>("selectionrectangle");
        }

        public override void Update()
        {
            if (TileMap.SelectedTile == null || TileMap.SelectedTile.Object != Owner)
                return;

            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GraphicsDevice graphicsDevice = ServiceLocator.GetService<GraphicsDevice>();

            IPlaceable placeable = Owner as IPlaceable;
            Layer layer = LayerManager.GetLayerOfEntity(Owner);

            IFieldOfView data = placeable.Data as IFieldOfView;
            
            Render(placeable.PositionComponent.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), data.FieldOfView);

            if (!(placeable is Tower))
                return;




            List<RangeBoosterAddOn> nearbyRangeBoosters = new List<RangeBoosterAddOn>();
            List<CooldownAddOn> nearbyCooldownAddOns = new List<CooldownAddOn>();
            List<AmmoAddOn> nearbyAmmoAddOns = new List<AmmoAddOn>();
            List<ShieldGenerator> nearbyShieldGenerators = new List<ShieldGenerator>();
            List<RefineryAddOn> nearbyRefineryAddOns = new List<RefineryAddOn>();

            ICollideable collideable = Owner as ICollideable;


            if (collideable != null)
            {
                BoundsCollisionComponent collisionComponent = collideable.BoundsCollisionComponent;
                List<int> surroundingCells =
                    CollisionHelper.GetSurroundingCells(collisionComponent.Cells.FirstOrDefault());

                foreach (int surroundingCell in surroundingCells)
                {
                    List<CollisionComponent> nearbyComponents = CollisionHelper.GetNearby(surroundingCell);
                    List<RangeBoosterAddOn> rangeBoosters = nearbyComponents.Where(c => c.Owner is RangeBoosterAddOn).Select(c => c.Owner).Cast<RangeBoosterAddOn>().ToList();
                    foreach (RangeBoosterAddOn rangeBooster in rangeBoosters)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, rangeBooster.PositionComponent.Position) < (rangeBooster.Data as AddOnData).FieldOfView)
                        {
                            nearbyRangeBoosters.Add(rangeBooster);
                        }
                    }
                    List<CooldownAddOn> cooldownAddOns = nearbyComponents.Where(c => c.Owner is CooldownAddOn).Select(c => c.Owner).Cast<CooldownAddOn>().ToList();
                    foreach (CooldownAddOn cooldownAddOn in cooldownAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, cooldownAddOn.PositionComponent.Position) < (cooldownAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyCooldownAddOns.Add(cooldownAddOn);
                        }
                    }

                    List<AmmoAddOn> ammoAddOns = nearbyComponents.Where(c => c.Owner is AmmoAddOn).Select(c => c.Owner).Cast<AmmoAddOn>().ToList();
                    foreach (AmmoAddOn ammoAddOn in ammoAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, ammoAddOn.PositionComponent.Position) < (ammoAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyAmmoAddOns.Add(ammoAddOn);
                        }
                    }
                    List<ShieldGenerator> shieldGenerators = nearbyComponents.Where(c => c.Owner is ShieldGenerator).Select(c => c.Owner).Cast<ShieldGenerator>().ToList();
                    foreach (ShieldGenerator shieldGenerator in shieldGenerators)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, shieldGenerator.PositionComponent.Position) < (shieldGenerator.Data as ShieldGeneratorData).FieldOfView)
                        {
                            nearbyShieldGenerators.Add(shieldGenerator);
                        }
                    }

                    List<RefineryAddOn> refineryAddOns = nearbyComponents.Where(c => c.Owner is RefineryAddOn).Select(c => c.Owner).Cast<RefineryAddOn>().ToList();
                    foreach (RefineryAddOn refineryAddOn in refineryAddOns)
                    {
                        if (Vector2.Distance(placeable.PositionComponent.Position, refineryAddOn.PositionComponent.Position) < (refineryAddOn.Data as AddOnData).FieldOfView)
                        {
                            nearbyRefineryAddOns.Add(refineryAddOn);
                        }
                    }


                    List<IPlaceable> nearbyObjects = new List<IPlaceable>();
                    nearbyObjects.AddRange(nearbyAmmoAddOns);
                    nearbyObjects.AddRange(nearbyCooldownAddOns);
                    nearbyObjects.AddRange(nearbyRangeBoosters);
                    nearbyObjects.AddRange(nearbyShieldGenerators);
                    nearbyObjects.AddRange(nearbyRefineryAddOns);


                    foreach (IPlaceable place in nearbyObjects)
                    {
                        PositionComponent placePosition = place.PositionComponent;
                        PositionComponent selectedObjPosition = placeable.PositionComponent;

                        double angle = GeometryOps.AngleBetweenTwoVectors(placePosition.Position, selectedObjPosition.Position);

                        spriteBatch.Draw(mSelc, selectedObjPosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), null,
                                Color.White * 0.01f, 0.0f, new Vector2(mSelc.Width / 2, mSelc.Height / 2), 1.0f, SpriteEffects.None, 0.38f);

                        spriteBatch.Draw(mSelBc, placePosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), null,
                                Color.White * 0.01f, 0.0f, new Vector2(mSelBc.Width / 2, mSelBc.Height / 2), 1.0f, SpriteEffects.None, 0.39f);

                        double distance = Vector2.Distance(placePosition.Position, selectedObjPosition.Position);

                        decimal sizeX = Decimal.Divide((decimal)distance, mSelR.Width);

                        spriteBatch.Draw(mSelR, selectedObjPosition.Position + new Vector2(TileMap.TileWidth / 2, TileMap.TileHeight / 2), null,
                            Color.White * 0.04f, (float)angle, new Vector2(0, mSelR.Height / 2), new Vector2((float)sizeX, 1.0f), SpriteEffects.None, 0.4f);



                    }
                }
            }

        }
    }

}
