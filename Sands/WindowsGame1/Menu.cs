using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using DPSF;
using DPSF.ParticleSystems;

namespace Sands
{
    class Menu : View
    {
        SpriteBatch batch;
        SpriteFont  font36;
        SpriteFont  font28;
        SpriteFont  font24;
        Texture2D   padlock;
        Texture2D   questionMark;
        Texture2D   white;
        Texture2D   arrow;
        MenuHelper  currentHelper;
        SnowHelper  snowHelper;

        private int menuSelector;
        private Color selected;
        private Color unselected;

        private KeyboardState previousKeyboardState;
        private MouseState previousMouseState;


        private List<MetaLevel> metas;
        private List<Texture2D> metaResources;

        public Menu(SandsGame game) : base(game)
        {
            snowHelper = new SnowHelper(game);
            currentHelper = snowHelper;

            menuSelector = 0;
            selected = Color.White;
            unselected = new Color(200, 200, 200);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            /* Update our particle system */
            currentHelper.Update(gameTime);

            HandleKeyboard();
            HandleMouse();
        }

        private void HandleKeyboard()
        {
             /* Handle keyboard input */
            KeyboardState currentState = Keyboard.GetState();

            if (currentState.IsKeyDown(Keys.Down)) {
                if (previousKeyboardState == null || previousKeyboardState.IsKeyUp(Keys.Down)) {
                    // MOVE DOWN
                    menuSelector++;
                    if (menuSelector > metas.Count || metas[menuSelector - 1].State == LevelState.LOCKED) {
                        /* Invalid, revert it */
                        menuSelector--;
                    }
                }
            }

            if (currentState.IsKeyDown(Keys.Up)) {
                if (previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.Up)) {
                    // MOVE UP
                    menuSelector--;
                    if (menuSelector < 0 || (menuSelector > 0 && metas[menuSelector - 1].State == LevelState.LOCKED)) {
                        /* Invalid, revert it */
                        menuSelector++;
                    }
                }
            }

            if (currentState.IsKeyDown(Keys.Enter)) {
                if (previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.Enter)) {
                    int index = menuSelector;
                    if (index > 0) {
                        index--;
                    }

                    game.SwitchToLevel(metas[index]);
                }
            }

            previousKeyboardState = currentState;
        }

        private void HandleMouse()
        {
            MouseState currentState = Mouse.GetState();

            bool click = false;

            if (currentState.LeftButton == ButtonState.Released) {
                if (previousMouseState != null && previousMouseState.LeftButton == ButtonState.Pressed) {
                    click = true;
                }
            }

            if (isOverNewGame(currentState)) {
                menuSelector = 0;
            }

            int i = 0;
            foreach (MetaLevel ml in metas) {
                if (ml.State != LevelState.LOCKED) {
                    String text = (ml.State == LevelState.UNKNOWN) ? "???" : ml.Title;
                    Vector2 measurement = font24.MeasureString(text);
                    if (isOverMenuItem(currentState, (int)measurement.X, i)) {
                        menuSelector = i + 1;
                    }
                }
                i++;
            }

            if (click) {
                int index = menuSelector;
                if (index > 0) {
                    index--;
                }
                game.SwitchToLevel(metas[index]);
            }

            previousMouseState = currentState;
        }

        private bool isOverNewGame(MouseState state)
        {
            if (state.X > 65 && state.X < 200) {
                if (state.Y > 115 && state.Y < 145) {
                    return true;
                }
            }
            return false;
        }

        private bool isOverMenuItem(MouseState state, int labelWidth, int index)
        {
            if (state.X > 90 && state.X < 195 + labelWidth) {
                if (state.Y > 175 + index * 52 && state.Y < 225 + index * 52) {
                    return true;
                }
            }
            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            currentHelper.Draw(game.GraphicsDevice, gameTime);

            batch.Begin();

            RenderTitle();
            RenderMenu();
            
            
            batch.End();
        }

        public void RenderTitle()
        {
            int width = game.GraphicsDevice.Viewport.Width;
            int centre = width / 2;

            Vector2 measurement = font36.MeasureString("Song of Ice");
            int xLocation = centre - ((int)measurement.X / 2);

            batch.DrawString(font36, "Song of Ice", new Vector2(xLocation, 20), Color.White);
        }

        public void RenderMenu()
        {
            Color tmpColor = (menuSelector == 0) ? selected : unselected;
            batch.DrawString(font28, "New Game", new Vector2(75, 100), tmpColor);

            batch.DrawString(font24, "Replay Levels", new Vector2(75, 133), unselected);

            if (menuSelector == 0) {
                batch.Draw(arrow, new Rectangle(45, 122, 25, 14), Color.White);
            }

            int i = 0;
            foreach (MetaLevel ml in metas) {
                String title;
                Texture2D resource;
                if (ml.State == LevelState.KNOWN) {
                    title = ml.Title;
                    resource = metaResources[i];
                } else if (ml.State == LevelState.UNKNOWN) {
                    title = "???";
                    resource = questionMark;
                } else {
                    title = "Locked";
                    resource = padlock;
                }

                Rectangle whiteDest = new Rectangle(100, 180 + i * 52, 54, 42);
                Rectangle resourceDest = new Rectangle(102, 182 + i * 52, 50, 38);

                tmpColor = (menuSelector == i+1) ? selected : unselected;

                batch.Draw(white, whiteDest, tmpColor);
                batch.Draw(resource, resourceDest, Color.White);
                batch.DrawString(font24, title, new Vector2(165, 178 + i * 52), tmpColor);

                if (menuSelector == i + 1) {
                    batch.Draw(arrow, new Rectangle(45, 190 + i * 52, 25, 14), Color.White);
                }

                i++;
            }
        }

        public override void Load()
        {
            metas = game.GetMetaLevels();
            metaResources = new List<Texture2D>();

            batch = new SpriteBatch(game.GraphicsDevice);
            font36 = content.Load<SpriteFont>("Font36");
            font28 = content.Load<SpriteFont>("Font28");
            font24 = content.Load<SpriteFont>("Font24");
            padlock = content.Load<Texture2D>("padlock");
            questionMark = content.Load<Texture2D>("questionMark");
            white = content.Load<Texture2D>("white");
            arrow = content.Load<Texture2D>("arrow");

            foreach (MetaLevel ml in metas) {
                Texture2D resource = content.Load<Texture2D>(ml.ThumbnailResource);
                metaResources.Add(resource);
            }

            snowHelper.Load(content);
            if (sound)
            {
                snowHelper.Play();
            }
        }

        public override void Unload()
        {
            batch.Dispose();
            font36 = null;
            font28 = null;
            font24 = null;
            padlock.Dispose();
            questionMark.Dispose();
            white.Dispose();
            arrow.Dispose();

            foreach (Texture2D tex in metaResources) {
                tex.Dispose();
            }
            metaResources.Clear();

            snowHelper.Stop();
            snowHelper.Unload(content);
        }

        protected override void SoundStarted()
        {
            base.SoundStarted();

            if (currentHelper != null) {
                currentHelper.Play();
            }
        }

        protected override void SoundStopped()
        {
            base.SoundStopped();

            if (currentHelper != null)
            {
                currentHelper.Stop();
            }
        }
        
    }
}
