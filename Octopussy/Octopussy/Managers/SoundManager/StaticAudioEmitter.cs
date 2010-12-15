using Microsoft.Xna.Framework;

namespace Octopussy.Managers.SoundManager
{
    internal class StaticAudioEmitter : IAudioEmitter
    {
        #region Implementation of IAudioEmitter

        public Vector3 Position
        {
            get { return new Vector3(0, 0, 0); }
        }

        public Vector3 Forward
        {
            get { return Vector3.Forward; }
        }

        public Vector3 Up
        {
            get { return Vector3.Up; }
        }

        public Vector3 Velocity
        {
            get { return Vector3.Zero; }
        }

        #endregion
    }
}