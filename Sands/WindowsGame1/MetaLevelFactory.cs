using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sands
{
    class MetaLevelFactory
    {
        public static List<MetaLevel> CreateDefaultMetaLevelList()
        {
            List<MetaLevel> levels = new List<MetaLevel>();

            MetaLevel current = new MetaLevel("Star Wars", "sw", "swlevel_thumb");
            levels.Add(current);

            current = new MetaLevel("First Breath", "fb", "fblevel_thumb");
            levels.Add(current);

            current = new MetaLevel("1-up", "oneup", "1uplevel_thumb");
            levels.Add(current);

            current = new MetaLevel("Credits", "cr", "crlevel_thumb");
            levels.Add(current);

            return levels;
        }
    }
}
