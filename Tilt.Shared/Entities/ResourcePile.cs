using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Entities
{
    public class ResourcePile 
        : Entity
        , IPlaceable
    {
        private IData mData;
        private PositionComponent mPositionComponent;

        public ObjectType ObjectType { get {return ObjectType.ResourcePile; } }

        public IData Data
        {
            get { return mData; }
            set { mData = value; }
        }
        public PositionComponent PositionComponent
        {
            get {return mPositionComponent;}
            set { mPositionComponent = value; }
        }

        public override void UnRegister()
        {
            mPositionComponent.UnRegister();
            mData = null;
        }
    }

    public class ResourceRenderComponent : RenderComponent
    {
        private Rectangle mSourceRectangle;

        public ResourceRenderComponent(string texturePath, Rectangle sourceRectangle, Entity owner, bool register = true) : base(texturePath, owner, register)
        {
            mSourceRectangle = sourceRectangle;
        }

        public override void Update()
        {
            SpriteBatch spriteBatch = ServiceLocator.GetService<SpriteBatch>();
            GoldPile goldPile = Owner as GoldPile;
            PositionComponent positionComponent = goldPile.PositionComponent;

            spriteBatch.Draw(mTexture, positionComponent.Position, mSourceRectangle, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.2f);
        }
    }

    public class GoldPile : ResourcePile
    {
        private ResourceRenderComponent mResourceRenderComponent;

        //parse from json
        //dead tile in tilemap
        //make sure no building can be placed on
        //resource loader has to be placed next to it

        public GoldPile(string texturePath, Rectangle sourceRectangle, int x, int y, IData pileData)
        {
            mResourceRenderComponent = new ResourceRenderComponent(texturePath, sourceRectangle, this);
            PositionComponent = new PositionComponent(x,y, this);
            Data = pileData;
        }

        public override void UnRegister()
        {
            mResourceRenderComponent.UnRegister();
        }
    }
}
