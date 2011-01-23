using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Media;

namespace Ice
{
    abstract class Level : View
    {
        protected MetaLevel metaLevel;
        protected Song music;

        public Level(SongOfIce game, MetaLevel meta)
            : base(game)
        {
            metaLevel = meta;
        }
    }
}
