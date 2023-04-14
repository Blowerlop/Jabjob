using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
public class MainUI : MonoBehaviour
{
    [SerializeField] private VivoxManager vivoxManager;

    [SerializeField] private GameObject _logginPanel;
    [SerializeField] private GameObject _textChanelPanel;
    private void Awake()
    {
        vivoxManager.OnUserLoggedIn += OnUserLoggedIn;
    }

    

    private void OnUserLoggedIn()
    {
        _logginPanel.SetActive(false);
        _textChanelPanel.SetActive(true);
    }
}
