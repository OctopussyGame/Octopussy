using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Octopussy.Game.Screens;
using Octopussy.Utils;

namespace Octopussy.Game.Elements
{
    public class Rocket : Entity
    {
        private const float RocketSpeed = 1.5f;
        private const float RocketRotationSpeed = 4f;

        private Player _owner;
        private Boolean _isDead;

        public Rocket(GameplayScreen screen, Player owner) : base(screen, "models/rocket/rocket", true, true, true, 20)
        {
            this._owner = owner;
            this.Position = Vector3.Zero;
            this.HeightOffset = 60;
            this._isDead = false;
        }

        public Player Owner
        {
            get { return _owner; }
        }

        public bool IsDead
        {
            get { return _isDead; }
        }

        public void Die()
        {
            this._isDead = true;
        }

        public void Fly(Vector3 from, float direction)
        {
            this.IsBoundToHeightMap = true;
            this.Position = from;
            this.Rotation = direction;
            this._speed = RocketSpeed;
        }

        public override void Update(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            this._gameTime = gameTime;
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            if (!_isDead) this.RotationZ = time * RocketRotationSpeed;
            
            if (_canCollide) this.UpdateCollision(gameTime, heightMapInfo);

            if (!_isDead)
            {
                _speed = RocketSpeed;
                AdjustToHeightMap(gameTime, heightMapInfo);
            }
            else
            {
                _speed = 0;
            }
            
            this.UpdateBoundingSphere();
            
            this.Moved = false;
        }

        public override void Stop(GameTime gameTime)
        // ReSharper restore MemberCanBeProtected.Global
        {
            this.Die();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_isDead)
            {
                base.Draw(gameTime);
            }
        }

        protected override void OnCollision(Entity entity, GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            if (entity.ModelName.Contains("egg") ||
                entity.ModelName.Contains("stone"))
            {
                this.Die();
            }
        }
    }
}
