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

        public override bool Equals(object obj)
        {
            if (this == obj) {
                return true;
            }

            if (obj is MetaLevel) {
                MetaLevel mlObject = (MetaLevel)obj;
                return Equals(mlObject);
            } else if (obj is String) {
                String sObject = (String)obj;
                return Equals(sObject);
            }

            return false;
        }

        private bool Equals(String id)
        {
            return (this.Id == id);
        }

        private bool Equals(MetaLevel other)
        {
            return (this.Id == other.Id);
        }

        private String title;
        private String id;
        private String thumbnailResource;
        private LevelState state;
    }
}
