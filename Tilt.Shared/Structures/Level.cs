using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tilt.EntityComponent.Entities;

namespace Tilt.EntityComponent.Structures
{

    public class Level
    {
        public List<TileCoord> DeadTiles { get; set; }

        public List<TileCoord> SpawnTiles { get; set; }

        public List<List<UnitType>> Units { get; set; }

        public int Number { get; set; }

        public bool Complete { get; set; }

        public int Time { get; set; }

        public float TimeBetweenSpawns { get; set; }

        public uint Food { get; set; }

        public uint Gold { get; set; }

        public uint Minerals { get; set; }

        public string Name { get; set; }

        public string MapNameLeft { get; set; }

        public string MapNameRight { get; set; }

        public TileCoord Base { get; set; }

        public int BaseHealth { get; set; }

        public List<ResourceTile> ResourceTiles { get; set; } 
    }
}
