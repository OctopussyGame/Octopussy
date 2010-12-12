#region File Description
//-----------------------------------------------------------------------------
// IAudioEmitter.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

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
    public class Entity : IAudioEmitter
    {
        #region Fields

        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 EyePosition
        {
            get { Vector3 eyePosition = position; eyePosition.Y = eyeHeight; return eyePosition; }
        }
        /// <summary>
        /// Gets or sets the 3D position of the entity.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector3 position;


        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 EyeForward
        {
            get { Vector3 eyeForward = forward; eyeForward.Y = eyeHeight; return eyeForward; }
        }
        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 Forward
        {
            get { return forward; }
            set { forward = value; }
        }
        Vector3 forward;

        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }
        Vector3 up;

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            protected set { velocity = value; }
        }
        Vector3 velocity;

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        float speed;

        /// <summary>
        /// Gets how fast this entity is braking.
        /// </summary>
        public float Friction
        {
            get { return friction; }
        }

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        float rotation;

        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
        public Boolean RotateInTime
        {
            get { return rotateInTime; }
            set { rotateInTime = value; }
        }
        Boolean rotateInTime;

        Model model;
        string modelName;
        Boolean isUsingBumpMap;

        // Settings
        protected float rotationSpeed = 0.005f;
        protected float movementSpeed = 0.01f;
        protected float maxMovementSpeed = 0.9f;
        protected float friction = 0.001f;
        protected float eyeHeight = 100;
        protected float timeRotationSpeed = 0.42f;
        protected float projectileSpeed = 500;
        // the next 4 fields are inputs to the normal mapping effect, and will be set
        // at load time.  change these to change the light properties to modify
        // the appearance of the model.
        Vector4 lightColor = new Vector4(1, 1, 1, 1);
        Vector4 ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
        float shininess = .3f;
        float specularPower = 4.0f;

        protected GameplayScreen screen;

        GameTime gameTime;
        
        #endregion

        #region Initialization

        public Entity(GameplayScreen screen, string modelName, Boolean isUsingBumpMap = false)
        {
            if (screen == null)
                throw new ArgumentNullException("screen");
            if (modelName == null)
                throw new ArgumentNullException("modelName");

            this.screen = screen;

            this.modelName = modelName;
            this.isUsingBumpMap = isUsingBumpMap;
            position = new Vector3(0, 0, 0);
            rotation = 0;
            up = Vector3.Up;
            velocity = Vector3.Zero;
            speed = 0;
            rotateInTime = false;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            model = screen.ScreenManager.Game.Content.Load<Model>(modelName);

            if (isUsingBumpMap)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["LightColor"].SetValue(lightColor);
                        effect.Parameters["AmbientLightColor"].SetValue
                            (ambientLightColor);

                        effect.Parameters["Shininess"].SetValue(shininess);
                        effect.Parameters["SpecularPower"].SetValue(specularPower);
                    }
                }
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        public void UnloadContent()
        {
            
        }

        #endregion

        #region Interaction

        public void TurnLeft(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            rotation += time * rotationSpeed;
            ComputeRotation(gameTime);
        }

        public void TurnRight(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            rotation -= time * rotationSpeed;
            ComputeRotation(gameTime);
        }

        public void Accellerate(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            speed += time * movementSpeed;
        }

        public void Decellerate(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            speed -= time * movementSpeed;
        }

        public void Shoot()
        {
            float vx = ((float)Math.Cos(rotation) * (projectileSpeed));
            float vz = ((float)Math.Sin(rotation) * (projectileSpeed));

            //((Octopussy.Game)Game).audioManager.Play3DSound("sounds/shot", false, this);
            screen.AddProjectile(new Projectile(new Vector3(position.X, 90, position.Z), new Vector3(-vz, 0, -vx), screen.explosionParticles,
                                               screen.explosionSmokeParticles,
                                               screen.projectileTrailParticles));
        }

        private void ComputeRotation(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (MathHelper.ToDegrees(rotation) > 360)
                rotation = 0;
            else if (MathHelper.ToDegrees(rotation) < -360)
                rotation = 0;
        }

        private void ComputeSpeed(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // Apply friction to speed
            if (speed > 0) {
                speed -= time * friction;
                if (speed - friction < 0) {
                    speed = 0;
                }
            } else if (speed < 0) {
                speed += time * friction;
                if (speed + friction > 0) {
                    speed = 0;
                }
            }
            // Limit movement.
            /*if (cameraDistance > 5000)
                cameraDistance = 5000;
            else if (cameraDistance < 10)
                cameraDistance = 10;*/
            // Limit movement speed.
            if (speed > maxMovementSpeed)
                speed = maxMovementSpeed;
            else if (speed < (-maxMovementSpeed))
                speed = -maxMovementSpeed;
        }

        #endregion

        #region Update

        public void HandleInput(KeyboardState lastKeyboardState, GamePadState lastGamePadState,
                         KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            var gameTime = this.gameTime; // this needs rewrite

            // Check for input to rotate left and right.
            if (currentKeyboardState.IsKeyDown(Keys.Left)) {
                TurnLeft(gameTime);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right)) {
                TurnRight(gameTime);
            }

            // Check for input to adjust speed.
            if (currentKeyboardState.IsKeyDown(Keys.Up)) {
                Accellerate(gameTime);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down)) {
                Decellerate(gameTime);
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Space) &&
                 lastKeyboardState.IsKeyUp(Keys.Space))) {
                Shoot();
            }
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            ComputeSpeed(gameTime);

            position.X -= ((float)Math.Sin(rotation) * (time * speed));
            position.Z -= ((float)Math.Cos(rotation) * (time * speed));

            forward.X = ((float)Math.Sin(rotation) * (100));
            forward.Z = ((float)Math.Cos(rotation) * (100));
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            Matrix view = screen.ViewMatrix;
            Matrix projection = screen.ProjectionMatrix;

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            GraphicsDevice device = screen.ScreenManager.Game.GraphicsDevice;
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            Matrix rotationMatrix;
            Matrix positionMatrix = Matrix.CreateTranslation(position.X, position.Y, position.Z);
            if (rotateInTime) {
                rotationMatrix = Matrix.CreateRotationY(time * timeRotationSpeed);
            } else {
                rotationMatrix = Matrix.CreateRotationY(rotation);
            }

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes) {
                if (isUsingBumpMap) {
                    foreach (EffectMaterial effect in mesh.Effects) {
                        Matrix world = transforms[mesh.ParentBone.Index] * rotationMatrix;
                        world *= positionMatrix;

                        effect.Parameters["World"].SetValue(world);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["LightPosition"].SetValue(screen.lightPosition);
                    }
                } else {
                    foreach (BasicEffect effect in mesh.Effects) {
                        Matrix world = transforms[mesh.ParentBone.Index] * rotationMatrix;
                        world *= positionMatrix;

                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.EnableDefaultLighting();

                        // Override the default specular color to make it nice and bright,
                        // so we'll get some decent glints that the bloom can key off.
                        effect.SpecularColor = Vector3.One;
                    }
                }

                mesh.Draw();
            }
        }

        #endregion
    }
}
