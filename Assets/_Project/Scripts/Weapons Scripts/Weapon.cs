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
        public Transform clipTransform;
        public Transform clipLiquid; 
        public int ammo;
        public int totalAmmo;

        private Transform clipParent;
        private Vector3 clipPosition;
        private Quaternion clipRotation;
        private Material liquidMaterial;
        private ParticleSystem firingParticle;

        public Material fakeLiquidMaterial; 
        private void Awake()
        {
            ammo = weaponData.maxAmmo;
            totalAmmo = weaponData.totalAmmo;
            firingParticle = bulletStartPoint.GetComponent<ParticleSystem>();

            clipParent = clipTransform.parent;
            clipPosition = clipTransform.localPosition;
            clipRotation = clipTransform.localRotation;
        }

        private void OnEnable()
        {
            GameEvent.onPlayerWeaponAmmoChangedEvent.Subscribe(UpdateAmmoFillLiquid, this);
            liquidMaterial = clipLiquid.GetComponent<Renderer>().material; 
        }
        private void OnDisable()
        {
            GameEvent.onPlayerWeaponAmmoChangedEvent.Unsubscribe(UpdateAmmoFillLiquid);
        }
        private void OnValidate()
        {
            if (weaponData != null && weaponData.prefab != this)
            {
                weaponData.prefab = this;
            }
        }

        public void UpdateAmmoFillLiquid(int ammoNumber)
        {
            liquidMaterial.SetFloat("_Fill", (float) ammoNumber / (float) weaponData.maxAmmo)  ; 
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
        public void ResetClipPosition()
        {
            clipTransform.SetParent(clipParent);
            clipTransform.localPosition = clipPosition;
            clipTransform.localRotation = clipRotation;
        }

    }
}



