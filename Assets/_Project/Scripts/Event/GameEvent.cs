using UnityEngine;

namespace Project
{
    public static class GameEvent
    {
        #region  Samples
        // Samples
        public static readonly Event onGlobalEventSample = new Event(nameof(onGlobalEventSample));
        public static readonly Event onLocalEventSample = new Event(nameof(onLocalEventSample));
        
        #endregion


        #region  Player
        // Movement relative
        public static readonly Event<Vector3> onPlayerVelocityChange = new Event<Vector3>(nameof(onPlayerVelocityChange));
        public static readonly Event<float> onPlayerSpeedChange = new Event<float>(nameof(onPlayerSpeedChange));

        
        // Weapon relative
        public static readonly Event<Weapon> onPlayerWeaponChange = new Event<Weapon>(nameof(onPlayerWeaponChange));

        #endregion


    }
}
