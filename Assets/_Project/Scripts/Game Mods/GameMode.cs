using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [System.Serializable]
    public abstract class GameMode : ScriptableObject
    {
        public string gameModeName;
        public string gameModeDescription;
        public float gameDurationInSeconds;
        public float respawnDurationInSeconds;

        public abstract void Start();
        public abstract void OnDestroy();
    }
}
