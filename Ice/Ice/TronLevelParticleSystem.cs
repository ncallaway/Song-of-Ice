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
    class TronLevelParticleSystem : DefaultPointSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public TronLevelParticleSystem(Game cGame) : base(cGame) { }

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
        private int normalBlockSize;

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
                                                UpdateVertexProperties, "diamondblue");

            // Set the Name of the Particle System
            Name = "Blue Tibits";

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
            normalBlockSize = 10;

            InitialProperties.LifetimeMin = 20f;
            InitialProperties.LifetimeMax = 20f;
            InitialProperties.PositionMin = new Vector3(-.5f, -.5f, 0f);
            InitialProperties.PositionMax = new Vector3(.5f, .5f, 0f);
            InitialProperties.VelocityMin = new Vector3(-2f, -3f, 0);
            InitialProperties.VelocityMax = new Vector3(2f, -6f, 0);
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
            cParticle.Lifetime = 20f;

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

        public void UpdateParticlePhysics(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
        }

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

            /* gravity! */
            if (cParticle.Velocity.Y > -12f) {
                cParticle.Velocity.Y -= .06f;
            }

            Vector2 position = new Vector2(screenPosition.X * (1600f / (float)port.Width), screenPosition.Y * (1200f / (float)port.Height));
            
            if (readPosition(position)) {
                /* HIT! */

                if (position.Y < 150) {
                    cParticle.NormalizedElapsedTime = 1.0f; /* kill the particle */
                    return;
                }

                
                Vector2 surfacePosition = findSurfacePosition(position, cParticle.Velocity);
                if (position != surfacePosition && cParticle.Velocity.Length() > 2f) {
                    /* move the particle to the surface */
                    position = surfacePosition; //2 * surfacePosition - position;
                    moveToPosition(cParticle, position);
                }

                
                /* Bounce! */
                if (cParticle.Velocity.Length() > 2f) {
                    Vector3 normal = computeNormal(position);
                    Vector3 inVelocity = cParticle.Velocity;
                    inVelocity.Normalize();
                    Vector3 velocity = Vector3.Reflect(inVelocity, normal);
                    velocity *= cParticle.Velocity.Length() * 0.05f;

                    cParticle.Velocity = velocity;
                    return;
                }

               
                cParticle.NormalizedElapsedTime = 1.0f; /* kill the particle */ 

                for (int x = (int)position.X - blockSize; x < (int)position.X + blockSize; x++) {
                    for (int y = (int)position.Y - blockSize; y < (int)position.Y + blockSize; y++) {
                        writePosition(x, y, true);
                    }

                    int lastFill = (int)position.Y - blockSize;
                    for (int y = (int)position.Y + blockSize; y < 1200 && y - ((int)position.Y +blockSize) < 45; y++) {
                        if (readPosition(x, y)) {
                            /* must be continuous fill! */
                            for (int j = y; j > lastFill; j--) {
                                writePosition(x, j, true);
                            }
                            lastFill = y;
                        }
                    }
                }
                
            }
        }

        private void moveToPosition(DefaultPointSpriteParticle cParticle, Vector2 position)
        {
            Vector2 screenPosition = new Vector2(position.X * ((float)port.Width / 1600f), position.Y * ((float)port.Height / 1200f));

            Vector3 currentScreenPosition = port.Project(cParticle.Position, custprojection, custview, Matrix.Identity);
            currentScreenPosition.X = screenPosition.X;
            currentScreenPosition.Y = screenPosition.Y;
            cParticle.Position = port.Unproject(currentScreenPosition, custprojection, custview, Matrix.Identity);
        }

        private Vector2 findSurfacePosition(Vector2 startPosition, Vector3 velocity)
        {
            float x = startPosition.X;
            float y = startPosition.Y;

            velocity.Normalize();

            while (x >= 0 && x < 1600 && y >= 150 && y < 1200 && readPosition((int)x, (int)y)) {
                x -= velocity.X;
                y += velocity.Y;
            }

            x += velocity.X;
            y -= velocity.Y;

            if (readPosition((int)x, (int)y)) {
                return new Vector2((int)x, (int)y);
            }
            return startPosition;
        }

        private Vector3 computeNormal(Vector2 position)
        {
            /*
            Vector3 normal = Vector3.UnitY;
            int x = (int)position.X;
            int y = (int)position.Y;

            for (int i = x - 1; i <= x + 1; i++) {
                for (int j = y - 1; j <= y + 1; j++) {
                    if (readPosition(i, j)) {
                        normal += new Vector3(x - i, y - j, 0);
                    }
                }
            }

            /* cheat, and only take positive normals
            if (normal.Y < 0) {
                normal = -normal;
            }
            normal.Normalize();
            return normal;*/

            Vector3 normal;

            int x = (int)position.X;
            int y = (int)position.Y;

            float n = (normalBlockSize * 2) + 1;
            float sumX = n * x;
            float sumXsquared = 0;
            float sumY = 0;
            float sumXY = 0;

            int height = y;
            /* Go left */
            for (int i = 0; i < normalBlockSize; i++) {
                height = surfaceHeight(x - i, height);
                sumXsquared += (x - i) * (x - 1);
                sumY += height;
                sumXY += (x - i) * height;
            }

            /* center */
            sumXsquared += x * x;
            sumY += y;
            sumXY += x * y;

            /* Go right */
            height = y;
            for (int i = 0; i < normalBlockSize; i++) {
                height = surfaceHeight(x + i, height);
                sumXsquared += (x + i) * (x + 1);
                sumY += height;
                sumXY += (x + i) * height;
            }

            float slopeDenominator = (sumX * sumX - n * sumXsquared);
            float slopeNumerator = ((sumX * sumY - n * sumXY));

            if (slopeDenominator == 0) {
                /* Vertical line */
                return Vector3.UnitX;
            }

            if (slopeNumerator == 0) {
                /* Horizontal line */
                return Vector3.UnitY;
            }

            float slope = slopeNumerator / slopeDenominator;

            Vector2 hill = new Vector2(1, slope);
            normal = new Vector3(1, slope, 0);

            /* unneccessary safety check! */
            if (normal.Y < 0) {
                normal = -normal;
            }

            normal.Normalize();
            return normal;
        }

        private int surfaceHeight(float x, float startY)
        {
            int intX = (int)x;
            int intY = (int)startY;
            if (intY < 150) {
                return 150;
            }

            if (intY >= 1200) {
                intY = 1200 - 1;
            }

            if (intX < 0 || intX >= 1600) {
                return intY;
            }

            bool up = readPosition(intX, intY);

            intY--;

            while (up && intY > 150 && readPosition(intX, intY)) {
                intY--;
            }

            while (!up && intY < 1200 && !readPosition(intX, intY)) {
                intY++;
            }

            if (up) {
                /* Up alg overscans by one! */
                intY++;
            }

            return intY;
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

        protected void UpdateParticleForPosition(DefaultPointSpriteParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Example: cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
            cParticle.NormalizedElapsedTime = 0.0f;
        }
        //===========================================================
        // Particle System Update Functions
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
