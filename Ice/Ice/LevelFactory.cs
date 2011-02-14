using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ice
{
    class LevelFactory
    {
        public static Level CreateLevelFromMetaLevel(MetaLevel metaLevel, SongOfIce game) {
            if (metaLevel.Id == "oneup") {
                return new MushroomLevel(game, metaLevel);
            }

            if (metaLevel.Id == "tr") {
                return new TronLevel(game, metaLevel);
            }

            if (metaLevel.Id == "cr") {
                return new CreditsLevel(game, metaLevel);
            }

            return null;
        }

        public static List<MetaLevel> CreateDefaultMetaLevelList()
        {
            List<MetaLevel> levels = new List<MetaLevel>();

            MetaLevel current = new MetaLevel("Tron", "tr", "trondaftpunk_thumb");
            levels.Add(current);

            current = new MetaLevel("1-up", "oneup", "1upheart_thumb");
            levels.Add(current);

            //current = new MetaLevel("Star Wars", "sw", "questionmark");
            //levels.Add(current);

            current = new MetaLevel("Credits", "cr", "credits_thumb");
            levels.Add(current);

            return levels;
        }
    }
}
