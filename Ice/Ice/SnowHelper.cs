using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

using DPSF;
using DPSF.ParticleSystems;

namespace Sands
{
    class SnowHelper : MenuHelper
    {
        private bool setup;

        private MenuSnowParticleSystem particleSystem;

        private VertexPositionTexture[] vertices;
        private VertexDeclaration texturedVertexDeclaration;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Texture2D snow;
        private Effect shader;

        /// <summary>
        /// Creates a new snow helper with a particle system tied to
        /// the specific Game object.
        /// </summary>
        /// <param name="game">Game object that is used by the snow particle system</param>
        public SnowHelper(Game game) : base(game)
        {
            setup = false;
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            particleSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            if (!setup)
            {
                SetUpVertices(device);
                SetUpCamera(device);
                setup = true;
            }

            Matrix worldMatrix = Matrix.Identity;
            shader.CurrentTechnique = shader.Techniques["Textured"];
            shader.Parameters["xWorld"].SetValue(worldMatrix);
            shader.Parameters["xView"].SetValue(viewMatrix);
            shader.Parameters["xProjection"].SetValue(projectionMatrix);
            shader.Parameters["xTexture"].SetValue(snow);
            shader.Begin();

            foreach (EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Begin();

                device.VertexDeclaration = texturedVertexDeclaration;
                device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);

                pass.End();
            }
            shader.End();

            particleSystem.SetWorldViewProjectionMatrices(Matrix.Identity, viewMatrix, projectionMatrix);

            particleSystem.Draw();


        }

        public override void Load(ContentManager content)
        {
            particleSystem = new MenuSnowParticleSystem(game);
            particleSystem.AutoInitialize(game.GraphicsDevice, content, null);


            music = content.Load<Song>("First-Breath-After-Coma");
            snow = content.Load<Texture2D>("Snowscape_stub");
            shader = content.Load<Effect>("effects");
        }

        public override void Unload(ContentManager content)
        {
            if (playing)
            {
                Stop();
            }

            particleSystem.Destroy();

            music.Dispose();
            snow.Dispose();
            shader.Dispose();
            music = null;
        }

        private void SetUpVertices(GraphicsDevice device)
        {
            vertices = new VertexPositionTexture[6];

            vertices[0].Position = new Vector3(-1f, 1f, -10000f);
            vertices[0].TextureCoordinate.X = 0;
            vertices[0].TextureCoordinate.Y = 0;

            vertices[1].Position = new Vector3(1f, -1f, -10000f);
            vertices[1].TextureCoordinate.X = 1;
            vertices[1].TextureCoordinate.Y = 1;

            vertices[2].Position = new Vector3(-1f, -1f, -100f);
            vertices[2].TextureCoordinate.X = 0;
            vertices[2].TextureCoordinate.Y = 1;

            vertices[3].Position = new Vector3(-1f, 1f, -100f);
            vertices[3].TextureCoordinate.X = 0;
            vertices[3].TextureCoordinate.Y = 0;

            vertices[4].Position = new Vector3(1f, 1f, -100f);
            vertices[4].TextureCoordinate.X = 1;
            vertices[4].TextureCoordinate.Y = 0;

            vertices[5].Position = new Vector3(1f, -1f, -100f);
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
