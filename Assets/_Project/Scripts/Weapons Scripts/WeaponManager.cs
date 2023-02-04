using System.Collections.Generic;
using Project;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class WeaponManager : NetworkBehaviour, IGameEventListener
{
    #region Variables
    [SerializeField] private Weapon _defaultWeapon;
    [SerializeField] [ReadOnlyField] private Weapon _currentWeapon;
    [SerializeField] private Transform _weaponHandler;
    #endregion


    #region Updates
    private void Start()
    {
        EquipWeaponLocal(_defaultWeapon);
    }
    
    
    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        
    }
    
    #endregion

    #region Methods


    /*
    public void EquipWeapon(Weapon weapon)
    {
        EquipWeaponLocal(weapon);
        EquipWeaponServerRpc(weapon);
    }
    
    [ServerRpc]
    private void EquipWeaponServerRpc(Weapon weapon)
    {
        EquipWeaponClientRpc(weapon);
    }
    
    [ClientRpc]
    private void EquipWeaponClientRpc(Weapon weapon)
    {
        if (IsOwner == false) EquipWeaponLocal(weapon);
    }*/
    
    public void EquipWeaponLocal(Weapon weapon)
    {
        UnequipWeapon();
        _currentWeapon = Instantiate(weapon, _weaponHandler);
        Debug.Log("Equipping Weapon !");
    }

    private void UnequipWeapon()
    {
        if (_currentWeapon == null) return;
        
        Destroy(_currentWeapon.gameObject);
    }

    public Weapon GetCurrentWeapon() => _currentWeapon;

    #endregion

    
}


#if UNITY_EDITOR
[CustomEditor(typeof(WeaponManager)), CanEditMultipleObjects]
public class WeaponManagerEditor : Editor
{
    public Object weapon;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        WeaponManager t = target as WeaponManager;

        weapon = EditorGUILayout.ObjectField(weapon, typeof(Weapon), true);


        if (GUILayout.Button("EquipWeapon")) 
        {
            t.EquipWeaponLocal(weapon as Weapon);
        }
    }
}
#endif