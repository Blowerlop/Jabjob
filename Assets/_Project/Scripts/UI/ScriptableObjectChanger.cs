using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectChanger : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] scriptableObjects;
    [SerializeField] private GunDisplay gunDisplay;
    private int currentIndex = 0;

    private void Awake()
    {
        ChangeScriptableObject(0);
    }

    public void ChangeScriptableObject(int change)
    {
        currentIndex += change;
        if (currentIndex < 0) 
            currentIndex = scriptableObjects.Length - 1;
        else if (currentIndex > scriptableObjects.Length - 1)
            currentIndex = 0;

        if (gunDisplay != null)
            gunDisplay.DisplayGun((GunStat)scriptableObjects[currentIndex]);
    }
}
