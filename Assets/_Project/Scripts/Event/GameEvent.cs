using System;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Project
{
    public static class GameEvent
    {
        #region Samples

        // Samples
        public static readonly Event onGlobalEventSample = new Event(nameof(onGlobalEventSample));
        public static readonly Event onLocalEventSample = new Event(nameof(onLocalEventSample));

        #endregion


        #region Player

        // Movement relative
        public static readonly Event<Vector3> onPlayerVelocityChanged =
            new Event<Vector3>(nameof(onPlayerVelocityChanged));

        public static readonly Event<float> onPlayerSpeedChanged = new Event<float>(nameof(onPlayerSpeedChanged));


        // Weapon relative
        public static readonly Event<Weapon> onPlayerWeaponChangedLocal =
            new Event<Weapon>(nameof(onPlayerWeaponChangedLocal));

        public static readonly Event<byte> onPlayerWeaponChangedServer =
            new Event<byte>(nameof(onPlayerWeaponChangedServer));

        public static readonly Event<int> onPlayerWeaponAmmoChanged = new Event<int>(nameof(onPlayerWeaponAmmoChanged));

        // Health relative
        public static readonly Event<int> onPlayerHealthChanged = new Event<int>(nameof(onPlayerHealthChanged));
        public static readonly Event<ulong> onPlayerDied = new Event<ulong>(nameof(onPlayerDied));
        
        #endregion
        
        
        #region GameManager

        public static readonly Event onGameFinished = new Event(nameof(onGameFinished));

        #endregion


    }
}
