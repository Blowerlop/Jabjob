using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoDisplay;

    [SerializeField] private GameObject player;
    [SerializeField] private GunScript ammo;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ammo = player.GetComponent<GunScript>();
    }

    private void Update()
    {
        ammoDisplay.text = ammo.ammo.ToString();
    }
}
