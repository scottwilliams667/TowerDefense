using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.Shared.Components
{
    public class GeometryRenderComponent : Component
    {
        private LayerType mRegisteredLayer;

        private int mSides;
        private Color mColor;

        public GeometryRenderComponent(int sides, Color color, Entity owner, bool register = true) : base(owner, register)
        {
            mSides = sides;
            mColor = color;
        }

        public override void Register()
        {
            mRegisteredLayer = LayerManager.Layer.Type;
            LayerManager.Layer.RenderSystem.Register(this);
        }

        public override void UnRegister()
        {
            LayerManager.GetLayer(mRegisteredLayer).RenderSystem.UnRegister(this);
        }

        public int Sides { get {return  mSides;} }

        public Color Color { get { return mColor;  } }
    }

    public class CircleRenderComponent : GeometryRenderComponent
    {
        public CircleRenderComponent(int sides, Color color, Entity owner, bool register = true) : base(sides, color, owner, register)
        {
        }

        public virtual void Render(Vector2 position, float radius)
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();

            Layer layer = LayerManager.GetLayerOfEntity(Owner);

            //End the draw call to shove the Circle on top of everything

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);

            spriteBatch.DrawCircle(position, radius, Sides, Color);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, layer.Matrix);
        }

    }
    
}
