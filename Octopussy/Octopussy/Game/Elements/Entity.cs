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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Octopussy.Game.Screens;
using Octopussy.Managers.SoundManager;

#endregion

namespace Octopussy.Game.Elements
{
    public class Entity : IAudioEmitter
    {
        #region Fields

        private readonly Vector4 _ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
        private readonly Boolean _isUsingAlpha;
        private readonly Boolean _isUsingBumpMap;
        private readonly Vector4 _lightColor = new Vector4(1, 1, 1, 1);
        private readonly string _modelName;
        private float _alpha;
        private const float EyeHeight = 100;
        private Vector3 _forward;
        private float _friction = 0.001f;
        private GameTime _gameTime;
        private const float MaxMovementSpeed = 0.9f;
        private Model _model;
        private float _movementSpeed = 0.01f;
        private Vector3 _position;
        private const float ProjectileSpeed = 500;
        private float _rotation;
        private const float RotationSpeed = 0.005f;
        private float _rotationX;
        private readonly GameplayScreen _screen;
        private const float Shininess = .3f;
        private const float SpecularPower = 4.0f;
        private float _speed;
        private const float TimeRotationSpeed = 0.42f;

        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 EyePosition
        {
            get
            {
                Vector3 eyePosition = _position;
                eyePosition.Y = EyeHeight;
                return eyePosition;
            }
        }


        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
// ReSharper disable UnusedMember.Global
        public Vector3 EyeForward
// ReSharper restore UnusedMember.Global
        {
            get
            {
                Vector3 eyeForward = _forward;
                eyeForward.Y = EyeHeight;
                return eyeForward;
            }
        }

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float Speed
        {
            get { return _speed; }
// ReSharper disable UnusedMember.Global
            set { _speed = value; }
// ReSharper restore UnusedMember.Global
        }

        /// <summary>
        /// Gets how fast this entity is braking.
        /// </summary>
        public float Friction
        {
            get { return _friction; }
            set { _friction = value; }
        }

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public float RotationX
        {
            get { return _rotationX; }
            set { _rotationX = value; }
        }

        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public Boolean RotateInTime { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public Boolean MoveInTime { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        public float MovementSpeed
        {
            get { return _movementSpeed; }
            set { _movementSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the 3D position of the entity.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 Forward
        {
            get { return _forward; }
            set { _forward = value; }
        }

        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public Vector3 Up { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        /// <summary>
        /// Gets or sets how fast this entity is moving.
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public Vector3 Velocity { get; protected set; }
// ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Initialization

        public Entity(GameplayScreen screen, string modelName, Boolean isUsingBumpMap = false,
                      Boolean isUsingAlpha = false)
        {
            if (screen == null)
                throw new ArgumentNullException("screen");
            if (modelName == null)
                throw new ArgumentNullException("modelName");

            this._screen = screen;

            this._modelName = modelName;
            this._isUsingBumpMap = isUsingBumpMap;
            this._isUsingAlpha = isUsingAlpha;
            _position = new Vector3(0, 0, 0);
            _rotation = 0;
            _rotationX = 0;
            _alpha = 1;
            Up = Vector3.Up;
            Velocity = Vector3.Zero;
            _speed = 0;
            RotateInTime = false;
            MoveInTime = false;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public virtual void LoadContent()
        {
            _model = _screen.ScreenManager.Game.Content.Load<Model>(_modelName);

            if (_isUsingBumpMap)
            {
                foreach (ModelMesh mesh in _model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["LightColor"].SetValue(_lightColor);
                        effect.Parameters["AmbientLightColor"].SetValue
                            (_ambientLightColor);

                        effect.Parameters["Shininess"].SetValue(Shininess);
                        effect.Parameters["SpecularPower"].SetValue(SpecularPower);
                    }
                }
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void UnloadContent()
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
        }

        #endregion

        #region Interaction

// ReSharper disable MemberCanBeProtected.Global
        public void TurnLeft(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            _rotation += time*RotationSpeed;
            ComputeRotation(gameTime);
        }

// ReSharper disable MemberCanBeProtected.Global
        public void TurnRight(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            _rotation -= time*RotationSpeed;
            ComputeRotation(gameTime);
        }

// ReSharper disable MemberCanBeProtected.Global
        public void Accellerate(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            _speed += time*_movementSpeed;
        }

// ReSharper disable MemberCanBeProtected.Global
        public void Decellerate(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            _speed -= time*_movementSpeed;
        }

// ReSharper disable MemberCanBeProtected.Global
        public void Shoot()
// ReSharper restore MemberCanBeProtected.Global
        {
            float vx = ((float) Math.Cos(_rotation)*(ProjectileSpeed));
            float vz = ((float) Math.Sin(_rotation)*(ProjectileSpeed));

            _screen.ScreenManager.AudioManager.Play3DSound("sound/tu_mas", false, this);
            _screen.AddProjectile(new Projectile(new Vector3(_position.X, 90, _position.Z), new Vector3(-vz, 0, -vx),
                                                _screen.explosionParticles,
                                                _screen.explosionSmokeParticles,
                                                _screen.projectileTrailParticles));
        }

// ReSharper disable UnusedParameter.Local
        private void ComputeRotation(GameTime gameTime)
// ReSharper restore UnusedParameter.Local
        {
            if (MathHelper.ToDegrees(_rotation) > 360)
                _rotation = 0;
            else if (MathHelper.ToDegrees(_rotation) < -360)
                _rotation = 0;
        }

        private void ComputeSpeed(GameTime gameTime)
        {
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            // Apply friction to speed
            if (_speed > 0)
            {
                _speed -= time*_friction;
                if (_speed - _friction < 0)
                {
                    _speed = 0;
                }
            }
            else if (_speed < 0)
            {
                _speed += time*_friction;
                if (_speed + _friction > 0)
                {
                    _speed = 0;
                }
            }
            // Limit movement.
            /*if (cameraDistance > 5000)
                cameraDistance = 5000;
            else if (cameraDistance < 10)
                cameraDistance = 10;*/
            // Limit movement speed.
            if (_speed > MaxMovementSpeed)
                _speed = MaxMovementSpeed;
            else if (_speed < (-MaxMovementSpeed))
                _speed = -MaxMovementSpeed;
        }

        #endregion

        #region Update

// ReSharper disable UnusedParameter.Global
        public virtual void HandleInput(KeyboardState lastKeyboardState, GamePadState lastGamePadState,
// ReSharper restore UnusedParameter.Global
// ReSharper disable UnusedParameter.Global
                                        KeyboardState currentKeyboardState, GamePadState currentGamePadState)
// ReSharper restore UnusedParameter.Global
        {
            GameTime gameTime = this._gameTime; // this needs rewrite

            // Check for input to rotate left and right.
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                TurnLeft(gameTime);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                TurnRight(gameTime);
            }

            // Check for input to adjust speed.
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                Accellerate(gameTime);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                Decellerate(gameTime);
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Space) &&
                 lastKeyboardState.IsKeyUp(Keys.Space)))
            {
                Shoot();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            this._gameTime = gameTime;
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            ComputeSpeed(gameTime);

            _position.X -= ((float) Math.Sin(_rotation)*(time*_speed));
            _position.Z -= ((float) Math.Cos(_rotation)*(time*_speed));

            _forward.X = ((float) Math.Sin(_rotation)*(100));
            _forward.Z = ((float) Math.Cos(_rotation)*(100));

            if (RotateInTime)
            {
                time = (float) gameTime.TotalGameTime.TotalSeconds;
                _rotation = time*TimeRotationSpeed;
            }

            if (MoveInTime)
            {
                time = (float) gameTime.TotalGameTime.TotalSeconds;
                if (((int) time)%4 == 0)
                {
                    Accellerate(gameTime);
                }
                else if (((int) time)%2 == 0)
                {
                    Decellerate(gameTime);
                }
            }
        }

        #endregion

        #region Draw

        public virtual void Draw(GameTime gameTime)
        {
            Matrix view = _screen.ViewMatrix;
            Matrix projection = _screen.ProjectionMatrix;

            GraphicsDevice device = _screen.ScreenManager.Game.GraphicsDevice;

            device.BlendState = _isUsingAlpha ? BlendState.AlphaBlend : BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            Matrix rotationMatrix = Matrix.CreateRotationY(_rotation);
            Matrix rotationXMatrix = Matrix.CreateRotationX(_rotationX);
            Matrix positionMatrix = Matrix.CreateTranslation(_position.X, _position.Y, _position.Z);

            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in _model.Meshes)
            {
                if (_isUsingBumpMap)
                {
                    foreach (EffectMaterial effect in mesh.Effects)
                    {
                        Matrix world = transforms[mesh.ParentBone.Index]*rotationMatrix;
                        world *= rotationXMatrix;
                        world *= positionMatrix;

                        effect.Parameters["World"].SetValue(world);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["LightPosition"].SetValue(_screen.lightPosition);
                    }
                }
                else
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        Matrix world = transforms[mesh.ParentBone.Index]*rotationMatrix;
                        world *= rotationXMatrix;
                        world *= positionMatrix;

                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.Alpha = _alpha;

                        // Set the fog to match the black background color
                        effect.FogEnabled = true;
                        effect.FogColor = Vector3.Zero;
                        effect.FogStart = 1000;
                        effect.FogEnd = 3200;

                        effect.EnableDefaultLighting();
                        //effect.DirectionalLight0.Direction = screen.lightPosition;

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