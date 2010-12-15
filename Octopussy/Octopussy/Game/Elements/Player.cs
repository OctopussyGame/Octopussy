using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopussy
{
    public class Player : Entity
    {
        // Settings
        protected float rotationSpeed = 0.005f;
        protected float movementSpeed = 0.01f;
        protected float maxMovementSpeed = 0.9f;
        protected float friction = 0.001f;
        protected float eyeHeight = 100;
        protected float timeRotationSpeed = 0.42f;
        protected float projectileSpeed = 500;

        public Player(GameplayScreen screen, string modelName, Boolean isUsingBumpMap = false, Boolean isUsingAlpha = false)
            : base(screen, modelName, isUsingBumpMap, isUsingAlpha)
        {
            
        }
    }
}
