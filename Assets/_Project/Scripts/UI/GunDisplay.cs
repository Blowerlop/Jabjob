using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform gunHolder;

    public void DisplayGun(GunStat gun)
    {
        gunName.text = gun.weaponName;
        maxAmmo.text = gun.maxAmmo.ToString();
        shootRate.text = gun.shootRate.ToString();
        dispertion.text = gun.dispertion.ToString();

        if (gunHolder.childCount > 0)
            Destroy(gunHolder.GetChild(0).gameObject);

        Instantiate(gun.model, gunHolder.position, gun.model.transform.rotation, gunHolder);
    }
}
