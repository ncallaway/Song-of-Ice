using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Win32;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using DPSF;

namespace Sands
{
    public delegate void MetaLevelsUpdated();

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SandsGame : Microsoft.Xna.Framework.Game
    {
        public const bool windowed = true;

        private ParticleSystemManager particleManager;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Menu mainMenu;

        private List<MetaLevel> metas;

        public  MetaLevelsUpdated MetaLevelInfoUpdated { get { return metaLevelUpdatedDelegate; } set { metaLevelUpdatedDelegate = value; } }
        private MetaLevelsUpdated metaLevelUpdatedDelegate;

        public SandsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "Sands";
            IsMouseVisible = true;
            
            particleManager = new ParticleSystemManager();
            mainMenu = new Menu(this);
            metas = MetaLevelFactory.CreateDefaultMetaLevelList();
            InitializeLevelLockStatus();

            /* Fullscreen Options */
            if (windowed)
            {
                graphics.PreferredBackBufferWidth = 1024;
                graphics.PreferredBackBufferHeight = 768;
            } else {
                graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
                graphics.IsFullScreen = true;
            }

        }

        public void InitializeLevelLockStatus() {
            RegistryKey hkcu = Registry.CurrentUser;
            RegistryKey software = hkcu.CreateSubKey("Software");
            RegistryKey adundas = software.CreateSubKey("Adundas");
            RegistryKey pieces = adundas.CreateSubKey("Ice");
            RegistryKey levels = pieces.CreateSubKey("levels");

            uint count = 0;
            foreach (MetaLevel l in metas)
            {
                object result = levels.GetValue(l.Id);
                
                if (result == null)
                {
                    /* Set the default value */
                    if (count == 0)
                    {
                        levels.SetValue(l.Id, 1, RegistryValueKind.DWord); /* 0 - KNOWN, 1 - UNKNOWN, 2 - LOCKED */
                        l.State = LevelState.UNKNOWN;
                    }
                    else
                    {
                        levels.SetValue(l.Id, 2, RegistryValueKind.DWord);
                        l.State = LevelState.LOCKED;
                    }
                    count++;
                    continue;
                }

                Int32 loaded = (Int32)result;

                switch (loaded)
                {
                    case 0:
                        l.State = LevelState.KNOWN;
                        break;
                    case 1:
                        l.State = LevelState.UNKNOWN;
                        break;
                    case 2:
                        /* FALL-THRU */
                    default:
                        l.State = LevelState.LOCKED;
                        break;
                }

                count++;
            }
        }

        internal List<MetaLevel> GetMetaLevels()
        {
            return metas;
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainMenu.Load();

            mainMenu.Sound = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            mainMenu.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);

            mainMenu.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            mainMenu.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
