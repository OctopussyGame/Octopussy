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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Octopussy.Game.Screens;
using Octopussy.Managers.BloomManager;
using Octopussy.Managers.SoundManager;
using Octopussy.Utils;

#endregion

namespace Octopussy.Game.Elements
{
    public class Entity : IAudioEmitter
    {
        #region Fields

        private const float EyeHeight = 120;
        private const float ProjectileSpeed = 500;
        private const float Shininess = .3f;
        private const float TimeRotationSpeed = 0.42f;
        private const float CollisionInterval = 0.001f;
        private float _randomMovementRotationProbability = 0.8f;
        private float _randomMovementMinimalInterval = 0.2f;
        private float _randomMovementMaximalnInterval = 4.0f;
        
        private readonly Vector4 _ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
        private readonly Vector4 _lightColor = new Vector4(1, 1, 1, 1);
        private readonly Boolean _isUsingAlpha;
        private readonly Boolean _isUsingBumpMap;
        private readonly GameplayScreen _screen;
        private readonly string _modelName;

        protected GameTime _gameTime;
        private Model _model;
        protected Vector3 _forward;
        protected Vector3 _position;
        protected Boolean _isBoundToHeightMap;
        protected Boolean Moved;
        private TimeSpan collisionDelay = TimeSpan.Zero;
        private TimeSpan randomMovementDelay = TimeSpan.Zero;
        private float _scale;
        private float _alpha;
        private float _friction = 0.01f;
        private float _movementSpeed = 0.02f;
        protected float _rotation;
        private float _rotationX;
        private float _rotationZ;
        protected float _speed;
        private float _rotationSpeed = 0.002f;
        private float _maxMovementSpeed = 0.9f;
        private float _specularPower = 4.0f;
        protected Matrix _orientation = Matrix.Identity;
        protected bool _canCollide;
        private BoundingSphere _boundingSphere;
        private float _collisionRadius;
        private bool _movesRandomly;
        private float _heightOffset;

        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 EyePosition
        {
            get
            {
                Vector3 eyePosition = _position;
                eyePosition.Y += EyeHeight;
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
                eyeForward.Y += EyeHeight;
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

        public bool IsBoundToHeightMap
        {
            get { return _isBoundToHeightMap; }
            set { _isBoundToHeightMap = value; }
        }

        public Model Model
        {
            get { return _model; }
        }

        public string ModelName
        {
            get { return _modelName; }
        }

        public float RotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }

        public float MaxMovementSpeed
        {
            get { return _maxMovementSpeed; }
            set { _maxMovementSpeed = value; }
        }

        public float SpecularPower
        {
            get { return _specularPower; }
            set { _specularPower = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public BoundingSphere BoundingSphere
        {
            get { return _boundingSphere; }
        }

        public bool MovesRandomly
        {
            get { return _movesRandomly; }
            set { _movesRandomly = value; }
        }

        public float RandomMovementMinimalInterval
        {
            get { return _randomMovementMinimalInterval; }
            set { _randomMovementMinimalInterval = value; }
        }

        public float RandomMovementMaximalnInterval
        {
            get { return _randomMovementMaximalnInterval; }
            set { _randomMovementMaximalnInterval = value; }
        }

        public float RandomMovementRotationProbability
        {
            get { return _randomMovementRotationProbability; }
            set { _randomMovementRotationProbability = value; }
        }

        public float HeightOffset
        {
            get { return _heightOffset; }
            set { _heightOffset = value; }
        }

        public float RotationZ
        {
            get { return _rotationZ; }
            set { _rotationZ = value; }
        }

// ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Initialization

        public Entity(GameplayScreen screen, string modelName, Boolean isUsingBumpMap = false,
                      Boolean isUsingAlpha = false, bool canCollide = false, float collisionRadius = 100)
        {
            if (screen == null)
                throw new ArgumentNullException("screen");
            if (modelName == null)
                throw new ArgumentNullException("modelName");

            this.Moved = true;
            this._screen = screen;
            _collisionRadius = collisionRadius;
            _canCollide = canCollide;

            this._modelName = modelName;
            this._isUsingBumpMap = isUsingBumpMap;
            this._isUsingAlpha = isUsingAlpha;
            _position = new Vector3(0, 0, 0);
            _rotation = 0;
            _rotationX = 0;
            RotationZ = 0;
            _alpha = 1;
            _speed = 0;
            _scale = 1;
            Up = Vector3.Up;
            Velocity = Vector3.Zero;
            RotateInTime = false;
            MoveInTime = false;
            _movesRandomly = false;
            _isBoundToHeightMap = true;
            HeightOffset = 0;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public virtual void LoadContent()
        {
            _model = _screen.ScreenManager.Game.Content.Load<Model>(_modelName);

            if (_canCollide)
            {
                this._boundingSphere = new BoundingSphere(_position, _collisionRadius * _scale);
            }

            if (_isUsingBumpMap)
            {
                foreach (ModelMesh mesh in Model.Meshes)
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
            this.Moved = true;
            var time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            _rotation += time * RotationSpeed;
            ComputeRotation(gameTime);
        }

// ReSharper disable MemberCanBeProtected.Global
        public void TurnRight(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            this.Moved = true;
            var time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _rotation -= time * RotationSpeed;
            ComputeRotation(gameTime);
        }

// ReSharper disable MemberCanBeProtected.Global
        public void Accellerate(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            this.Moved = true;
            var time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _speed += time * _movementSpeed;
        }

// ReSharper disable MemberCanBeProtected.Global
        public void Decellerate(GameTime gameTime)
// ReSharper restore MemberCanBeProtected.Global
        {
            this.Moved = true;
            var time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _speed -= time * _movementSpeed;
        }

        // ReSharper disable MemberCanBeProtected.Global
        public virtual void Stop(GameTime gameTime)
        // ReSharper restore MemberCanBeProtected.Global
        {
            this.Moved = true;
            _speed = 0;
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

        protected virtual void ComputeSpeed(GameTime gameTime)
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

            /*if ((currentKeyboardState.IsKeyDown(Keys.Space) &&
                 lastKeyboardState.IsKeyUp(Keys.Space)))
            {
                Shoot();
            }*/
        }

        protected void AdjustToHeightMap(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            var time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            Vector3 newPosition = _position;
            
            newPosition.X -= ((float)Math.Sin(_rotation) * (time * _speed));
            newPosition.Z -= ((float)Math.Cos(_rotation) * (time * _speed));

            _forward.X = ((float)Math.Sin(_rotation) * (100));
            _forward.Z = ((float)Math.Cos(_rotation) * (100));
            _orientation = Matrix.CreateRotationY(_rotation + MathHelper.ToRadians(180));
            
            if (heightMapInfo.IsOnHeightmap(newPosition))
            {
                // now that we know we're on the heightmap, we need to know the correct
                // height and normal at this position.
                Vector3 normal;
                heightMapInfo.GetHeightAndNormal(newPosition,
                    out newPosition.Y, out normal);

                newPosition.Y += _heightOffset;

                // As discussed in the doc, we'll use the normal of the heightmap
                // and our desired forward direction to recalculate our orientation
                // matrix. It's important to normalize, as well.
                _orientation.Up = normal;

                _orientation.Right = Vector3.Cross(_orientation.Forward, _orientation.Up);
                _orientation.Right = Vector3.Normalize(_orientation.Right);

                _orientation.Forward = Vector3.Cross(_orientation.Up, _orientation.Right);
                _orientation.Forward = Vector3.Normalize(_orientation.Forward);

                // once we've finished all computations, we can set our position to the
                // new position that we calculated.
                _position = newPosition;
            }
            else
            {
                Stop(gameTime);
            }
        }

        protected void UpdateBoundingSphere()
        {
            this._boundingSphere.Center = _position;
        }

        protected virtual void OnCollision(Entity entity, GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            //this._isBlocked = true;
        }

        protected Boolean InCollisionWith(Entity entity)
        {
            return entity.BoundingSphere.Intersects(this.BoundingSphere);
        }

        protected void UpdateCollision(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            collisionDelay -= gameTime.ElapsedGameTime;
            if (collisionDelay >= TimeSpan.Zero)
            {
                return;
            }

            foreach (Entity entity in _screen.CollideableEntites)
            {
                if (entity != this && entity.BoundingSphere.Intersects(this.BoundingSphere))
                {
                    this.OnCollision(entity, gameTime, heightMapInfo);
                }
            }

            collisionDelay += TimeSpan.FromSeconds(CollisionInterval);
        }

        private void performRandomMovement(GameTime gameTime)
        {
            randomMovementDelay -= gameTime.ElapsedGameTime;
            if (randomMovementDelay >= TimeSpan.Zero)
            {
                return;
            }

            // Rotate in random direction
            if (_screen.Random.NextDouble() <= _randomMovementRotationProbability)
            {
                int direction = _screen.Random.Next(2);
                for (int i = 0; i < _screen.Random.Next(20); i++)
                {
                    if (direction == 1)
                    {
                        TurnLeft(gameTime);
                    }
                    else
                    {
                        TurnRight(gameTime);
                    }
                }
            }

            // Move for random times
            for (int i = 0; i < _screen.Random.Next(5); i++)
            {
                Accellerate(gameTime);
            }

            randomMovementDelay += TimeSpan.FromSeconds(RandomMovementMinimalInterval + (_screen.Random.NextDouble() * RandomMovementMaximalnInterval));
        }

        public virtual void Update(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            this._gameTime = gameTime;
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            if (MovesRandomly) performRandomMovement(gameTime);

            if (_canCollide) this.UpdateCollision(gameTime, heightMapInfo);
                
            if (MoveInTime)
            {
                if (((int)time) % 4 == 0)
                {
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                    Accellerate(gameTime);
                }
                else if (((int)time) % 2 == 0)
                {
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                    Decellerate(gameTime);
                }
            }

            if (!this.Moved)
            {
                return;
            }

            ComputeSpeed(gameTime);

            if (RotateInTime)
            {
                _rotation = time * TimeRotationSpeed;
            }

            if (_isBoundToHeightMap)
            {
                AdjustToHeightMap(gameTime, heightMapInfo);
            }
            else
            {
                var militime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                
                _position.X -= ((float)Math.Sin(_rotation) * (militime * _speed));
                _position.Z -= ((float)Math.Cos(_rotation) * (militime * _speed));

                _forward.X = ((float)Math.Sin(_rotation) * (100));
                _forward.Z = ((float)Math.Cos(_rotation) * (100));
                _orientation = Matrix.CreateRotationY(_rotation);
            }

            if (_canCollide)
            {
                this.UpdateBoundingSphere();
            }

            this.Moved = false;
        }

        protected void MoveBack(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            Stop(gameTime);
            Decellerate(gameTime);
            ComputeSpeed(gameTime);
            AdjustToHeightMap(gameTime, heightMapInfo);
            Stop(gameTime);
            UpdateBoundingSphere();
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

            Matrix worldMatrix = Matrix.CreateRotationZ(RotationZ) * _orientation * Matrix.CreateScale(_scale) * Matrix.CreateRotationX(_rotationX) * Matrix.CreateTranslation(_position);

            var transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            //if (_canCollide) DebugShapeRenderer.AddBoundingSphere(this.BoundingSphere, Color.Red);
            
            foreach (ModelMesh mesh in Model.Meshes)
            {
                if (_isUsingBumpMap)
                {
                    foreach (EffectMaterial effect in mesh.Effects)
                    {
                        Matrix world = transforms[mesh.ParentBone.Index] * worldMatrix;
                        
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
                        Matrix world = transforms[mesh.ParentBone.Index] * worldMatrix;
                        
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;

                        effect.Alpha = _alpha;

                        // Set the fog to match the black background color
                        effect.FogEnabled = false;
                        effect.FogColor = Vector3.Zero;
                        effect.FogStart = GameplayScreen.FogStart;
                        effect.FogEnd = GameplayScreen.FogEnd;

                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                        //effect.DirectionalLight0.Direction = _screen.lightPosition;

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