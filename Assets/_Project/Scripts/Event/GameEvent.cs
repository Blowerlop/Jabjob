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

        public static readonly Event<Vector3> onPlayerVelocityChange = new Event<Vector3>(nameof(onPlayerVelocityChange));
        public static readonly Event<float> onPlayerSpeedChange = new Event<float>(nameof(onPlayerSpeedChange));
        public static readonly Event<bool> onPlayerGroundedStateChange = new Event<bool>(nameof(onPlayerGroundedStateChange));

    }
}
