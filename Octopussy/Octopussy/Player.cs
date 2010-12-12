using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopussy
{
    public class Player : Entity
    {
        public Player(GameplayScreen screen, string modelName, Boolean isUsingBumpMap = false) 
            : base(screen, modelName, isUsingBumpMap)
        {
            
        }
    }
}
