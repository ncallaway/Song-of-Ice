using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

using DPSF;
using DPSF.ParticleSystems;

namespace Ice
{
    class MushroomLevel : Level
    {
        private bool setup;

        private MushroomLevelParticleSystem particleSystem;

        private VertexPositionTexture[] vertices;
        private VertexDeclaration texturedVertexDeclaration;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Texture2D mushroom;
        private Texture2D hitmap;
        private Effect shader;

        private VisualizationData visData;

        private KeyboardState previousKeyboardState;
        private MouseState previousMouseState;

        private float hundredFrameMovingPowerAvg;
        private int frameCount;

        public MushroomLevel(SongOfIce game, MetaLevel meta) : base(game, meta) {
            setup = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!levelComplete && TotalLevelTime(gameTime).TotalSeconds < 324 ) {
                hitmap = particleSystem.Hitmap;

                /* Emitter drag */
                particleSystem.Emitter.PositionData.Velocity *= .98f;
                if (particleSystem.Emitter.PositionData.Velocity.Length() < .1f) {
                    particleSystem.Emitter.PositionData.Velocity = Vector3.Zero;
                }


                /* Emitter particles based on music / time / other state */
                setEmitterParticles(gameTime);

                clampParticleSystemEmitter();
                particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                /* Input */
                handleKeyboard();
                handleMouse();
            } else {
                MediaPlayer.Volume = 0;
                completeLevel();
                handleLevelCompleteMouse();
                handleLevelCompleteKeyboard();
            }
        }

        private void setEmitterParticles(GameTime gameTime)
        {
            MediaPlayer.GetVisualizationData(visData);

            /* Emitter PPS calcuation */

            /* Power modulation */
            /* The power is summed for all frequency bands, and there is a 200 frame moving average that's kept. */
            /* If the power is < 80% of the current 200 frame moving average, the power is treated as 0. If the power
             * is non-zero, a logistic growth function is applied. */
            float power = 0;
            float particlesPerSecond = 0;
            foreach (float f in visData.Frequencies) {
                power += f;
            }

            frameCount++;
            if (frameCount > 200) {
                hundredFrameMovingPowerAvg -= hundredFrameMovingPowerAvg / 200f;
            }

            hundredFrameMovingPowerAvg += power / 200f;

            float modPower = power - (hundredFrameMovingPowerAvg * .8f);
            if (modPower < 0) {
                modPower = 0;
            }

            modPower *= .5f;
            modPower = (float)(1 + Math.Pow(Math.E, (-(modPower-15))));

            if (modPower <= 0) {
                particlesPerSecond = 1;
            } else {
                particlesPerSecond = 100 / modPower;
            }

            /* Time modulation */
            float secondsElapsed = (float)TotalLevelTime(gameTime).TotalSeconds;
            particlesPerSecond *= secondsElapsed * .25f;


            particleSystem.Emitter.ParticlesPerSecond = particlesPerSecond;
        }

        private void handleLevelCompleteKeyboard()
        {
            KeyboardState keyboard = Keyboard.GetState();
            /* Keyboard Handling */
            if (keyboard.IsKeyDown(Keys.Escape)) {
                game.SwitchToMenu();
            }

            previousKeyboardState = keyboard;
        }

        private void handleMouse()
        {
            MouseState mouse = Mouse.GetState();
            /* Mouse handling */
            if (previousMouseState != null && mouse.X != previousMouseState.X) {
                float delta = mouse.X - previousMouseState.X;
                particleSystem.Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(delta * .005f));
            }

            previousMouseState = mouse;
        }

        private void handleLevelCompleteMouse()
        {
            /* Check for click */
            MouseState mouse = Mouse.GetState();
            
            if (mouse.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) {
                SwitchToNextLevel();
            }

            previousMouseState = mouse;
        }

        private void completeLevel()
        {
            if (levelComplete) {
                return;
            }
            mushroom.Save("1-up.png", ImageFileFormat.Png);
            game.UnlockLevel(metaLevel);
            levelComplete = true;
        }

        private void handleKeyboard()
        {
            KeyboardState keyboard = Keyboard.GetState();
            /* Keyboard Handling */
            if (keyboard.IsKeyDown(Keys.Escape)) {
                game.SwitchToMenu();
            }

            if (keyboard.IsKeyDown(Keys.Left)) {
                particleSystem.Emitter.PositionData.Velocity.X -= 0.6f;
            }
            if (keyboard.IsKeyDown(Keys.Right)) {

                particleSystem.Emitter.PositionData.Velocity.X += 0.6f;
            }

            previousKeyboardState = keyboard;
        }

        private void clampParticleSystemEmitter()
        {
            Vector3 screenPosition = game.GraphicsDevice.Viewport.Project(particleSystem.Emitter.PositionData.Position, projectionMatrix, viewMatrix, Matrix.Identity);

            /* Emitter position clamping */
            if (screenPosition.X < 0) {
                screenPosition.X = 0;
                particleSystem.Emitter.PositionData.Position = game.GraphicsDevice.Viewport.Unproject(screenPosition, projectionMatrix, viewMatrix, Matrix.Identity);
            } else if (screenPosition.X > game.GraphicsDevice.Viewport.Width) {
                screenPosition.X = game.GraphicsDevice.Viewport.Width;
                particleSystem.Emitter.PositionData.Position = game.GraphicsDevice.Viewport.Unproject(screenPosition, projectionMatrix, viewMatrix, Matrix.Identity);
            }

            /* Emitter speed clamping */
            if (particleSystem.Emitter.PositionData.Velocity.X < -12f) {
                particleSystem.Emitter.PositionData.Velocity.X = -12f;
            }

            if (particleSystem.Emitter.PositionData.Velocity.X > 12f) {
                particleSystem.Emitter.PositionData.Velocity.X = 12f;
            }

            /* Emitter rotation clamping */
            if (particleSystem.Emitter.OrientationData.Right.X < .707f) {
                if (particleSystem.Emitter.OrientationData.Right.Y > 0) {
                    particleSystem.Emitter.OrientationData.Right = new Vector3(1f, 1f, 0f);
                } else {
                    particleSystem.Emitter.OrientationData.Right = new Vector3(1f, -1f, 0f);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!setup) {
                SetUpVertices(game.GraphicsDevice);
                setup = true;
            }

            Matrix worldMatrix = Matrix.Identity;
            shader.CurrentTechnique = shader.Techniques["VerticalEdge_2_0"];
            shader.Parameters["xWorld"].SetValue(worldMatrix);
            shader.Parameters["xView"].SetValue(viewMatrix);
            shader.Parameters["xProjection"].SetValue(projectionMatrix);
            shader.Parameters["xTexture"].SetValue(mushroom);
            shader.Parameters["xHitmap"].SetValue(hitmap);
            shader.Parameters["xHitmapWidth"].SetValue(hitmap.Width);
            shader.Parameters["xHitmapHeight"].SetValue(hitmap.Height);
            if (!levelComplete) {
                shader.Parameters["xAlpha"].SetValue(1f);
            } else {
                shader.Parameters["xAlpha"].SetValue(.8f);
            }

            game.GraphicsDevice.RenderState.AlphaBlendEnable = true;

            shader.Begin();

            foreach (EffectPass pass in shader.CurrentTechnique.Passes) {
                pass.Begin();

                game.GraphicsDevice.VertexDeclaration = texturedVertexDeclaration;
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);

                pass.End();
            }
            shader.End();

            particleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, viewMatrix, projectionMatrix);

            if (!levelComplete) {
                particleSystem.Draw();
            } else {
                DrawLevelWinText();
            }

            game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;
        }

        public override void Load()
        {
            base.Load();

            visData = new VisualizationData();
            mushroom = content.Load<Texture2D>("1upheart");
            hitmap = content.Load<Texture2D>("hitmap");
            shader = content.Load<Effect>("effects");
            music = content.Load<Song>("minibosses-supermariobros2");

            SetUpCamera(game.GraphicsDevice);

            particleSystem = new MushroomLevelParticleSystem(game);
            particleSystem.AutoInitialize(game.GraphicsDevice, content, null);
            particleSystem.Hitmap = hitmap;
            particleSystem.CustView = viewMatrix;
            particleSystem.CustProjection = projectionMatrix;
            particleSystem.CustViewport = game.GraphicsDevice.Viewport;
        }

        public override void Unload()
        {
            mushroom.Dispose();
            hitmap.Dispose();
            shader.Dispose();
        }

        protected override void SoundStarted()
        {
            base.SoundStarted();

            MediaPlayer.IsVisualizationEnabled = true;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(music);
            
        }

        protected override void SoundStopped()
        {
            base.SoundStarted();

            MediaPlayer.Stop();
        }

        private void SetUpVertices(GraphicsDevice device)
        {
            vertices = new VertexPositionTexture[6];

            vertices[0].Position = new Vector3(-1f, 1f, 1);
            vertices[0].TextureCoordinate.X = 0;
            vertices[0].TextureCoordinate.Y = 0;

            vertices[1].Position = new Vector3(1f, -1f, 1);
            vertices[1].TextureCoordinate.X = 1;
            vertices[1].TextureCoordinate.Y = 1;

            vertices[2].Position = new Vector3(-1f, -1f, 1);
            vertices[2].TextureCoordinate.X = 0;
            vertices[2].TextureCoordinate.Y = 1;

            vertices[3].Position = new Vector3(-1f, 1f, 1);
            vertices[3].TextureCoordinate.X = 0;
            vertices[3].TextureCoordinate.Y = 0;

            vertices[4].Position = new Vector3(1f, 1f, 1);
            vertices[4].TextureCoordinate.X = 1;
            vertices[4].TextureCoordinate.Y = 0;

            vertices[5].Position = new Vector3(1f, -1f, 1);
            vertices[5].TextureCoordinate.X = 1;
            vertices[5].TextureCoordinate.Y = 1;

            texturedVertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
        }

        private void SetUpCamera(GraphicsDevice device)
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 30), Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }
    }
}
