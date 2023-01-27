using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ammoDisplay;
    [SerializeField] private GunScript _playerGun;

    private void Start()
    {
        _playerGun = transform.root.GetComponent<GunScript>();
    }

    private void Update()
    {
        _ammoDisplay.text = _playerGun.ammo.ToString();
    }
}
