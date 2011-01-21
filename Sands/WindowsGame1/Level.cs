using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sands
{
    abstract class Level : View
    {
        protected MetaLevel metaLevel;
        public Level(SandsGame game, MetaLevel meta)
            : base(game)
        {
            metaLevel = meta;
        }
    }
}
