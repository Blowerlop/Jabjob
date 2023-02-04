using System;
using System.Collections;
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

    [Tooltip(@"Weapon shotting rate (bullets per sec)"), Range(0, 2)]
    public float shootRate;

    [Tooltip(@"Weapon bullet max dispertion"), Range(0, 15)]
    public float dispertion;

    [Tooltip(@"If user can hold click")]
    public bool riffle;

    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public bool spray;

    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public int bulletNumber;

 

    private void OnEnable()
    {
        if (AllSOWeapons.ContainsKey(ID))
        {
            Debug.LogError(ID + " ID is already used !");
        }
        else
        {
            AllSOWeapons.Add(ID, this);
        }
        
        Debug.Log("Initializing SOWeapons...");
    }

    private void OnDisable()
    {
        AllSOWeapons.Remove(ID);
        Debug.Log("Refreshing SOWeapons...");
    }

    private static Dictionary<byte, SOWeapon> AllSOWeapons = new Dictionary<byte, SOWeapon>();

    public static Weapon GetWeaponPrefab(byte id)
    {
        return AllSOWeapons[id].prefab;
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

        myScript.spray = GUILayout.Toggle(myScript.spray, "Spray");

        if (myScript.spray)
            myScript.bulletNumber = EditorGUILayout.IntSlider("Bullet Number",myScript.bulletNumber,1,30);

    }
}
#endif
