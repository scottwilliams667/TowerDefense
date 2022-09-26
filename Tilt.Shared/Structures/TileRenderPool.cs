using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;
using Tilt.EntityComponent.Systems;

namespace Tilt.EntityComponent.Structures
{
    public static class TileRenderPool
    {
        public static List<TileRenderComponent> mAvailable = new List<TileRenderComponent>();
        public static List<TileRenderComponent> mInUse = new List<TileRenderComponent>();
        private static int kTiles = 1024;

        /// <summary>
        /// Create new TileRenders, and Register them with the Layer if they have not been
        /// </summary>
        public static void Init()
        {
            if (mAvailable.Count < kTiles)
            {
                for (int i = mAvailable.Count; i < kTiles; i++)
                {
                    mAvailable.Add(new TileRenderComponent(null));
                }
            }
            foreach (TileRenderComponent tileRender in mAvailable)
            {
                if(!LayerManager.Layer.RenderSystem.Components.Contains(tileRender))
                    tileRender.Register();
            }

            
        }

        public static TileRenderComponent GetObject()
        {
            if (mAvailable.Count == 0)
            {
                TileRenderComponent tileRenderComponent = new TileRenderComponent(null);
                mInUse.Add(tileRenderComponent);
                tileRenderComponent.IsVisible = true;
                return tileRenderComponent;
            }
            else
            {
                TileRenderComponent tileRenderComponent = mAvailable[0];
                mAvailable.Remove(tileRenderComponent);
                mInUse.Add(tileRenderComponent);
                tileRenderComponent.IsVisible = true;
                return tileRenderComponent;;
            }
        }

        public static void ReleaseObject(TileRenderComponent tileRenderComponent)
        {
            tileRenderComponent.Tile = null;
            tileRenderComponent.IsVisible = false;
            mInUse.Remove(tileRenderComponent);
            mAvailable.Add(tileRenderComponent);
        }
    }
}
