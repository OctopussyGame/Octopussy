#region File Description

//-----------------------------------------------------------------------------
// Projectile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Octopussy.Managers.ParticlesManager;

#endregion

namespace Octopussy.Game.Elements
{
    /// <summary>
    /// This class demonstrates how to combine several different particle systems
    /// to build up a more sophisticated composite effect. It implements a rocket
    /// projectile, which arcs up into the sky using a ParticleEmitter to leave a
    /// steady stream of trail particles behind it. After a while it explodes,
    /// creating a sudden burst of explosion and smoke particles.
    /// </summary>
    public class Projectile
    {
        #region Constants

        private const float TrailParticlesPerSecond = 200;
        private const int NumExplosionParticles = 30;
        private const int NumExplosionSmokeParticles = 50;
        private const float ProjectileLifespan = 1.5f;
        //private const float sidewaysVelocityRange = 60;
        //private const float verticalVelocityRange = 40;
        private const float gravity = 15;

        #endregion

        #region Fields

        //private static Random random = new Random();
        private readonly ParticleSystem _explosionParticles;
        private readonly ParticleSystem _explosionSmokeParticles;
        private readonly ParticleEmitter _trailEmitter;
        private float _age;

        private Vector3 _position;
        private Vector3 _velocity;

        #endregion

        /// <summary>
        /// Constructs a new projectile.
        /// </summary>
        public Projectile(Vector3 from, Vector3 startVelocity,
                          ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles,
                          ParticleSystem projectileTrailParticles)
        {
            this._explosionParticles = explosionParticles;
            this._explosionSmokeParticles = explosionSmokeParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            _position = from;

            _velocity.X = startVelocity.X; //(float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            _velocity.Y = startVelocity.Y; //(float)(random.NextDouble() + 0.5) * verticalVelocityRange;
            _velocity.Z = startVelocity.Z; //(float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

            // Use the particle emitter helper to output our trail particles.
            _trailEmitter = new ParticleEmitter(projectileTrailParticles,
                                               TrailParticlesPerSecond, _position);
        }


        /// <summary>
        /// Updates the projectile.
        /// </summary>
        public bool Update(GameTime gameTime)
        {
            var elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            _position += _velocity*elapsedTime;
            _velocity.Y -= elapsedTime*gravity;
            _age += elapsedTime;

            // Update the particle emitter, which will create our particle trail.
            _trailEmitter.Update(gameTime, _position);

            // If enough time has passed, explode! Note how we pass our velocity
            // in to the AddParticle method: this lets the explosion be influenced
            // by the speed and direction of the projectile which created it.
            if (_age > ProjectileLifespan)
            {
                for (int i = 0; i < NumExplosionParticles; i++)
                    _explosionParticles.AddParticle(_position, _velocity);

                for (int i = 0; i < NumExplosionSmokeParticles; i++)
                    _explosionSmokeParticles.AddParticle(_position, _velocity);

                return false;
            }

            return true;
        }
    }
}