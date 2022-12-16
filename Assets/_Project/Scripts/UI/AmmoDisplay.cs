using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoDisplay;

    [SerializeField] GameObject player;
    [SerializeField] GunScript ammo;

    private void FixedUpdate()
    {
        if (ammo == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            ammo = player.GetComponent<GunScript>();
        }
    }

    private void Update()
    {
        ammoDisplay.text = ammo.ammo.ToString();
    }
}
