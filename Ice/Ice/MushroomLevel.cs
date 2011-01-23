using System;
using System.Collections.Generic;
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

        private bool levelStarted;
        private TimeSpan levelStart;

        private MouseState previousMouseState;

        public MushroomLevel(SongOfIce game, MetaLevel meta) : base(game, meta) {
            setup = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            hitmap = particleSystem.Hitmap;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) {
                game.UnlockLevel(metaLevel);
                game.SwitchToMenu();
            }

            /* Emitter PPS calcuation */
            particleSystem.Emitter.ParticlesPerSecond = (float)TotalLevelTime(gameTime).TotalSeconds * (float)TotalLevelTime(gameTime).TotalSeconds;

            Vector3 screenPosition = game.GraphicsDevice.Viewport.Project(particleSystem.Emitter.PositionData.Position, projectionMatrix, viewMatrix, Matrix.Identity);

            if (screenPosition.X < 0)  {
                screenPosition.X = 0;
                particleSystem.Emitter.PositionData.Position = game.GraphicsDevice.Viewport.Unproject(screenPosition, projectionMatrix, viewMatrix, Matrix.Identity);
            } else if (screenPosition.X > game.GraphicsDevice.Viewport.Width) {
                screenPosition.X = game.GraphicsDevice.Viewport.Width;
                particleSystem.Emitter.PositionData.Position = game.GraphicsDevice.Viewport.Unproject(screenPosition, projectionMatrix, viewMatrix, Matrix.Identity);
            }


            /* Emitter drag */
            particleSystem.Emitter.PositionData.Velocity *= .98f;
            if (particleSystem.Emitter.PositionData.Velocity.Length() < .1f) {
                particleSystem.Emitter.PositionData.Velocity = Vector3.Zero;
            }

            /* Keyboard Handling */
            if (keyboard.IsKeyDown(Keys.Left)) {
                particleSystem.Emitter.PositionData.Velocity.X -= 0.6f;
            }
            if (keyboard.IsKeyDown(Keys.Right)) {
                
                particleSystem.Emitter.PositionData.Velocity.X += 0.6f;
            }

            /* Mouse handling */
            if (previousMouseState != null && mouse.X != previousMouseState.X) {
                float delta = mouse.X - previousMouseState.X;
                particleSystem.Emitter.OrientationData.Rotate(Matrix.CreateRotationZ(delta * .005f));
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

            particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            previousMouseState = mouse;
        }

        private TimeSpan TotalLevelTime(GameTime gameTime)
        {
            if (levelStarted) {
                return gameTime.TotalGameTime - levelStart;
            }

            levelStarted = true;
            levelStart = gameTime.TotalGameTime;
            return new TimeSpan();
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
            shader.Begin();

            foreach (EffectPass pass in shader.CurrentTechnique.Passes) {
                pass.Begin();

                game.GraphicsDevice.VertexDeclaration = texturedVertexDeclaration;
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);

                pass.End();
            }
            shader.End();

            particleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, viewMatrix, projectionMatrix);
            particleSystem.Draw();

            game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;
        }

        public override void Load()
        {            
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
