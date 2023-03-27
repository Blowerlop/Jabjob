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
        [field: SerializeField] public SOWeapon weaponData;
        public int ammo;
        private void Awake()
        {
            ammo = weaponData.maxAmmo;
        }

        private void OnValidate()
        {
            if (weaponData != null && weaponData.prefab != this)
            {
                weaponData.prefab = this;
            }
        }
    }
}



