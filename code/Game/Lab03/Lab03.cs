using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab03
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab03 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model model; // FBX File

        Matrix world, view, projection;
        Effect effect;

        //3D camera and light
        Vector3 cameraOffset = new Vector3(0, 0, 0);
        Vector3 cameraPosition = new Vector3(0, 0, 10);
        Vector3 lightPosition = new Vector3(1, 1, 1);
        float lightAngle = 0;
        float lightAngle2 = 0;
        float angle = 0;
        float angle2 = 0;
        float distance = 10;

        //object materials
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        Vector3 diffuseLightDirection = new Vector3(1, 1, 1);
        float diffuseIntensity = 0.3f;

        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float shininess = 10.0f;

        //Mouse Event
        MouseState previousMouseState;

        public Lab03()
        {
            graphics = new GraphicsDeviceManager(this);

            //Setting the screen size to 3k resolution
            graphics.PreferredBackBufferWidth = 2880;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1620;   // set this value to the desired height of your window

            Content.RootDirectory = "Content";
            // ***** From MonoGame3.6 Need this statement
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            // **********************************************
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("bunny");
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(
                new Vector3(0, 0, 10),
                Vector3.Zero,
                Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100);
            effect = Content.Load<Effect>("Diffuse");
           

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            //{
            //    angle += 0.02f;

            //}
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                float offsetx = 0.1f*(Mouse.GetState().X - previousMouseState.X);
                float offsety = 0.1f*(Mouse.GetState().Y - previousMouseState.Y);
                angle += offsetx;
                angle2 += offsety;
            }
            //Vector3 cameraPosition = distance * new Vector3(
            //    (float)System.Math.Sin(angle),
            //    0, (float)System.Math.Cos(angle));
            Vector3 camera = Vector3.Transform(
                new Vector3(0, 0, 20), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);
            //view = Matrix.CreateLookAt(cameraPosition, new Vector3(), new Vector3(0, 1, 0));
            effect.Parameters["View"].SetValue(view);

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //model.Draw(world, view, projection);
            effect.CurrentTechnique = effect.Techniques[0];
            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseLightDirection"].SetValue(diffuseLightDirection);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                       

                        pass.Apply();
                        Matrix worldInverseTranspose = Matrix.Transpose(
                            Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);

                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;

                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList, part.VertexOffset, 0,
                            part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        
                    }
                }

            }
            

            base.Draw(gameTime);
        }
    }
}
