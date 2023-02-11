using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GunDisplay : MonoBehaviour
{
    [Header("Gun Game")]
    [SerializeField] private TextMeshProUGUI gunName;

    [Header("Gun Stats")]
    [SerializeField] private TextMeshProUGUI maxAmmo;
    [SerializeField] private TextMeshProUGUI shootRate;
    [SerializeField] private TextMeshProUGUI dispertion;

    [Header("Gun Holder")]
    private Transform gunHolder;

    private void Start()
    {
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder").transform;
    }

    public void DisplayGun(SOWeapon gun)
    {
        gunName.text = gun.weaponName;
        maxAmmo.text = gun.maxAmmo.ToString();
        shootRate.text = ShootRateToShootPerSecond(gun.shootRate).ToString();
        dispertion.text = gun.dispersion.ToString();

        if (gunHolder.childCount > 0)
            Destroy(gunHolder.GetChild(0).gameObject);

        Instantiate(gun.model, gunHolder.position, gun.model.transform.rotation, gunHolder);
    }

    private float ShootRateToShootPerSecond(float shootRate)
    {
        return Mathf.Round(1 / shootRate);
    }
}
