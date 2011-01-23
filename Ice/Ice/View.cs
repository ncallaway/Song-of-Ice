using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ice
{
    abstract class View
    {
        public bool Sound
        {
            set
            {
                if (sound == value) { return; }
                if (!sound) {
                    sound = value;
                    SoundStarted();
                } else if (sound) {
                    sound = value;
                    SoundStopped();
                }
            }
            get { return sound; }
        }

        public View(SongOfIce game)
        {
            content = new ContentManager(game.Services, "Content");
            this.game = game;
        }

        public virtual void Update(GameTime gameTime) {}
        public abstract void Draw(GameTime gameTime);

        public abstract void Load();
        public abstract void Unload();

        protected virtual void SoundStarted() { }
        protected virtual void SoundStopped() { }

        protected ContentManager content;
        protected SongOfIce game;
        protected bool sound;
    }
}
