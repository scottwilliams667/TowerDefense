using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;

namespace Tilt.EntityComponent.Systems
{
    public static class LayerManager
    {
        private static Dictionary<LayerType, Layer> mLayerCache = new Dictionary<LayerType, Layer>();
        private static Stack<Layer> mLayers = new Stack<Layer>();
        private static Layer mLayer;
        
        public static bool Push(LayerType layerType, bool loadFromCache = false)
        {
            bool loadedFromCache = true;
            Layer layer = null;
            if (loadFromCache)
                mLayerCache.TryGetValue(layerType, out layer);


            if (layer == null)
            {
                layer = new Layer(layerType);
                loadedFromCache = !loadedFromCache;
            }
            mLayer = layer;

            mLayers.Push(mLayer);

            return loadedFromCache;
        }

        public static void Push(Layer layer)
        {
            mLayer = layer;
            mLayers.Push(layer);
        }

        public static LayerType Peek()
        {
            return mLayers.Peek().Type;
        }

        public static Layer GetLayer(int layerIndex)
        {
            return mLayers.ToList().ElementAt(layerIndex);
        }

        public static Layer GetLayer(LayerType layerType)
        {
            return mLayers.FirstOrDefault(l => l.Type == layerType);
        }

        public static Layer GetLayerOfEntity(Entity entity)
        {
            return mLayers.FirstOrDefault(l => l.EntitySystem.Entities.Contains(entity));
        }

        public static bool HasLayer(LayerType layerType)
        {
            return mLayers.FirstOrDefault(l => l.Type == layerType) != null;
        }

        public static void SetLayer(LayerType layerType)
        {
            Layer layer = mLayers.FirstOrDefault(l => l.Type == layerType);
            if (layer == null)
            {
                layer = new Layer(layerType);
                mLayers.Push(layer);
            }
            mLayer = layer;

        }

        public static void SetLayer(int layerIndex)
        {
            Layer layer = mLayers.ToList().ElementAt(layerIndex);
            if (layer != null)
                mLayer = layer;
        }

        public static void SetLayer(Layer layer)
        {
            mLayer = layer;
        }

        public static Layer Pop(bool saveToCache = false, bool clearEvents = false)
        {
            Layer layer = mLayers.Pop();
            if (saveToCache)
                mLayerCache[layer.Type] = layer;
            else
            {
                layer.EntitySystem.UnRegisterAll();
            }

            if(clearEvents)
                EventSystem.Clear();
            
            mLayer = (mLayers.Count > 0) ? mLayers.Peek() : null;

            return layer;
        }

        public static void TransitionTo(LayerType layerType, bool loadFromCache = false, bool saveToCache = false)
        {

            if (saveToCache)
            {
                foreach (Layer ly in mLayers.ToList())
                {
                    mLayerCache[ly.Type] = ly;
                }
            }

            mLayers.Clear();
            mLayer = null;

            EventSystem.Clear();

            Layer layer = null;
            if (loadFromCache)
                mLayerCache.TryGetValue(layerType, out layer);
            else
            {
                layer = new Layer(layerType);
            }

            mLayers.Push(layer);
            mLayer = layer;

        }

        public static bool SaveLayer(Layer layer)
        {
            try
            {
                mLayerCache[layer.Type] = layer;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static Layer Layer
        {
            get { return mLayer; }
            set { mLayer = value; }
        }

        public static Stack<Layer> Layers
        {
            get { return mLayers; }
        }
    }
}
