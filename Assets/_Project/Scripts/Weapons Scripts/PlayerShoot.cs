using System;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : MonoBehaviour
    {
        #region Variables

        private Weapon _weapon;
        private WeaponManager _weaponManager;

        #endregion


        #region Updates

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
        }

        private void Start()
        {
            _weapon = _weaponManager.GetCurrentWeapon();
        }

        #endregion


        #region Methods

        #endregion
    }
}