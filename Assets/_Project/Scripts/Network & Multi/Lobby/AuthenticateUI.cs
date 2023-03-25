using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {


    public static AuthenticateUI Instance { get; private set; }
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_InputField _playerNameInputField;
    private string _playerName;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _playerNameInputField.onEndEdit.AddListener(UpdateInputName);
        _playerNameInputField.onDeselect.AddListener(UpdateInputName);
    }

    private void OnDisable()
    {
        _playerNameInputField.onEndEdit.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Authenticate();
        }
    }

    public string GetPlayerName()
    {
        return _playerName;
    }
    public void UpdateInputName(string input)
    {
        _playerName = input;
        LobbyManager.Instance.UpdatePlayerName(_playerName);
    }

    public void Authenticate()
    {
        LobbyManager.Instance.Authenticate(_playerName);
        _playerNameText.text += _playerName;
    }
}