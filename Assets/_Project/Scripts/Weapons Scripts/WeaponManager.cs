using System;
using System.Runtime.InteropServices;
using Project;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class WeaponManager : NetworkBehaviour
{
    
    #region Variables
    [SerializeField] private Weapon _defaultWeapon;
    [SerializeField] [ReadOnlyField] private Weapon _currentWeapon;
    private readonly NetworkVariable<byte> _weaponID = new NetworkVariable<byte>(writePerm: NetworkVariableWritePermission.Owner);
    [field: SerializeField] public Transform weaponHandler { get; private set; }
    #endregion


    #region Updates

    private void Awake()
    {
        _weaponID.OnValueChanged += EquipCurrentWeaponNetwork;
    }

    private void Start()
    {
        EquipWeaponLocal(_defaultWeapon); 
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner == false)
        {
            EquipWeaponLocal(_weaponID.Value);
            //enabled = false;
        }

    }

    public override void OnDestroy()
    {
        _weaponID.OnValueChanged -= EquipCurrentWeaponNetwork;

    }
    

    #endregion

    #region Methods

    private void EquipCurrentWeaponNetwork(byte currentValue, byte newValue)
    {
        
        EquipWeaponLocal(newValue);
    }

    public void EquipWeaponLocal(Weapon weapon)
    {
        UnEquipWeapon();
        _currentWeapon = Instantiate(weapon, weaponHandler);
        _weaponID.Value = _currentWeapon.weaponData.ID;
        GameEvent.onPlayerWeaponChangedLocal.Invoke(this, true, _currentWeapon);
        
        Debug.Log("Equipping Weapon !");
        
        // SI on trouve comment serialize un gameobject ou un intptr alors on pourra se passer de l'instancier par le réseau (network object inutile vu que zero communication réseau)
        // NetworkObjectReference --> struct pour référencier un networkgameobject 
    }
    
    private void EquipWeaponLocal(byte weaponID)
    {
        UnEquipWeapon();
        _currentWeapon = Instantiate(SOWeapon.GetWeaponPrefab(weaponID), weaponHandler);
        GameEvent.onPlayerWeaponChangedServer.Invoke(this, true, weaponID);

        Debug.Log("Equipping Weapon !");
        
        // SI on trouve comment serialize un gameobject ou un intptr alors on pourra se passer de l'instancier par le réseau (network object inutile vu que zero communication réseau)
        // NetworkObjectReference --> struct pour référencier un networkgameobject 
    }

    private void UnEquipWeapon()
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
            if (t != null) t.EquipWeaponLocal(weapon as Weapon);
        }
    }
}
#endif



public static class AddressHelper
{
    private static object mutualObject;
    private static ObjectReinterpreter reinterpreter;

    static AddressHelper()
    {
        AddressHelper.mutualObject = new object();
        AddressHelper.reinterpreter = new ObjectReinterpreter();
        AddressHelper.reinterpreter.AsObject = new ObjectWrapper();
    }

    public static IntPtr GetAddress(object obj)
    {
        lock (AddressHelper.mutualObject)
        {
            AddressHelper.reinterpreter.AsObject.Object = obj;
            IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
            AddressHelper.reinterpreter.AsObject.Object = null;
            return address;
        }
    }

    public static T GetInstance<T>(IntPtr address)
    {
        lock (AddressHelper.mutualObject)
        {
            AddressHelper.reinterpreter.AsIntPtr.Value = address;
            T obj = (T)AddressHelper.reinterpreter.AsObject.Object;
            AddressHelper.reinterpreter.AsObject.Object = null;
            return obj;
        }
    }

    // I bet you thought C# was type-safe.
    [StructLayout(LayoutKind.Explicit)]
    private struct ObjectReinterpreter
    {
        [FieldOffset(0)] public ObjectWrapper AsObject;
        [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
    }

    private class ObjectWrapper
    {
        public object Object;
    }

    private class IntPtrWrapper
    {
        public IntPtr Value;
    }
}