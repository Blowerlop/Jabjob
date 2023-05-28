using System.Collections;
using System.Collections.Generic;
using Project;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 

public class AmmoDisplay : MonoBehaviour, IGameEventListener
{
    public float blinkingRate;
    public float redColorTime;


    [SerializeField] private TextMeshProUGUI _ammoDisplay;
    [SerializeField] private Image _lowAmmoImage;
    Color _lowAmmoColor;
    float _lowAmmoAlpha;
    bool isBlinking;
    float blinkingTimer = 0;
    Gradient gradient = new Gradient();

    private void Start()
    {
        gradient = ColorHelpersUtilities.GetGradient(Color.white, Color.red);
        isBlinking = false; 
    }
    public void OnEnable()
    {
        _lowAmmoColor = _lowAmmoImage.color; 
        GameEvent.onPlayerWeaponAmmoChangedEvent.Subscribe(UpdateAmmoDisplayText, this);
    }

    public void OnDisable()
    {
        GameEvent.onPlayerWeaponAmmoChangedEvent.Unsubscribe(UpdateAmmoDisplayText);

    }

    private void Update()
    {
        if(_ammoDisplay.text == "0" ) BlinkingState();
    }
    private void UpdateAmmoDisplayText(int ammo)
    {
        _ammoDisplay.text = ammo.ToString();
        _lowAmmoImage.enabled = ammo < 6;
        _lowAmmoAlpha = (ammo * (120f - 250) / 5f + 250f) /255f;
        _lowAmmoColor = new Color(_lowAmmoColor.r, _lowAmmoColor.g, _lowAmmoColor.b, _lowAmmoAlpha);
        _lowAmmoImage.color = _lowAmmoColor; 
    }


    private void BlinkingState()
    {
        if (isBlinking && blinkingTimer > blinkingRate)
        {
            blinkingTimer = 0;
            _ammoDisplay.color = Color.white; 
            isBlinking = false;
        }
        if (!isBlinking)
        {
            _ammoDisplay.color = gradient.Evaluate(1f - blinkingTimer / redColorTime); 
            if (blinkingTimer > redColorTime)
            {
                blinkingTimer = 0;
                isBlinking = true;
            }
        }
        blinkingTimer += Time.deltaTime;
    }

}
