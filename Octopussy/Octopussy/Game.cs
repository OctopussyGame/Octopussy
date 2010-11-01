#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace Octopussy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;

        BloomComponent bloom;
        int bloomSettingsIndex = 0;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D background;

        Entity model, rio, tank;
        
        Model grid;

        public ParticleSystem explosionParticles;
        public ParticleSystem explosionSmokeParticles;
        public ParticleSystem projectileTrailParticles;
        public ParticleSystem smokePlumeParticles;
        public ParticleSystem fireParticles;

        List<Projectile> projectiles = new List<Projectile>();

        TimeSpan timeToNextProjectile = TimeSpan.Zero;

        Random random = new Random();

        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        public AudioManager audioManager;

        // Camera state.
        float cameraArc = -45;
        float cameraRotation = 0;
        float cameraDistance = 1200;
        Vector3 fpsCameraPosition, fpsCameraTarget;

        // Draw matricies
        public Matrix ViewMatrix
        {
            get { return view; }
        }
        public Matrix ProjectionMatrix
        {
            get { return projection; }
        }
        public Matrix view;
        public Matrix projection;

        Boolean fpsCamera = false;

        // Light
        // the light rotates around the origin using these 3 constants.  the light
        // position is set in the draw function.
        const float LightHeight = 600;
        const float LightRotationRadius = 800;
        const float LightRotationSpeed = .5f;
        bool rotateLight = true;
        float lightRotation;
        public Vector3 lightPosition;

        #endregion

        #region Initialization

        public Game()
        {
            // Basic initialization
            Window.Title = "Octopussy Game";
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            
            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(this, Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(this, Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(this, Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(this, Content);
            fireParticles = new FireParticleSystem(this, Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            Components.Add(explosionParticles);
            Components.Add(explosionSmokeParticles);
            Components.Add(projectileTrailParticles);
            Components.Add(smokePlumeParticles);
            Components.Add(fireParticles);

            // Load entities
            model = new Entity(this, "models/lizard/lizard", true);
            model.RotateInTime = true;
            tank = new Entity(this, "models/popelnice/popelnice");
            tank.Position = new Vector3(400, 0, -400);
            tank.RotateInTime = true;
            rio = new Entity(this, "models/popelnice/popelnice");
            rio.Position = new Vector3(200, 0, 200);
            Components.Add(tank);
            Components.Add(model);
            Components.Add(rio);
            
            // Components registration
            bloom = new BloomComponent(this);
            Components.Add(bloom);

            audioManager = new AudioManager(this);
            Components.Add(audioManager);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize our renderer
            DebugShapeRenderer.Initialize(GraphicsDevice);
            fpsCameraPosition = rio.EyePosition;
            
            base.Initialize();
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("fonts/hudFont");
            background = Content.Load<Texture2D>("images/sunset");
            grid = Content.Load<Model>("models/grid/grid");
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            UpdateCamera(gameTime);
            UpdateProjectiles(gameTime);
                    
            base.Update(gameTime);
        }

        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                cameraArc += time * 0.025f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                cameraArc -= time * 0.025f;
            }

            cameraArc += currentGamePadState.ThumbSticks.Right.Y * time * 0.05f;

            // Limit the arc movement.
            if (cameraArc > 90.0f)
                cameraArc = 90.0f;
            else if (cameraArc < -90.0f)
                cameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                cameraRotation += time * 0.05f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                cameraRotation -= time * 0.05f;
            }

            cameraRotation += currentGamePadState.ThumbSticks.Right.X * time * 0.1f;

            // Check for input to zoom camera in and out.
            if (currentKeyboardState.IsKeyDown(Keys.Z))
                cameraDistance += time * 0.25f;

            if (currentKeyboardState.IsKeyDown(Keys.X))
                cameraDistance -= time * 0.25f;

            cameraDistance += currentGamePadState.Triggers.Left * time * 0.5f;
            cameraDistance -= currentGamePadState.Triggers.Right * time * 0.5f;

            // Limit the camera distance.
            if (cameraDistance > 5000)
                cameraDistance = 5000;
            else if (cameraDistance < 10)
                cameraDistance = 10;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraArc = -45;
                cameraRotation = 0;
                cameraDistance = 1200;
            }
        }

        /// <summary>
        /// Helper for updating the explosions effect.
        /// </summary>
        void UpdateExplosions(GameTime gameTime)
        {
            timeToNextProjectile -= gameTime.ElapsedGameTime;

            if (timeToNextProjectile <= TimeSpan.Zero)
            {
                // Create a new projectile once per second. The real work of moving
                // and creating particles is handled inside the Projectile class.
                //projectiles.Add(new Projectile(explosionParticles,
                //                               explosionSmokeParticles,
                //                               projectileTrailParticles));

                timeToNextProjectile += TimeSpan.FromSeconds(1);
            }
        }

        /// <summary>
        /// Helper for updating the list of active projectiles.
        /// </summary>
        void UpdateProjectiles(GameTime gameTime)
        {
            int i = 0;

            while (i < projectiles.Count)
            {
                if (!projectiles[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    projectiles.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        public void addProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }
        
        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            Viewport viewport = device.Viewport;

            bloom.BeginDraw();

            device.Clear(Color.Black);

            // Draw the background image.
            spriteBatch.Begin(0, BlendState.Opaque);            
            spriteBatch.Draw(background,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.White);            
            spriteBatch.End();

            // Set camera
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;            
            if (fpsCamera)
            {
                // FPS camera
                Matrix rotationMatrix = Matrix.CreateRotationY(rio.Rotation);
                Vector3 transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);
                Vector3 position = rio.EyePosition;
                fpsCameraPosition += Vector3.Transform(position, rotationMatrix) * rio.Speed;
                fpsCameraTarget = transformedReference + position;
                
                view = Matrix.CreateLookAt(position, fpsCameraTarget, Vector3.Up);

                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                        aspectRatio,
                                                                        1, 10000);
            }
            else
            {
                // Movable, zoomable and rotateable camera
                view = Matrix.CreateTranslation(0, -25, 0) *
                              Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                              Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                              Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                                  new Vector3(0, 0, 0), Vector3.Up);

                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                        aspectRatio,
                                                                        1, 10000);
            }

            // Draw light
            if (rotateLight)
            {
                lightRotation +=
                    (float)gameTime.ElapsedGameTime.TotalSeconds * LightRotationSpeed;
            }
            Matrix lightRotationMatrix = Matrix.CreateRotationY(lightRotation);
            lightPosition = new Vector3(LightRotationRadius, LightHeight, 0);
            lightPosition = Vector3.Transform(lightPosition, lightRotationMatrix);

            // Draw the ground grid
            DrawGrid(view, projection);

            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(view, projection);
            explosionSmokeParticles.SetCamera(view, projection);
            projectileTrailParticles.SetCamera(view, projection);
            smokePlumeParticles.SetCamera(view, projection);
            fireParticles.SetCamera(view, projection);

            // Draw debug shapes
            DebugShapeRenderer.Draw(gameTime, view, projection);

            // Draw other components (which includes the bloom).
            base.Draw(gameTime);

            // Display some text over the top. Note how we draw this after the bloom,
            // because we don't want the text to be affected by the postprocessing.
            DrawOverlayText();
        }

        /// <summary>
        /// Helper for drawing the background grid model.
        /// </summary>
        void DrawGrid(Matrix view, Matrix projection)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            grid.Draw(Matrix.Identity, view, projection);
        }

        /// <summary>
        /// Displays an overlay showing what the controls are,
        /// and which settings are currently selected.
        /// </summary>
        void DrawOverlayText()
        {
            string text = "V = settings (" + bloom.Settings.Name + ")\n" +
                          "B = toggle bloom (" + (bloom.Visible ? "on" : "off") + ")\n";// +
                          //"Rio VX = " + vx + ")\n" +
                          //"Rio VZ = " + vz + ")\n" +
                          //"Rio Rotation = " + rioRotation + ")\n" +
                          //"Rio Speed = " + rio.Speed + ")\n" +
                          //"Rio Friction = " + rio.Friction + ")\n" +
                          //"Rio Speed - Friction (<0) = " + (rio.Speed - rio.Friction) + ")\n" +
                          //"Rio Speed + Friction (>0) = " + (rio.Speed + rio.Friction) + ")\n" +
                          //"Rio X = " + rio.Position.X + ")\n" +
                          //"Rio Z = " + rio.Position.Z + ")\n";

            spriteBatch.Begin();
            // Draw the string twice to create a drop shadow, first colored black
            // and offset one pixel to the bottom right, then again in white at the
            // intended position. This makes text easier to read over the background.
            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);
            spriteBatch.End();
        }        

        #endregion

        #region Handle Input

        /// <summary>
        /// Handles input for quitting or changing the bloom settings.
        /// </summary>
        private void HandleInput(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            rio.HandleInput(gameTime, lastKeyboardState, lastGamePadState, currentKeyboardState, currentGamePadState);

            // Switch to the next bloom settings preset?
            if ((currentGamePadState.Buttons.A == ButtonState.Pressed &&
                 lastGamePadState.Buttons.A != ButtonState.Pressed) ||
                (currentKeyboardState.IsKeyDown(Keys.V) &&
                 lastKeyboardState.IsKeyUp(Keys.V)))
            {
                bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                bloom.Visible = true;
            }

            // Toggle bloom on or off?
            if ((currentKeyboardState.IsKeyDown(Keys.B) &&
                 lastKeyboardState.IsKeyUp(Keys.B)))
            {
                bloom.Visible = !bloom.Visible;
            }

            // Toggle fullscreen
            if ((currentKeyboardState.IsKeyDown(Keys.F) &&
                 lastKeyboardState.IsKeyUp(Keys.F)))
            {
                graphics.ToggleFullScreen();
            }

            // Toggle fpsCamera on or off?
            if ((currentGamePadState.Buttons.B == ButtonState.Pressed &&
                 lastGamePadState.Buttons.B != ButtonState.Pressed) ||
                (currentKeyboardState.IsKeyDown(Keys.C) &&
                 lastKeyboardState.IsKeyUp(Keys.C)))
            {
                fpsCamera = !fpsCamera;
            }

            // Toggle light rotation on or off?
            if ((currentKeyboardState.IsKeyDown(Keys.L) &&
                 lastKeyboardState.IsKeyUp(Keys.L)))
            {
                rotateLight = !rotateLight;
            }
        }

        #endregion
    }
}
