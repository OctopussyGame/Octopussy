#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Octopussy
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;
        
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

        GraphicsDeviceManager graphics;

        List<Entity> entites = new List<Entity>();

        private GameMode mode;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(GameMode mode)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.mode = mode;
        }
        #endregion

        #region LoadContent

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            graphics = (GraphicsDeviceManager)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceManager));

            // Construct our particle system components.
            explosionParticles = new ExplosionParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);
            fireParticles = new FireParticleSystem(ScreenManager.Game, ScreenManager.Game.Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system components.
            ScreenManager.Game.Components.Add(explosionParticles);
            ScreenManager.Game.Components.Add(explosionSmokeParticles);
            ScreenManager.Game.Components.Add(projectileTrailParticles);
            ScreenManager.Game.Components.Add(smokePlumeParticles);
            ScreenManager.Game.Components.Add(fireParticles);

            // Load entities
            model = new Entity(this, "models/lizard/lizard", true);
            model.RotateInTime = true;
            tank = new Entity(this, "models/popelnice/popelnice");
            tank.Position = new Vector3(400, 0, -400);
            tank.RotateInTime = true;
            rio = new Entity(this, "models/popelnice/popelnice");
            rio.Position = new Vector3(200, 0, 200);

            entites.Add(model);
            entites.Add(tank);
            entites.Add(rio);

            fpsCameraPosition = rio.EyePosition;

            // Components registration
            bloom = new BloomComponent(ScreenManager.Game);
            ScreenManager.Game.Components.Add(bloom);

            audioManager = new AudioManager(ScreenManager.Game);
            ScreenManager.Game.Components.Add(audioManager);

            // -----

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/hudFont");
            background = ScreenManager.Game.Content.Load<Texture2D>("images/sunset");
            grid = ScreenManager.Game.Content.Load<Model>("models/grid/grid");

            foreach (var entity in entites)
                entity.LoadContent();

            ScreenManager.Game.ResetElapsedTime();
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
            for (int i = ScreenManager.Game.Components.Count - 1; i > 1; i--)
                ScreenManager.Game.Components.RemoveAt(i);

            foreach (var entity in entites)
                entity.UnloadContent();
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateCamera(gameTime);
            UpdateProjectiles(gameTime);

            foreach (var entity in entites)
                entity.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        private void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.W)) {
                cameraArc += time * 0.025f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S)) {
                cameraArc -= time * 0.025f;
            }

            cameraArc += currentGamePadState.ThumbSticks.Right.Y * time * 0.05f;

            // Limit the arc movement.
            if (cameraArc > 90.0f)
                cameraArc = 90.0f;
            else if (cameraArc < -90.0f)
                cameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.D)) {
                cameraRotation += time * 0.05f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A)) {
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
                currentKeyboardState.IsKeyDown(Keys.R)) {
                cameraArc = -45;
                cameraRotation = 0;
                cameraDistance = 1200;
            }
        }

        /// <summary>
        /// Helper for updating the explosions effect.
        /// </summary>
        private void UpdateExplosions(GameTime gameTime)
        {
            timeToNextProjectile -= gameTime.ElapsedGameTime;

            if (timeToNextProjectile <= TimeSpan.Zero) {
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
        private void UpdateProjectiles(GameTime gameTime)
        {
            int i = 0;

            while (i < projectiles.Count) {
                if (!projectiles[i].Update(gameTime)) {
                    // Remove projectiles at the end of their life.
                    projectiles.RemoveAt(i);
                } else {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        public void AddProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            GraphicsDevice device = ScreenManager.GraphicsDevice;
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
            if (fpsCamera) {
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
            } else {
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
            if (rotateLight) {
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
            
            foreach (var entity in entites)
                entity.Draw(gameTime);

            // Draw other components (which includes the bloom).
            base.Draw(gameTime);

            // Display some text over the top. Note how we draw this after the bloom,
            // because we don't want the text to be affected by the postprocessing.
            DrawOverlayText();
        }

        /// <summary>
        /// Helper for drawing the background grid model.
        /// </summary>
        private void DrawGrid(Matrix view, Matrix projection)
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
        private void DrawOverlayText()
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
        public override void HandleInput(InputState input)
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            int playerIndex = (int)ControllingPlayer.Value;

            currentKeyboardState = input.CurrentKeyboardStates[playerIndex];
            currentGamePadState = input.CurrentGamePadStates[playerIndex];

            // Check for exit.
            if (input.IsPauseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                return;
            }

            rio.HandleInput(lastKeyboardState, lastGamePadState, currentKeyboardState, currentGamePadState);

            // Switch to the next bloom settings preset?
            if ((currentGamePadState.Buttons.A == ButtonState.Pressed &&
                 lastGamePadState.Buttons.A != ButtonState.Pressed) ||
                (currentKeyboardState.IsKeyDown(Keys.V) &&
                 lastKeyboardState.IsKeyUp(Keys.V))) {
                bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
                bloom.Visible = true;
            }

            // Toggle bloom on or off?
            if ((currentKeyboardState.IsKeyDown(Keys.B) &&
                 lastKeyboardState.IsKeyUp(Keys.B))) {
                bloom.Visible = !bloom.Visible;
            }

            // Toggle fullscreen
            if ((currentKeyboardState.IsKeyDown(Keys.F) &&
                 lastKeyboardState.IsKeyUp(Keys.F))) {
                graphics.ToggleFullScreen();
            }

            // Toggle fpsCamera on or off?
            if ((currentGamePadState.Buttons.B == ButtonState.Pressed &&
                 lastGamePadState.Buttons.B != ButtonState.Pressed) ||
                (currentKeyboardState.IsKeyDown(Keys.C) &&
                 lastKeyboardState.IsKeyUp(Keys.C))) {
                fpsCamera = !fpsCamera;
            }

            // Toggle light rotation on or off?
            if ((currentKeyboardState.IsKeyDown(Keys.L) &&
                 lastKeyboardState.IsKeyUp(Keys.L))) {
                rotateLight = !rotateLight;
            }
        }

        #endregion
    }
}
