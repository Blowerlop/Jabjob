using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LogginPanel : MonoBehaviour
{
    [SerializeField] private VivoxManager _vivoxManager;
    [SerializeField] private TMP_InputField _displayNameTMP;
    [SerializeField] private Button _logginButton;

    private void Awake()
    {
        _logginButton.onClick.AddListener(() =>
        {
            if(_displayNameTMP.text.Length > 0)
                _vivoxManager.Login(_displayNameTMP.text);
        });
    }
}
