using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private WeaponSO _defaultWeapon;
    private WeaponSO _currentWeapon;
    [SerializeField] private Transform _weaponHandler;
    #endregion


    #region Updates
    private void Start()
    {
        EquipWeapon(_defaultWeapon);
    }
    #endregion

    #region Methods
    private void EquipWeapon(WeaponSO weapon)
    {
        UnequipWeapon();
        Instantiate(weapon.model, _weaponHandler);
        _currentWeapon = weapon;
        Debug.Log("Equipping Weapon !");
    }

    private void UnequipWeapon()
    {
        Destroy(_currentWeapon);
    }
    #endregion
}
