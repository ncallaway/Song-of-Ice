using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Ice
{
    abstract class Level : View
    {
        protected MetaLevel metaLevel;
        protected Song music;

        protected bool levelStarted;
        protected TimeSpan levelStart;
        protected bool levelComplete;

        protected SpriteBatch levelWinBatch;
        protected SpriteFont levelWinFont;

        public Level(SongOfIce game, MetaLevel meta)
            : base(game)
        {
            metaLevel = meta;
            levelComplete = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!levelStarted) {
                levelStarted = true;
                levelStart = gameTime.TotalGameTime;
                levelComplete = false;
            }
        }

        public override void Load()
        {
            levelWinBatch = new SpriteBatch(game.GraphicsDevice);
            levelWinFont = content.Load<SpriteFont>("Font28");
        }

        protected void DrawLevelWinText()
        {
            Vector2 position;
            Vector2 fontSize = levelWinFont.MeasureString("\'gratz! You beat the level!");
            position.Y = game.GraphicsDevice.Viewport.Height / 2;
            position.X = (game.GraphicsDevice.Viewport.Width - fontSize.X) / 2;

            levelWinBatch.Begin();

            levelWinBatch.DrawString(levelWinFont, "\'gratz! You beat the level!", position, Color.White);

            fontSize = levelWinFont.MeasureString("Clicky the mouse to go to the next level.");
            position.Y += 30;
            position.X = (game.GraphicsDevice.Viewport.Width - fontSize.X) / 2;

            levelWinBatch.DrawString(levelWinFont, "Clicky the mouse to go to the next level.", position, Color.White);
            levelWinBatch.End();
        }

        protected TimeSpan TotalLevelTime(GameTime gameTime)
        {
            if (levelStarted) {
                return gameTime.TotalGameTime - levelStart;
            }

            levelStarted = true;
            levelStart = gameTime.TotalGameTime;
            return new TimeSpan();
        }

        protected void SwitchToNextLevel()
        {
            MetaLevel nextLevel = game.GetNextLevel(metaLevel);
            if (nextLevel == null) {
                game.SwitchToMenu();
            } else {
                game.SwitchToLevel(nextLevel);
            }
        }
    }
}
