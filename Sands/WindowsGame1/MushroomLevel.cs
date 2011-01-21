using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace Sands
{
    class MushroomLevel : Level
    {
        private bool setup;

        private VertexPositionTexture[] vertices;
        private VertexDeclaration texturedVertexDeclaration;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Texture2D snow;
        private Effect shader;

        public MushroomLevel(SandsGame game, MetaLevel meta) : base(game, meta) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape)) {
                game.UnlockLevel(metaLevel);
                game.SwitchToMenu();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!setup) {
                SetUpVertices(game.GraphicsDevice);
                SetUpCamera(game.GraphicsDevice);
                setup = true;
            }

            Matrix worldMatrix = Matrix.Identity;
            shader.CurrentTechnique = shader.Techniques["Textured"];
            shader.Parameters["xWorld"].SetValue(worldMatrix);
            shader.Parameters["xView"].SetValue(viewMatrix);
            shader.Parameters["xProjection"].SetValue(projectionMatrix);
            shader.Parameters["xTexture"].SetValue(snow);
            shader.Begin();

            foreach (EffectPass pass in shader.CurrentTechnique.Passes) {
                pass.Begin();

                game.GraphicsDevice.VertexDeclaration = texturedVertexDeclaration;
                game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 2);

                pass.End();
            }
            shader.End();
        }

        public override void Load()
        {
            snow = content.Load<Texture2D>("1upheart");
            shader = content.Load<Effect>("effects");
        }

        public override void Unload()
        {
            snow.Dispose();
            shader.Dispose();
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
