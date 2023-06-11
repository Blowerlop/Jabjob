using UnityEngine;

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
        public static readonly Event<Vector3> onPlayerVelocityChangedEvent = new Event<Vector3>(nameof(onPlayerVelocityChangedEvent));
        public static readonly Event<float> onPlayerSpeedChangedEvent = new Event<float>(nameof(onPlayerSpeedChangedEvent));


        // Weapon relative
        public static readonly Event<Weapon> onPlayerWeaponChangedLocalEvent = new Event<Weapon>(nameof(onPlayerWeaponChangedLocalEvent));

        public static readonly Event<byte> onPlayerWeaponChangedServerEvent = new Event<byte>(nameof(onPlayerWeaponChangedServerEvent));

        public static readonly Event<int> onPlayerWeaponAmmoChangedEvent = new Event<int>(nameof(onPlayerWeaponAmmoChangedEvent));
        public static readonly Event<int> onPlayerWeaponTotalAmmoChangedEvent = new Event<int>(nameof(onPlayerWeaponTotalAmmoChangedEvent));

        // Stats relative
        public static readonly Event<ulong , int> onPlayerHealthChangedEvent = new Event<ulong , int>(nameof(onPlayerHealthChangedEvent));
        public static readonly Event<ulong, ulong, int> onPlayerDiedEvent = new Event<ulong, ulong, int>(nameof(onPlayerDiedEvent));
        public static readonly Event<ulong, int> onPlayerGetAKillEvent = new Event<ulong, int>(nameof(onPlayerGetAKillEvent)); 
        public static readonly Event<ulong, int> onPlayerDamageDealtEvent = new Event<ulong, int>(nameof(onPlayerDamageDealtEvent));
        public static readonly Event<ulong, int> onPlayerGetAssistEvent = new Event<ulong, int>(nameof(onPlayerGetAssistEvent));
        public static readonly Event<ulong> onPlayerRespawnedEvent = new Event<ulong>(nameof(onPlayerRespawnedEvent));
        public static readonly Event<ulong> onPlayerSpawnEvent = new Event<ulong>(nameof(onPlayerSpawnEvent));
        public static readonly Event<ulong, StringNetwork> onPlayerUpdateNameEvent = new Event<ulong, StringNetwork>(nameof(onPlayerUpdateNameEvent));
        public static readonly Event<ulong, StringNetwork> onPlayerUpdateModelEvent = new Event<ulong, StringNetwork>(nameof(onPlayerUpdateModelEvent));
        public static readonly Event<ulong, Color> onPlayerUpdateColorEvent = new Event<ulong, Color>(nameof(onPlayerUpdateColorEvent));
        public static readonly Event<ulong, int> onPlayerScoreEvent = new Event<ulong, int>(nameof(onPlayerScoreEvent));
        public static readonly Event<float> onPlayerDashEvent = new Event<float>(nameof(onPlayerDashEvent));

        #endregion


        #region GameManager

        public static readonly Event onGameFinishedEvent = new Event(nameof(onGameFinishedEvent));
        public static readonly Event<ulong> onPlayerJoinGameEvent = new Event<ulong>(nameof(onPlayerJoinGameEvent));
        public static readonly Event<ulong> onPlayerLeaveGameEvent = new Event<ulong>(nameof(onPlayerLeaveGameEvent));
        public static readonly Event<float> onGameTimerUpdated = new Event<float>(nameof(onGameTimerUpdated));
        public static readonly Event onAllPlayersJoinEvent = new Event(nameof(onAllPlayersJoinEvent));

        #endregion


    }
}
