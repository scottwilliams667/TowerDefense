using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Systems
{
    public class EntitySystem
    {
        private List<Entity> mEntities = new List<Entity>();

        public void Register(Entity entity)
        {
            mEntities.Add(entity);
        }

        public void UnRegister(Entity entity)
        {
            mEntities.Remove(entity);
        }

        public void UnRegisterAll()
        {
            foreach(Entity entity in mEntities.ToList())
                entity.UnRegister();
        }

        public Entity GetEntityById(ulong id)
        {
            return mEntities.FirstOrDefault(e => e.Id == id);
        }

        public List<T> GetEntitiesByType<T>()
        {
            return mEntities.Where(e => e is T).Cast<T>().ToList<T>();
        }

        public List<Entity> Entities
        {
            get { return mEntities; }
            set { mEntities = value; }
        }

    }
}
