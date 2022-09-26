using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Systems;
using Tilt.EntityComponent.Utilities;

namespace Tilt.EntityComponent.Structures
{
    [Flags]
    public enum LayerType
    {
        Game = 0x1,
        Hud = 0x2,
        GameMenuOverlay = 0x4,
        LevelRecap = 0x8,
        LevelSelect = 0x10,
        StartMenu = 0x20,
        TowerSelect = 0x40,
        TowerUpgrade = 0x80,
        GameOver = 0x100,
        Info = 0x200,
        WorldMap = 0x400,
        Credits = 0x800
    }


    [Flags]
    public enum LayerCaps
    {
        Render = 0x1,
        Position = 0x2,
        Collision = 0x4,
        //Audio = 0x8,
        Touch = 0x10,
        Entity = 0x20,
        Time = 0x40,

        All = Render | Position | Collision | 
            Touch | Entity | Time

    }


    public class Layer
    {
        private EntitySystem mEntitySystem;
        private CollisionSystem mCollisionSystem;
        private PositionSystem mPositionSystem;
        private RenderSystem mRenderSystem;
        private InputSystem mInputSystem;
        private TimeSystem mTimeSystem;
        private Matrix mMatrix;
        private BlendState mBlendState;

        private LayerCaps mCaps = LayerCaps.All;
        private LayerType mLayerType;

        public Layer(LayerType layerType) : this()
        {
            mLayerType = layerType;
        }

        public Layer()
        {
            mEntitySystem = new EntitySystem();
            mCollisionSystem = new CollisionSystem();
            mPositionSystem = new PositionSystem();
            mRenderSystem = new RenderSystem();
            mInputSystem = new InputSystem();
            mTimeSystem = new TimeSystem();
            mMatrix = Matrix.CreateTranslation(0, 0, 0);
        }

        public EntitySystem EntitySystem
        {
            get { return mEntitySystem; }
        }
        
        public CollisionSystem CollisionSystem
        {
            get { return mCollisionSystem; }
        }

        public PositionSystem PositionSystem
        {
            get { return mPositionSystem; }
        }

        public RenderSystem RenderSystem
        {
            get { return mRenderSystem; }
        }

        public InputSystem InputSystem
        {
            get { return mInputSystem; }
        }

        public TimeSystem TimeSystem
        {
            get { return mTimeSystem;}
        }

        public LayerCaps Caps
        {
            get { return mCaps; }
            set { mCaps = value; }
        }

        public LayerType Type
        {
            get { return mLayerType; }
            set { mLayerType = value; }
        }

        public Matrix Matrix
        {
            get { return mMatrix; } 
            set { mMatrix = value; }
        }

        public BlendState BlendState
        {
            get { return mBlendState;}
            set { mBlendState = value; }
        }

        public void Update()
        {
            if (Caps.HasFlag(LayerCaps.Collision))
                CollisionSystem.Update();
            if (Caps.HasFlag(LayerCaps.Position))
                PositionSystem.Update();
            if (Caps.HasFlag(LayerCaps.Touch))
                InputSystem.Update();
            if (Caps.HasFlag(LayerCaps.Time))
                TimeSystem.Update();
        }

        public void Draw()
        {

            if (Caps.HasFlag(LayerCaps.Render))
                RenderSystem.Update();
        }

    }
}
