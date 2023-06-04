using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public abstract class SOGameMode : ScriptableObject
    {
        public string gameModeName;
        
        public abstract void GameWinCondition();
    }
}
