using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class AmmoBoxReload : MonoBehaviour
    {
        public int AmmoReloaded;
        public float timeToReload;
        public AudioSource audioSource; 
        private void OnTriggerStay(Collider other)
        {
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
            if ( playerShoot != null && playerShoot.enabled && !playerShoot.isMaxAmmo)
            {
                playerShoot.ammoBoxReloadTimer += Time.fixedDeltaTime;
                playerShoot.ShowBarLoadingAmmoBox(playerShoot.ammoBoxReloadTimer / timeToReload);
                if(playerShoot.ammoBoxReloadTimer >= timeToReload)
                {
                    playerShoot.ammoBoxReloadTimer = 0;
                    audioSource.Play();
                    playerShoot.ReloadTotalAmmo(AmmoReloaded);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {

            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
            if (playerShoot != null)
            {
                playerShoot.ammoBoxReloadTimer = 0;
                playerShoot.HideBarloadingAmmoBox();
            }
        }
    }
}
