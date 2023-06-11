using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class AmmoBoxReload : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
            if (playerShoot != null)
            {
                playerShoot.ReloadTotalAmmo(10);
            }
        }
    }
}
