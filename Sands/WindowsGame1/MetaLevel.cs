using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sands
{
    public enum LevelState {
        KNOWN,
        UNKNOWN,
        LOCKED
    }

    class MetaLevel
    {
        public String Title { get { return title; } set { title = value; } }
        public String Id { get { return id; } set { id = value; } }
        public String ThumbnailResource { get { return thumbnailResource; } set { thumbnailResource = value; } }
        public LevelState State { get { return state; } set { state = value; } }

        public MetaLevel(String title, String id, String thumbnail)
        {
            this.title = title;
            this.thumbnailResource = thumbnail;
            this.state = LevelState.LOCKED;
            this.id = id;
        }

        private String title;
        private String id;
        private String thumbnailResource;
        private LevelState state;
    }
}
