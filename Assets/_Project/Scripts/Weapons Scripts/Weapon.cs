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
        public Transform bulletStartPoint;
        public int ammo;

        private ParticleSystem firingParticle;
        private void Awake()
        {
            ammo = weaponData.maxAmmo;
            firingParticle = bulletStartPoint.GetComponent<ParticleSystem>();
        }


        private void OnValidate()
        {
            if (weaponData != null && weaponData.prefab != this)
            {
                weaponData.prefab = this;
            }
        }

        public void PlayFiringPart()
        {
            firingParticle.Play();
        }

        public void SetFiringColorPart(Color color)
        {
            var firingPart = firingParticle.main; 
            var firingLeftOverPart = firingParticle.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            var randomColors = new ParticleSystem.MinMaxGradient(color, ColorHelpersUtilities.GetVariantColor(color));
            randomColors.mode = ParticleSystemGradientMode.TwoColors;

            firingPart.startColor = randomColors;
            firingLeftOverPart.startColor = randomColors;
        }
    }
}



