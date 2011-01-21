using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sands
{
    class LevelFactory
    {
        public static Level CreateLevelFromMetaLevel(MetaLevel metaLevel, SongOfIce game) {
            if (metaLevel.Id == "oneup") {
                return new MushroomLevel(game, metaLevel);
            }

            return null;
        }

        public static List<MetaLevel> CreateDefaultMetaLevelList()
        {
            List<MetaLevel> levels = new List<MetaLevel>();

            MetaLevel current = new MetaLevel("1-up", "oneup", "1upheart_thumb");
            levels.Add(current);

            current = new MetaLevel("Star Wars", "sw", "questionmark");
            levels.Add(current);

            current = new MetaLevel("First Breath", "fb", "questionmark");
            levels.Add(current);

            current = new MetaLevel("Credits", "cr", "questionmark");
            levels.Add(current);

            return levels;
        }
    }
}
