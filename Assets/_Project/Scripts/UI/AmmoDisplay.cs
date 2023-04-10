using System.Collections;
using System.Collections.Generic;
using Project;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour, IGameEventListener
{
    [SerializeField] private TextMeshProUGUI _ammoDisplay;

    
    public void OnEnable()
    {
        GameEvent.onPlayerWeaponAmmoChangedEvent.Subscribe(UpdateAmmoDisplayText, this);
    }

    public void OnDisable()
    {
        GameEvent.onPlayerWeaponAmmoChangedEvent.Unsubscribe(UpdateAmmoDisplayText);

    }

    private void UpdateAmmoDisplayText(int ammo)
    {
        _ammoDisplay.text = ammo.ToString();
    }
}
