using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tilt.EntityComponent.Entities;
using Tilt.EntityComponent.Utilities;
using Tilt.Shared.Structures;

namespace Tilt.EntityComponent.Structures
{
    public class Mob
    {
        private Queue<UnitType> mQueue;

        public Mob(List<UnitType> mUnits)
        {
            mQueue = new Queue<UnitType>(mUnits);
        }

        public Unit Spawn(TileCoord spawnTile)
        {
            if (IsEmpty())
                return null;

            Vector2 spawn = GeometryOps.TileCoordToPosition(spawnTile);
            UnitType unitType = mQueue.Dequeue();
            Unit unit = UnitFactory.Make(unitType, (int)spawn.X, (int)spawn.Y, 0.0f, "creatureanimation");

            return unit;
        }

        public bool IsEmpty()
        {
            return mQueue.Count == 0;
        }
    }
}
