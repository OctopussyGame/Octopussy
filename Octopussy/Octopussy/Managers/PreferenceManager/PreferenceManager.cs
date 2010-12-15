using System;
using Microsoft.Xna.Framework.Input;

namespace Octopussy.Managers.PreferenceManager
{
    public class PreferenceManager : ICloneable
    {
        public PreferenceManager()
        {
            PlayerOne = new PlayerPreference
                            {
                                Forward = Keys.W,
                                Backward = Keys.S,
                                Left = Keys.A,
                                Right = Keys.D,
                                Shoot = Keys.LeftShift
                            };

            PlayerTwo = new PlayerPreference
                            {
                                Forward = Keys.I,
                                Backward = Keys.K,
                                Left = Keys.J,
                                Right = Keys.L,
                                Shoot = Keys.RightShift
                            };
        }

// ReSharper disable MemberCanBePrivate.Global
        public PlayerPreference PlayerOne { get; set; }
// ReSharper restore MemberCanBePrivate.Global

// ReSharper disable MemberCanBePrivate.Global
        public PlayerPreference PlayerTwo { get; set; }
// ReSharper restore MemberCanBePrivate.Global

        #region ICloneable Members

        public object Clone()
        {
            var clone = new PreferenceManager();
            clone.PlayerOne = (PlayerPreference) PlayerOne.Clone();
            clone.PlayerTwo = (PlayerPreference) PlayerTwo.Clone();

            return clone;
        }

        #endregion
    }

    public class PlayerPreference : ICloneable
    {
        public Keys Forward { get; set; }

        public Keys Backward { get; set; }

        public Keys Left { get; set; }

        public Keys Right { get; set; }

        public Keys Shoot { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            var clone = new PlayerPreference();
            clone.Forward = Forward;
            clone.Backward = Backward;
            clone.Left = Left;
            clone.Right = Right;
            clone.Shoot = Shoot;

            return clone;
        }

        #endregion
    }
}