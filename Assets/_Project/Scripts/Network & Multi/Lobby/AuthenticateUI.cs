using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticateUI : MonoBehaviour {


    public static AuthenticateUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private Button authenticateButton;
    private string _playerName;

    private void Awake()
    {
        Instance = this;
        authenticateButton.onClick.AddListener(() => {
            EnterKey();
        });
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

    void EnterKey()
    {
        LobbyManager.Instance.Authenticate(_playerName);
        _playerNameText.text += _playerName;
        Hide();
    }


    private void Hide() {
        gameObject.SetActive(false);
    }

}