using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;


namespace Sands
{
    abstract class MenuHelper
    {
        public MenuHelper(Game game)
        {
            music = null;
            playing = false;
            this.game = game;
        }
        public abstract void Load(ContentManager content);
        public abstract void Unload(ContentManager content);

        public virtual void Update(GameTime gameTime) {}
        public abstract void Draw(GraphicsDevice device, GameTime gameTime);

        public virtual void Play()
        {
            if (music != null)
            {
                playing = true;
                MediaPlayer.Volume = .2f;
                MediaPlayer.Play(music);
            }
        }

        public virtual void Stop()
        {
            MediaPlayer.Stop();
            playing = false;
        }

        protected Song music;
        protected bool playing;
        protected Game game;
    }
}
