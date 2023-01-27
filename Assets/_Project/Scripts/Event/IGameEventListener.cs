using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public interface IGameEventListener
    {
        public void OnEnable();
        public void OnDisable();
    }
}