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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Octopussy.Game.Elements;
using Octopussy.Game.ParticleSystems;
using Octopussy.Managers.BloomManager;
using Octopussy.Managers.ParticlesManager;
using Octopussy.Managers.ScreenManager;
using Octopussy.Utils;

#endregion

namespace Octopussy.Game.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        private const float LightHeight = 600;
        private const float LightRotationRadius = 800;
        private const float LightRotationSpeed = .9f;
        public const float FogStart = 200;
        public const float FogEnd = 300;
        private const float PresetCleanInterval = 0.5f;
        
        private Vector3 CameraPositionOffset = new Vector3(0, 100, 450);
        private Vector3 CameraTargetOffset = new Vector3(0, 100, 0);
        private readonly List<Entity> entites = new List<Entity>();
        private readonly List<Entity> collideableEntites = new List<Entity>();
        private readonly string playerOneName;
        private readonly string playerTwoName;
        private readonly List<Projectile> projectiles = new List<Projectile>();
        private readonly Random random = new Random();
        private Texture2D background;
        private SoundEffectInstance backgroundSound;

        private BloomComponent bloom;
        private int bloomSettingsIndex;
        private float cameraArc = -45;
        private float cameraDistance = 1200;
        private float cameraRotation = 0;
        private GamePadState currentGamePadState;
        private KeyboardState currentKeyboardState;
        public ParticleSystem explosionParticles;
        public ParticleSystem explosionSmokeParticles;
// ReSharper disable MemberCanBePrivate.Global
        public ParticleSystem fireParticles;
// ReSharper restore MemberCanBePrivate.Global
        private Boolean _fpsCamera;
        private Vector3 fpsCameraPosition;
        private Vector3 topCameraPosition;
        private Vector3 fpsCameraTarget;
        private GraphicsDeviceManager graphics;
        private HeightMapInfo heightMapInfo;

        private GamePadState lastGamePadState;
        private KeyboardState lastKeyboardState;
        public Vector3 lightPosition;
        private float lightRotation;
        private GameMode _mode;

        private Player playerOne;
        private Player ai;

        public ParticleSystem projectileTrailParticles;
        private Matrix projection;
        private const bool rotateLight = true;
        private ParticleSystem smokePlumeParticles;
        private SpriteBatch spriteBatch;
        private Entity surface;
        private Model terrain;
        private TimeSpan timeToNextProjectile = TimeSpan.Zero;
        private Matrix view;
        private Vector3 cameraTargetPosition;
        private Vector3 cameraPosition;
        private TimeSpan presetCleanDelay;

        // Draw matricies
        public Matrix ViewMatrix
        {
            get { return view; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projection; }
        }

        public List<Entity> Entites
        {
            get { return entites; }
        }

        public List<Entity> CollideableEntites
        {
            get { return collideableEntites; }
        }

        public Random Random
        {
            get { return random; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(GameMode mode, string playerNameOne, string playerNameTwo)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            playerOneName = playerNameOne;
            playerTwoName = playerNameTwo;
            this._mode = mode;
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            graphics = (GraphicsDeviceManager) ScreenManager.Game.Services.GetService(typeof (IGraphicsDeviceManager));
            //graphics.ToggleFullScreen();

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

            // Load terrain
            terrain = ScreenManager.Game.Content.Load<Model>("terrain/terrain3");
            // The terrain processor attached a HeightMapInfo to the terrain model's
            // Tag. We'll save that to a member variable now, and use it to
            // calculate the terrain's heights later.
            heightMapInfo = terrain.Tag as HeightMapInfo;
            if (heightMapInfo == null)
            {
                const string message = "The terrain model did not have a HeightMapInfo " +
                                       "object attached. Are you sure you are using the " +
                                       "TerrainProcessor?";
                throw new InvalidOperationException(message);
            }

            // Load entities
            playerOne = new Player(this, "models/rio/rio", playerOneName, 1) {Position = new Vector3(400, 0, -400)};
            ai = new Player(this, "models/rio/rio", playerTwoName, 2) {Position = new Vector3(200, 0, 200)};
            ai.RandomMovementMinimalInterval = 0.01f;
            ai.RandomMovementMaximalnInterval = 0.03f;
            ai.MovesRandomly = true;

            surface = new Entity(this, "models/surface/surface", false, true);
            surface.Alpha = 0.6f;
            surface.MoveInTime = true;
            //surface.MaxMovementSpeed = 1.8f;
            surface.MovementSpeed = 0.0002f;
            surface.Friction = 0.00001f;
            surface.RotationX = MathHelper.ToRadians(180f);
            surface.Position = new Vector3(0, 500, 0);
            surface.IsBoundToHeightMap = false;
            Entites.Add(surface);

            Entites.Add(playerOne);
            Entites.Add(ai);

            AddMultipleInstancesOfEntity("models/egg/egg", 2, true, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassBig1", 30, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassBig2", 30, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassBig3", 30, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassBig4", 30, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassSmall1", 50, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassSmall2", 50, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassSmall3", 50, false, true);
            //AddMultipleInstancesOfEntity("models/grass/grassSmall4", 50, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchSmall1", 80, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchSmall2", 80, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchSmall3", 80, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchBig1", 60, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchBig2", 60, false, true);
            AddMultipleInstancesOfEntity("models/grassBunch/grassBunchBig3", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigBlue", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigGreen", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigPink", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigRed", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigViolet", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassBigYellow", 60, false, true);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderDorangev1", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderDorangev2", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderDorangev3", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderOrangev1", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderOrangev2", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderOrangev3", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderRedv1", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderRedv2", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinderRedv3", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinder2Pink", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinder2Red", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrassCylinder/seaGrassCylinder2Violet", 30, false, true, true, 15);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassMiddleBlue", 65, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassMiddleGreen", 65, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassMiddleRed", 65, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassMiddleViolet", 65, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassMiddleYellow", 65, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassSmallBlue", 40, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassSmallGreen", 40, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassSmallRed", 40, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassSmallViolet", 40, false, true);
            AddMultipleInstancesOfEntity("models/seaGrass/seaGrassSmallYellow", 40, false, true);
            AddMultipleInstancesOfEntity("models/star/seaStarSlim", 85, true, true, false, 15, true);
            AddMultipleInstancesOfEntity("models/shell/shellOpen", 25, true, true);
            AddMultipleInstancesOfEntity("models/shell/shellHalfClose", 25, true, true);
            AddMultipleInstancesOfEntity("models/shell/shellClose", 25, true, true);
            AddMultipleInstancesOfEntity("models/mussel/musselOpen", 25, true, true);
            AddMultipleInstancesOfEntity("models/mussel/musselClose", 25, true, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinBlack", 25, true, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinRed", 25, true, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinWithHairLongBlack", 25, true, true, true, 12, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinWithHairLongRed", 25, true, true, true, 12, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinWithHairShortBlack", 25, true, true, true, 12, true);
            AddMultipleInstancesOfEntity("models/urchin/urchinWithHairShortRed", 25, true, true, true, 12, true);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone1_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone2_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone2_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone2_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone3_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone3_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone3_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone4_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone4_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone4_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone5_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone5_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone5_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone6_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone6_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone6_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone7_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone7_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone7_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone8_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone8_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone8_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone9_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone9_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone9_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone10_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone10_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone10_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone11_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone11_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone11_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone12_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone12_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone12_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone13_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone13_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone13_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone14_tex1", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone14_tex2", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stone14_tex3", 1, true, false, true, 70);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup1_tex1", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup1_tex2", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup1_tex3", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup2_tex1", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup2_tex2", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup2_tex3", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup3_tex1", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup3_tex2", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup3_tex3", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup4_tex1", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup4_tex2", 1, true, false, true, 100);
            AddMultipleInstancesOfEntity("models/stone/stoneGroup4_tex3", 1, true, false, true, 100);

            // Components registration
            bloom = new BloomComponent(ScreenManager.Game);
            ScreenManager.Game.Components.Add(bloom);

            // Enable background sound
            backgroundSound = ScreenManager.AudioManager.Play3DSound("sound/game_background", true, surface);

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            //ScreenManager.Game.Content.Load<SpriteFont>("fonts/hudFont");
            //ScreenManager.Game.Content.Load<Texture2D>("images/hud");
            background = ScreenManager.Game.Content.Load<Texture2D>("images/game/background");

            foreach (Entity entity in Entites)
                entity.LoadContent();

            ScreenManager.Game.ResetElapsedTime();
        }

        private void AddMultipleInstancesOfEntity(String modelName, int count, Boolean isUsingBumpMap = false,
                                                  Boolean isUsingAlpha = false, Boolean isCollideable = false, float collisionRadius = 20, Boolean movesRandomly = false)
        {
            for (int i = 0; i < count; i++)
            {
                Entity entity = InitPositionRotationAndScale(new Entity(this, modelName, isUsingBumpMap, isUsingAlpha, isCollideable, collisionRadius));
                entity.MovesRandomly = movesRandomly; 
                Entites.Add(entity);
                if (isCollideable) CollideableEntites.Add(entity);
            }
        }

        private Entity InitPositionRotationAndScale(Entity entity)
        {
            var heightMapSize = heightMapInfo.HeightMapSize();

            entity.Position = new Vector3(Random.Next((int)heightMapSize.X, (int)heightMapSize.Y), 0, Random.Next((int)heightMapSize.Z, (int)heightMapSize.W));
            entity.Rotation = Random.Next(0, 360);
            entity.Scale = (float) (random.NextDouble() + random.NextDouble() + 0.5f);

            return entity;
        }

        #endregion

        #region UnloadContent

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
            if (!backgroundSound.IsDisposed)
                backgroundSound.Stop(true);

            for (int i = ScreenManager.Game.Components.Count - 1; i > 1; i--)
                ScreenManager.Game.Components.RemoveAt(i);

            foreach (Entity entity in Entites)
                entity.UnloadContent();
        }

        #endregion

        #region Update

        private void setBloomPresetCleaningTimeout()
        {
            presetCleanDelay = TimeSpan.FromSeconds(PresetCleanInterval);
        }

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!_fpsCamera) UpdateCamera(gameTime); // colission with player controls
            UpdateProjectiles(gameTime);

            presetCleanDelay -= gameTime.ElapsedGameTime;
            if (!bloom.Settings.Name.Equals(BloomSettings.PresetSettings[0].Name) && presetCleanDelay < TimeSpan.Zero)
            {
                setBloomPreset("Default");
            }

            foreach (Entity entity in Entites)
                entity.Update(gameTime, heightMapInfo);

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        private void UpdateCamera(GameTime gameTime)
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            /*float newCameraArc = cameraArc;
            float newCameraRotation = cameraRotation;
            float newCameraDistance = cameraDistance;*/

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                //newCameraArc += time * 0.025f;
                CameraPositionOffset.Z -= time * 0.25f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                //newCameraArc -= time * 0.025f;
                CameraPositionOffset.Z += time * 0.25f;
            }

            //cameraArc += currentGamePadState.ThumbSticks.Right.Y*time*0.05f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                //newCameraRotation += time * 0.05f;
                CameraPositionOffset.Y += time * 0.25f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                //newCameraRotation -= time * 0.05f;
                CameraPositionOffset.Y -= time * 0.25f;
            }

            //cameraRotation += currentGamePadState.ThumbSticks.Right.X*time*0.1f;

            // Check for input to zoom camera in and out.
            /*if (currentKeyboardState.IsKeyDown(Keys.Z))
            {
                //newCameraDistance += time*0.25f;
                CameraPositionOffset.Y += time * 0.25f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.X))
            {
                //newCameraDistance -= time*0.25f;
                CameraPositionOffset.Y -= time * 0.25f;
            }*/

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                //newCameraDistance -= time*0.25f;
                CameraPositionOffset = new Vector3(0, 100, 450);
            }

            //cameraDistance += currentGamePadState.Triggers.Left*time*0.5f;
            //cameraDistance -= currentGamePadState.Triggers.Right*time*0.5f;

            // The camera's position depends on the tank's facing direction: when the
            // tank turns, the camera needs to stay behind it. So, we'll calculate a
            // rotation matrix using the tank's facing direction, and use it to
            // transform the two offset values that control the camera.
            Matrix cameraFacingMatrix = Matrix.CreateRotationY(playerOne.Rotation);
            Vector3 positionOffset = Vector3.Transform(CameraPositionOffset,
                cameraFacingMatrix);
            Vector3 targetOffset = Vector3.Transform(CameraTargetOffset,
                cameraFacingMatrix);

            // once we've transformed the camera's position offset vector, it's easy to
            // figure out where we think the camera should be.
            cameraPosition = playerOne.Position + positionOffset;

            // We don't want the camera to go beneath the heightmap, so if the camera is
            // over the terrain, we'll move it up.
            if (heightMapInfo.IsOnHeightmap(cameraPosition))
            {
                // we don't want the camera to go beneath the terrain's height +
                // a small offset.
                float minimumHeight;
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal
                    (cameraPosition, out minimumHeight, out normal);

                minimumHeight += CameraPositionOffset.Y;

                if (cameraPosition.Y < minimumHeight)
                {
                    cameraPosition.Y = minimumHeight;
                }
            }

            // next, we need to calculate the point that the camera is aiming it. That's
            // simple enough - the camera is aiming at the tank, and has to take the 
            // targetOffset into account.
            cameraTargetPosition = playerOne.Position + targetOffset;
        }

        /// <summary>
        /// Helper for updating the explosions effect.
        /// </summary>
        private void UpdateExplosions(GameTime gameTime)
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
        private void UpdateProjectiles(GameTime gameTime)
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

        public void setBloomPreset(String presetName)
        {
            BloomSettings preset = BloomSettings.PresetSettings[0];

            foreach (var currentPreset in BloomSettings.PresetSettings)
            {
                if(currentPreset.Name.Equals(presetName))
                {
                    preset = currentPreset;
                    break;
                }
            }

            bloom.Settings = preset;
            setBloomPresetCleaningTimeout();
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

            device.Clear(new Color(0, 0, 0));

            // Draw the background image.
            DrawBackground(viewport);

            DrawTerrain(terrain);

            // Set camera
            float aspectRatio = viewport.Width / (float) viewport.Height;
            
            if (_fpsCamera)
            {
                // FPS camera
                Matrix rotationMatrix = Matrix.CreateRotationY(playerOne.Rotation);
                Vector3 transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);
                Vector3 position = playerOne.EyePosition;
                fpsCameraPosition += Vector3.Transform(position, rotationMatrix) * playerOne.Speed;
                fpsCameraTarget = transformedReference + position;

                view = Matrix.CreateLookAt(position, fpsCameraTarget, Vector3.Up);
            }
            else
            {
                // Chase camera
                view = Matrix.CreateLookAt(cameraPosition,
                                           cameraTargetPosition, Vector3.Up);
            }

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                 aspectRatio,
                                                                 1, 10000);

            // Draw light
            if (rotateLight)
            {
                lightRotation +=
                    (float) gameTime.ElapsedGameTime.TotalSeconds*LightRotationSpeed;
            }
            Matrix lightRotationMatrix = Matrix.CreateRotationY(lightRotation);
            lightPosition = new Vector3(LightRotationRadius, LightHeight, 0);
            lightPosition = Vector3.Transform(lightPosition, lightRotationMatrix);

            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(view, projection);
            explosionSmokeParticles.SetCamera(view, projection);
            projectileTrailParticles.SetCamera(view, projection);
            smokePlumeParticles.SetCamera(view, projection);
            fireParticles.SetCamera(view, projection);

            foreach (Entity entity in Entites)
                entity.Draw(gameTime);

            // Draw other components (which includes the bloom).
            base.Draw(gameTime);

            playerOne.DrawHUD(gameTime);
            ai.DrawHUD(gameTime);

            // Draw debug shapes
            DebugShapeRenderer.Draw(gameTime, view, projection);
        }

        private void DrawBackground(Viewport viewport)
        {
            spriteBatch.Begin(0, BlendState.Opaque);
            spriteBatch.Draw(background,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        private void DrawTerrain(Model model)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.SpecularPower = 100000;
                    
                    // Set the fog to match the black background color
                    effect.FogEnabled = false;
                    effect.FogColor = Vector3.Zero;
                    effect.FogStart = FogStart;
                    effect.FogEnd = FogEnd;
                }

                mesh.Draw();
            }
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

            var playerIndex = (int) ControllingPlayer.Value;

            currentKeyboardState = input.CurrentKeyboardStates[playerIndex];
            currentGamePadState = input.CurrentGamePadStates[playerIndex];

            // Check for exit.
            if (input.IsPauseGame(ControllingPlayer))
            {
                this.setBloomPreset("Default");
                ScreenManager.AddScreen(new QuitMenuScreen(true), ControllingPlayer);
                return;
            }

            playerOne.HandleInput(lastKeyboardState, lastGamePadState, currentKeyboardState, currentGamePadState);
            ai.HandleInput(lastKeyboardState, lastGamePadState, currentKeyboardState, currentGamePadState);

            //foreach (Entity entity in entites)
            //    entity.HandleInput(lastKeyboardState, lastGamePadState, currentKeyboardState, currentGamePadState);

            // Switch to the next bloom settings preset?
            /*if ((currentGamePadState.Buttons.A == ButtonState.Pressed &&
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
            }*/

             //Toggle fullscreen
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
                _fpsCamera = !_fpsCamera;
            }

            // Toggle light rotation on or off?
            /*if ((currentKeyboardState.IsKeyDown(Keys.L) &&
                 lastKeyboardState.IsKeyUp(Keys.L))) {
                rotateLight = !rotateLight;
            }*/
        }

        #endregion
    }
}