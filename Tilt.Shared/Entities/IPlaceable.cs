using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Components;

namespace Tilt.EntityComponent.Entities
{
    public interface IPlaceable
    {
        ObjectType ObjectType { get; }

        IData Data { get; set; }

        PositionComponent PositionComponent { get; set; }
    }

    public interface IBuildable
    {
        bool Enabled { get; set; }
    }

    public interface ICollideable
    {
        BoundsCollisionComponent BoundsCollisionComponent { get; set; }
    }

    public interface IShieldable
    {
        ShieldRenderComponent ShieldRenderComponent { get; set; }
    }

    public interface IHealable
    {
        HealthRenderComponent HealthRenderComponent { get; set; }

        HealthComponent HealthComponent { get; set; }
    }

    public interface IDamageable
    {
        bool ShowSmokeDamage { get; set; }
    }
}
