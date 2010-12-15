using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Octopussy
{
    public class PreferenceManager : ICloneable
    {
        public PlayerPreference PlayerOne
        {
            get;
            set;
        }

        public PlayerPreference PlayerTwo
        {
            get;
            set;
        }

        public PreferenceManager()
        {
            PlayerOne = new PlayerPreference()
            {
                Forward = Keys.W,
                Backward = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
                Shoot = Keys.LeftShift
            };

            PlayerTwo = new PlayerPreference()
            {
                Forward = Keys.I,
                Backward = Keys.K,
                Left = Keys.J,
                Right = Keys.L,
                Shoot = Keys.RightShift
            };
        }

        public object Clone()
        {
            var clone = new PreferenceManager();
            clone.PlayerOne = (PlayerPreference) this.PlayerOne.Clone();
            clone.PlayerTwo = (PlayerPreference) this.PlayerTwo.Clone();

            return clone;
        }
    }

    public class PlayerPreference : ICloneable
    {
        public Keys Forward
        {
            get;
            set;
        }

        public Keys Backward
        {
            get;
            set;
        }

        public Keys Left
        {
            get;
            set;
        }

        public Keys Right
        {
            get;
            set;
        }

        public Keys Shoot
        {
            get;
            set;
        }

        public object Clone()
        {
            var clone = new PlayerPreference();
            clone.Forward = this.Forward;
            clone.Backward = this.Backward;
            clone.Left = this.Left;
            clone.Right = this.Right;
            clone.Shoot = this.Shoot;

            return clone;
        }
    }
}
