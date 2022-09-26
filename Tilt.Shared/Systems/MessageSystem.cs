using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Structures;
using Tilt.EntityComponent.Systems;

namespace Tilt.Shared.Systems
{
    public interface IMessage
    {
    }

    public interface IMessageable
    {
        void RecieveMessage(IMessage message);
    }

    public class Message<T> : IMessage
    {
        public T Value {get;set;}

        public ulong FromEntityId {get;set;}

        public ulong ToEntityId {get;set;}
   
    }

    public static class MessageSystem 
    {
        public static void PassMessage<T>(ulong fromEntityId, ulong toEntityId, T value)
        {
            Message<T> message = new Message<T>()
            {
                FromEntityId = fromEntityId,
                ToEntityId = toEntityId,
                Value = value
            };

            //expensive
            Entity entity = LayerManager.Layers.SelectMany(l => l.EntitySystem.Entities).FirstOrDefault(e => e.Id == toEntityId);

            if(entity is IMessageable)
            {
                IMessageable messageable = entity as IMessageable;
                messageable.RecieveMessage(message);
            }
            
        }

        public static void PassMessage<T>(LayerType layerType, ulong fromEntityId, ulong toEntityId, T value)
        {
            Message<T> message = new Message<T>()
            {
                FromEntityId = fromEntityId,
                ToEntityId = toEntityId,
                Value = value
            };

            Layer layer = LayerManager.GetLayer(layerType);

            Entity entity = layer.EntitySystem.GetEntityById(fromEntityId);

            if(entity is IMessageable)
            {
                IMessageable messageable = entity as IMessageable;
                messageable.RecieveMessage(message);
            }

        }

    }
}
