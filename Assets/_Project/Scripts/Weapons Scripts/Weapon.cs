using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Project
{
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] public SOWeapon weaponData { get; private set; }
        public WeaponStruct @struct = new WeaponStruct();

    }


    public struct WeaponStruct : INetworkSerializeByMemcpy
    {
        public Weapon azeaz;
    }
}



