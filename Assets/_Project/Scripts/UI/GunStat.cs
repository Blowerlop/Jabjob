using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Assets/SO/Weapon", order = 0)]
public class GunStat : ScriptableObject
{
    [Header("Identity")]
    [Tooltip(@"Current Weapon ID /!\ unique")]
    public int ID;

    [Tooltip(@"Weapon name to display")]
    public string weaponName;

    [Header("Properties"), Tooltip(@"Weapon max ammo in a loader"), Range(0, 50)]
    public int maxAmmo;

    [Tooltip(@"Weapon shotting rate (bullets per sec)"), Range(0, 2)]
    public float shootRate;

    [Tooltip(@"Weapon bullet max dispertion"), Range(0, 15)]
    public float dispertion;

    [Tooltip(@"If user can hold click")]
    public bool riffle;

    [Tooltip(@"Weapon 3D model")]
    public GameObject model;

    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public bool spray;
    [HideInInspector, Tooltip(@"If weapons shoot multiple bullets")]
    public int bulletNumber;
}

[CustomEditor(typeof(GunStat))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var myScript = target as GunStat;

        myScript.spray = GUILayout.Toggle(myScript.spray, "Spray");

        if (myScript.spray)
            myScript.bulletNumber = EditorGUILayout.IntSlider("Bullet Number", myScript.bulletNumber, 1, 30);

    }
}
