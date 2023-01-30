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
    [SerializeField] private Button authenticateButton;
    private string _playerName;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        _playerNameInputField.onEndEdit.AddListener(UpdateInputName);
        _playerNameInputField.onDeselect.AddListener(UpdateInputName);
        authenticateButton.onClick.AddListener(EnterKey);
    }

    private void OnDisable()
    {
        authenticateButton.onClick.RemoveAllListeners();
        _playerNameInputField.onEndEdit.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterKey();
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

    public void EnterKey()
    {
        LobbyManager.Instance.Authenticate(_playerName);
        _playerNameText.text += _playerName;
        Hide();
    }


    private void Hide() {
        gameObject.SetActive(false);
    }

}