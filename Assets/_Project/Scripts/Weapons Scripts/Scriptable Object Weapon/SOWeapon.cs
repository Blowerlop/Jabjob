using System.Collections.Generic;
using Project;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Assets/SO/Weapon", order =0)]
public class SOWeapon : ScriptableObject
{
    [Header("Identity")]
    [Tooltip(@"Current Weapon ID /!\ unique")]
    public byte ID;

    [Header("Name")]
    [Tooltip(@"Weapon name to display")]
    public string weaponName;

    [Header("Model")]
    [Tooltip(@"Weapon 3D model")]
    public GameObject model;
    
    [Header("Prefab")]
    [Tooltip(@"Weapon prefab")]
    public Weapon prefab;

    [Header("Properties"),Tooltip(@"Weapon max ammo in a loader"), Range(0, 50)]
    public int maxAmmo;

    [Tooltip(@"Weapon shooting rate (bullets per sec)"), Range(0, 2)]
    public float shootRate;

    [Tooltip(@"Weapon bullet max dispersion"), Range(0, 15)]
    public float dispersion;

    [Tooltip(@"If user can hold click")]
    public bool automatic;
    
    [Tooltip(@"The time it takes to reload the weapon")]
    public float reloadDuration;

    [HideInInspector]
    [Tooltip(@"If weapon use raycast instead of real projectiles")]
    public bool raycast;
    
    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public bool spray;

    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public int bulletNumber;
    
    
    
    
    
    
    
    
    
    
    public static Dictionary<byte, SOWeapon> _allSOWeapons = new Dictionary<byte, SOWeapon>();
    
    
    
    private void OnValidate()
    {
        if (prefab != null && prefab.weaponData != this)
        {
            prefab.weaponData = this;
        }
    }


    public static Weapon GetWeaponPrefab(byte id)
    {
        if (_allSOWeapons.TryGetValue(id, out SOWeapon soWeapon))
        {
            return soWeapon.prefab;
        }

        throw new KeyNotFoundException($"The SOWeapon {id} ID has not been added to the database. \n 'Project/Resources/Systems/Database/SOWeaponsDatabase'");
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SOWeapon))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var myScript = target as SOWeapon;

        myScript.raycast = GUILayout.Toggle(myScript.raycast, "Raycast");

        if (myScript.raycast)
        {
            myScript.spray = GUILayout.Toggle(myScript.spray, "Spray");
        }

        if (myScript.raycast && myScript.spray)
            myScript.bulletNumber = EditorGUILayout.IntSlider("Bullet Number",myScript.bulletNumber,1,30);

    }
}
#endif

public class a : IGameEventListener
{
    public void OnEnable()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisable()
    {
        throw new System.NotImplementedException();
    }
}