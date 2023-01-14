using UnityEngine;

namespace Project
{
    public static class GameEvent
    {
        public static GlobalEvent onGlobalEvent = new GlobalEvent();
        public static OwnerEvent onOwnerEvent = new OwnerEvent();
    }
}
