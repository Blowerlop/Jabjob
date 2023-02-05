using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class SOWeaponsDatabase : MonoBehaviour
    {
        [SerializeField] private List<SOWeapon> _soWeaponsDatabase;

        private void Start()
        {
            for (int i = 0; i < _soWeaponsDatabase.Count; i++)
            {
                SOWeapon._allSOWeapons.Add(_soWeaponsDatabase[i].ID, _soWeaponsDatabase[i]);
            }
        }
    }
}