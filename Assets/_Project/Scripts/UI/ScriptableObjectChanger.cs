using Project;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptableObjectChanger : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] scriptableObjects;
    [SerializeField] private GunDisplay gunDisplay;
    private int currentIndex = 0;

    public void ChangeScriptableObject(int change)
    {
        currentIndex += change;
        if (currentIndex < 0) 
            currentIndex = scriptableObjects.Length - 1;
        else if (currentIndex > scriptableObjects.Length - 1)
            currentIndex = 0;

        if (gunDisplay != null)
            gunDisplay.DisplayGun((SOWeapon)scriptableObjects[currentIndex]);
    }

    public void ActivateWeaponPreview()
    {
        ChangeScriptableObject(0);
    }

    public SOWeapon GetWeaponChoose()
    {
        return (SOWeapon)scriptableObjects[currentIndex];
    }
}
