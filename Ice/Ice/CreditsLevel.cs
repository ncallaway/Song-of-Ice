using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Ice
{
    class CreditsLevel : Level 
    {
        SpriteBatch batch;
        SpriteFont font36;
        SpriteFont font28;
        SpriteFont font24;
        SpriteFont font16;
        SpriteFont font16italic;


        float introTime = 5000f;
        float fadeTime = 2000f;
        float hangTime = 1000f;
        float levelWinTime = 79000f;

        float vertical_base = 0;
        bool started;

        int halfScreen;
        int fullScreen;

        public CreditsLevel(SongOfIce game, MetaLevel meta) : base(game, meta) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (TotalLevelTime(gameTime).TotalMilliseconds > introTime) {
                vertical_base += gameTime.ElapsedGameTime.Milliseconds / 10;
            }

            if (TotalLevelTime(gameTime).TotalMilliseconds > levelWinTime) {
                game.UnlockLevel(metaLevel);
                SwitchToNextLevel();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            batch.Begin();

            DrawTitle(gameTime);

            DrawNoahBlock();

            DrawMusicBlock();

            if (vertical_base > 2696) {
                DrawHosannaBlock();
            }

            batch.End();
        }

        private void DrawHosannaBlock()
        {
            String title = "Dedicated to Hosanna";
            SpriteFont titleFont = font36;
            Vector2 position;
            position.X = XPositionForScreenCentered(titleFont, title);
            position.Y = halfScreen;
            batch.DrawString(titleFont, title, position, Color.White);
        }

        private void DrawTitle(GameTime gameTime)
        {

            if (vertical_base > fullScreen + halfScreen) { return; }

            Vector2 position;

            float aValue = (float)TotalLevelTime(gameTime).TotalMilliseconds / fadeTime;
            
            if (aValue > 1) {
                aValue = ((float)TotalLevelTime(gameTime).TotalMilliseconds - hangTime - fadeTime) / fadeTime;
                if (aValue < 0) {
                    aValue = 1;
                } else {
                    aValue = 1 - aValue;
                }
            }

            if (aValue < 0) { return; }

            Color fade = new Color(Color.White, (byte)(255f * aValue));

            String title = "Song of Ice";
            SpriteFont titleFont = font36;
            position.X = XPositionForScreenCentered(titleFont, title);
            position.Y = halfScreen - vertical_base;
            batch.DrawString(titleFont, title, position, fade);

            title = "by Noah Callaway";
            titleFont = font16;
            position.X = XPositionForScreenCentered(titleFont, title);
            position.Y += 50;
            batch.DrawString(titleFont, title, position, fade);
        }

        private void DrawNoahBlock()
        {
            /* Lead credit writer */
            String text = "Lead Credit Writer";
            SpriteFont font = font28;
            Vector2 position;

            position.X = XPositionForScreenCentered(font, text);
            position.Y = fullScreen + 15 - vertical_base;
            batch.DrawString(font, text, position, Color.White);

            text = "Noah Callaway";
            font = font16;
            position.X = XPositionForScreenCentered(font, text);
            position.Y += 30;
            batch.DrawString(font, text, position, Color.White);

            /* Assistant credit writers */
            text = "Assistant Credit Writers";
            font = font24;
            position.X = XPositionForScreenCentered(font, text);
            position.Y += 60;
            batch.DrawString(font, text, position, Color.White);

            text = "Noah Callaway";
            font = font16;
            position.X = XPositionForScreenCentered(font, text);
            position.Y += 25;
            batch.DrawString(font, text, position, Color.White);

            /* Programming / Design */
            position.Y += 100;
            font = font24;

            text = "Programming";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Design";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 25;
            font = font16;
            text = "Noah Callaway";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Also Noah";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* Some Art / Other Misc Jobs */
            position.Y += 50;
            font = font24;

            text = "Some Art";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Other Misc Jobs";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 25;
            font = font16;
            text = "Still Noah...";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Really just me!";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* Vector Restitution / Best Boy Grip */
            position.Y += 50;
            font = font24;

            text = "Vector Restitution";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Best Boy Grip";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 25;
            font = font16;
            text = "Me";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Is that really a job?";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* Vector Restitution / Best Boy Grip */
            position.Y += 50;
            font = font24;

            text = "Lead Technical Documentor";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Lady Gaga Impersonator";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 25;
            font = font16;
            text = "Some guy named \"Joseph\"";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Lady Gaga";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
        }

        private void DrawMusicBlock()
        {
            /* Attributions */
            String text = "Some Actual Attributions";
            SpriteFont font = font36;
            Vector2 position;

            position.X = XPositionForScreenCentered(font, text);
            position.Y = fullScreen + fullScreen - vertical_base;
            batch.DrawString(font, text, position, Color.White);

            position.Y += halfScreen;

            text = "music";
            font = font28;
            position.X = XPositionForScreenCentered(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* MUSIC! menu / mushroom level*/
            position.Y += 50;
            font = font24;
            text = "Menu";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Tron Level";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16italic;
            text = "First Breath After A Coma";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Disc Wars";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 18;
            font = font16;
            text = "Explosions in the Sky";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Tron Soundtrack - Daft Punk";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* 1 - Up Level / Credits */
            position.Y += 50;
            font = font24;
            text = "1-up Level";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Credits";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16italic;
            text = "Super Mario Bros. 2";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Son of Flynn";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 18;
            font = font16;
            text = "The Minibosses";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Tron Soundtrack - Daft Punk";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 100;
            text = "images";
            font = font28;
            position.X = XPositionForScreenCentered(font, text);
            batch.DrawString(font, text, position, Color.White);

            /* Menu */

            position.Y += 50;
            font = font24;
            text = "Menu";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "edited by Noah";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "http://www.wallpaperweb.org/wallpaper/nature/1680x1050/00605_snowscape.jpg";
            position.X = 150;
            batch.DrawString(font, text, position, Color.White);


            /* Tron Level */

            position.Y += 50;
            font = font24;
            text = "Tron Level";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "composed by Noah";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "http://windows7themes.net/wallpaper/Daft_Punk_Wallpaper.jpg";
            position.X = 150;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "http://kingofgng.com/media/20090726_tron_legacy_logo.jpg";
            position.X = 150;
            batch.DrawString(font, text, position, Color.White);

            /* Mushroom Level */

            position.Y += 50;
            font = font24;
            text = "Mushroom Level";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            position.Y += 30;
            font = font16;
            text = "created by Noah";
            position.X = 100;
            batch.DrawString(font, text, position, Color.White);

            /* TECHNICAL */

            position.Y += 100;
            text = "technical";
            font = font28;
            position.X = XPositionForScreenCentered(font, text);
            batch.DrawString(font, text, position, Color.White);

            position.Y += 50;
            font = font24;
            text = "XNA";
            position.X = XPositionForLeftColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
            text = "Dynamic Particle System Framework";
            position.X = XPositionForRightColumn(font, text);
            batch.DrawString(font, text, position, Color.White);
        }

        private int XPositionForScreenCentered(SpriteFont font, String text)
        {
            Vector2 measuredSize = font.MeasureString(text);
            return (game.GraphicsDevice.Viewport.Width - (int)measuredSize.X) / 2;
        }

        private int XPositionForLeftColumn(SpriteFont font, String text)
        {
            Vector2 measuredSize = font.MeasureString(text);

            int availableSpace = (game.GraphicsDevice.Viewport.Width - 250) / 2;

            return ((availableSpace - (int)measuredSize.X) / 2) + 100;
        }

        private int XPositionForRightColumn(SpriteFont font, String text)
        {
            Vector2 measuredSize = font.MeasureString(text);

            int availableSpace = (game.GraphicsDevice.Viewport.Width - 250) / 2;

            return game.GraphicsDevice.Viewport.Width - 100 - availableSpace + ((availableSpace - (int)measuredSize.X) / 2);
        }

        public override void Load()
        {
            vertical_base = 0;

            fullScreen = game.GraphicsDevice.Viewport.Height;
            halfScreen = fullScreen / 2;

            batch = new SpriteBatch(game.GraphicsDevice);
            font36 = content.Load<SpriteFont>("Font36");
            font28 = content.Load<SpriteFont>("Font28");
            font24 = content.Load<SpriteFont>("Font24");
            font16 = content.Load<SpriteFont>("Font16");
            music = content.Load<Song>("son-of-flynn");
            font16italic = content.Load<SpriteFont>("Font16Italic");
        }

        public override void Unload()
        {
            batch.Dispose();
            music.Dispose();
        }

        protected override void SoundStarted()
        {
            base.SoundStarted();

            MediaPlayer.IsVisualizationEnabled = false;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(music);

        }

        protected override void SoundStopped()
        {
            base.SoundStarted();

            MediaPlayer.Stop();
        }
    }
}

