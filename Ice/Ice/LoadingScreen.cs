using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ice
{
    class LoadingScreen : View
    {
        SpriteBatch batch;
        SpriteFont font28;
        Texture2D flake;

        float flakeRotation;

        public LoadingScreen(SongOfIce game)
            : base(game)
        {
            flakeRotation = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            flakeRotation += gameTime.ElapsedGameTime.Milliseconds * (float)Math.PI / 2000f;
        }


        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            batch.Begin();

            int flakeWidth  = 64;
            int flakeHeight = 64;

            int width = game.GraphicsDevice.Viewport.Width;
            int height = game.GraphicsDevice.Viewport.Height;

            Vector2 measurement = font28.MeasureString("Loading...");

            Rectangle center = new Rectangle((width) / 2, (height) / 2, flakeWidth, flakeHeight);
            Vector2 textLocation = new Vector2((width - measurement.X) / 2, (height + flakeHeight) / 2 + 15);

            batch.Draw(flake, center, null, Color.White, flakeRotation, new Vector2(flakeWidth / 2, flakeWidth / 2), SpriteEffects.None, 0);
            batch.DrawString(font28, "Loading...", textLocation, Color.White);

            batch.End();
        }

        public override void Load()
        {
            flakeRotation = 0;
            batch = new SpriteBatch(game.GraphicsDevice);
            font28 = content.Load<SpriteFont>("Font28");
            flake = content.Load<Texture2D>("flake0");
        }

        public override void Unload()
        {
            flake.Dispose();
        }
    }
}
