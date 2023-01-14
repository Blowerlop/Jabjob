using System;
using UnityEngine;

namespace Project
{
    public interface IGameEventListener
    {
        public void OnEnable();
        public void OnDisable();
    }
}