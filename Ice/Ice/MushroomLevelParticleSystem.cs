#region File Description
//===================================================================
// DefaultPointSpriteParticleSystemTemplate.cs
// 
// This file provides the template for creating a new Point Sprite Particle
// System that inherits from the Default Point Sprite Particle System.
//
// The spots that should be modified are marked with TODO statements.
//
// Copyright Daniel Schroeder 2008
//===================================================================
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace DPSF.ParticleSystems
{
    //-----------------------------------------------------------
    // TODO: Rename/Refactor the Particle System class
    //-----------------------------------------------------------
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
    [Serializable]
    class MushroomLevelParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public MushroomLevelParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any Particle System properties here
        //-----------------------------------------------------------

        public Texture2D Hitmap { get { return hitmap; } set { hitmap = value; } }
        private Texture2D hitmap;

        public Matrix CustView { get { return custview; } set { custview = value; } }
        private Matrix custview;

        public Matrix CustProjection { get { return custprojection; } set { custprojection = value; } }
        private Matrix custprojection;

        public Viewport CustViewport { get { return port; } set { port = value; } }
        private Viewport port;

        private Color[] pixels = new Color[1600 * 1200];

        private int f;
        private int stop;

        private int blockSize;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place any overridden Particle System functions here
        //-----------------------------------------------------------

        //===========================================================
        // Initialization Functions
        //===========================================================

        /// <summary>
        /// Function to Initialize the Particle System with default values
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device to draw to</param>
        /// <param name="cContentManager">The Content Manager to use to load Textures and Effect files</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            //-----------------------------------------------------------
            // TODO: Change any Initialization parameters desired and the Name
            //-----------------------------------------------------------
            // Initialize the Particle System before doing anything else
            InitializePointSpriteParticleSystem(cGraphicsDevice, cContentManager, 5000, 50000, 
                                                UpdateVertexProperties, "flakegreen");

            // Set the Name of the Particle System
            Name = "Green Tibits";

            f = 0;
            stop = 300;

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {
            //-----------------------------------------------------------
            // TODO: Setup the Particle System to achieve the desired result.
            // You may change all of the code in this function. It is just
            // provided to show you how to setup a simple particle system.
            //-----------------------------------------------------------

            // Set the Function to use to Initialize new Particles.
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            //ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
            ParticleInitializationFunction = InitializeParticleProperties;

            // Setup the Initial properties of the Particles.
            // These are only applied if using InitializeParticleUsingInitialProperties 
            // as the Particle Initialization Function.

            blockSize = 3;

            InitialProperties.LifetimeMin = 10f;
            InitialProperties.LifetimeMax = 10f;
            InitialProperties.PositionMin = new Vector3(-1f, -1f, 0f);
            InitialProperties.PositionMax = new Vector3(1f, 1f, 0f);
            InitialProperties.VelocityMin = new Vector3(-.5f, -3f, 0);
            InitialProperties.VelocityMax = new Vector3(.5f, -6f, 0);
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin = -MathHelper.Pi;
            InitialProperties.RotationalVelocityMax = MathHelper.Pi;
            InitialProperties.StartSizeMin = .25f;
            InitialProperties.StartSizeMax = 1f;
            InitialProperties.EndSizeMin = .25f;
            InitialProperties.EndSizeMax = 1f;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Position, Rotation, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            //ParticleEvents.AddEveryTimeEvent(UpdateParticleSizeUsingLerp);
            //ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleForPosition);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleHitmapCollision);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.AddOneTimeEvent(UpdatePixelData);
            ParticleSystemEvents.AddEveryTimeEvent(UpdateBitmapData, 101);
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            //ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 1000;
            Emitter.PositionData.Position = new Vector3(0, 12, 0);
            Emitter.OrientationData.Right = new Vector3(1f, 1f, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultPointSpriteParticle cParticle)
        {
            //-----------------------------------------------------------
            // TODO: Initialize all of the Particle's properties here.
            // If you plan on simply using the default InitializeParticleUsingInitialProperties
            // Particle Initialization Function (see the LoadParticleSystem() function above), 
            // then you may delete this function all together.
            //-----------------------------------------------------------

            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 10f;

            // Set the Particle's initial Position to be wherever the Emitter is

            Vector3 positionDelta = DPSFHelper.RandomVectorBetweenTwoVectors(InitialProperties.PositionMin, InitialProperties.PositionMax);
            if (positionDelta.Length() > 1f) {
                positionDelta.Normalize();
            }

            cParticle.Position = Emitter.PositionData.Position + positionDelta;

            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(InitialProperties.VelocityMin, InitialProperties.VelocityMax);

            cParticle.Rotation = DPSFHelper.RandomNumberBetween(InitialProperties.RotationMin, InitialProperties.RotationMax);

            cParticle.RotationalVelocity = DPSFHelper.RandomNumberBetween(InitialProperties.RotationalVelocityMin, InitialProperties.RotationalVelocityMax);

            cParticle.Size = .25f;
            cParticle.StartSize = .25f;
            cParticle.EndSize = 1f;
            cParticle.Color = Color.White;
            cParticle.StartColor = Color.White;
            cParticle.EndColor = Color.White;

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle Update functions here, using the 
        // same function prototype as below (i.e. public void FunctionName(DPSFParticle, float))
        //-----------------------------------------------------------

        public void UpdateParticleHitmapCollision(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            
            if (CustView == null || CustProjection == null) {
                return;
            }

            /* Get SCREEN POSTION */
            Vector3 screenPosition = port.Project(cParticle.Position, custprojection, custview, Matrix.Identity);

            /* Basic culling & physics*/
            if (screenPosition.X < 0 || screenPosition.X > port.Width) {
                if (screenPosition.X < 0) {
                    cParticle.Velocity.X = Math.Abs(cParticle.Velocity.X);
                } else {
                    cParticle.Velocity.X = -Math.Abs(cParticle.Velocity.X);
                }
            }
            if (screenPosition.Y < 0 || screenPosition.Y > port.Height) {
                if (screenPosition.Y > port.Height) {
                    cParticle.NormalizedElapsedTime = 1.0f;
                }
            }

            Vector2 position = new Vector2(screenPosition.X * (1600f / (float)port.Width), screenPosition.Y * (1200f / (float)port.Height));
            
            if (readPosition(position)) {
                /* HIT! */

                cParticle.NormalizedElapsedTime = 1.0f; /* kill the particle */

                for (int x = (int)position.X - blockSize; x < (int)position.X + blockSize; x++) {
                    for (int y = (int)position.Y - blockSize; y < (int)position.Y + blockSize; y++) {
                        writePosition(x, y, true);
                    }

                    int lastFill = (int)position.Y - blockSize;
                    for (int y = (int)position.Y + blockSize; y < 1200 && y - ((int)position.Y +blockSize) < 25; y++) {
                        if (readPosition(x, y)) {
                            /* must be partically continuous fill! */
                            for (int j = y; j > lastFill; j--) {
                                writePosition(x, j, true);
                            }
                            lastFill = y;
                        }
                    }
                }
                
            }
        }

        private bool readPosition(Vector2 position)
        {
            return readPosition((int)position.X, (int)position.Y);
        }

        private bool readPosition(int x, int y)
        {
            int index = x + (y * 1600);

            if (index < 0 || index >= 1600 * 1200) {
                return false;
            }

            Color pixel = pixels[index];
            return (pixel.R > 100);
        }

        private void writePosition(Vector2 position, bool value)
        {
            writePosition((int)position.X, (int)position.Y, value);
        }

        private void writePosition(int x, int y, bool value)
        {
            int index = x + (y * 1600);
            if (index < 0 || index > 1600 * 1200) {
                return;
            }

            byte pixValue = (value) ? (byte)255 : (byte)0;

            pixels[index].R = pixValue;
            pixels[index].G = pixValue;
            pixels[index].B = pixValue;
        }

        /// <summary>
        /// Example of how to create a Particle Event Function
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleFunctionExample(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle here
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
            cParticle.NormalizedElapsedTime = 1.0f;
        }

        protected void UpdateParticleForPosition(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle here
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
            cParticle.NormalizedElapsedTime = (cParticle.Position.Y - 20f) / -30f;

            if (cParticle.Position.Y < -20f)
            {
                cParticle.NormalizedElapsedTime = 1.0f;
            }
        }
        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //-----------------------------------------------------------
        // TODO: Place your Particle System Update functions here, using 
        // the same function prototype as below (i.e. public void FunctionName(float))
        //-----------------------------------------------------------

        /// <summary>
        /// Example of how to create a Particle System Event Function
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle System here
            // Example: Emitter.EmitParticles = true;
            // Example: SetTexture("TextureAssetName");
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        public void UpdatePixelData(float fElapsedTimeInSeconds)
        {
            hitmap.GetData<Color>(pixels);
        }

        public void UpdateBitmapData(float fElapsedTimeInSeconds)
        {
            hitmap.SetData<Color>(pixels);
        }
    }
}
