using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }


    [SerializeField] private Button createButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private Button gameModeButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;
    [SerializeField] private TextMeshProUGUI gameModeText;


    private string lobbyName;
    private bool isPrivate;
    private LobbyManager.GameMode gameMode;

    private void Awake() {
        Instance = this;

        createButton.onClick.AddListener(() => {
            LobbyManager.Instance.CreateLobby(
                isPrivate,
                gameMode
            );
            Hide();
        });
        publicPrivateButton.onClick.AddListener(() => {
            isPrivate = !isPrivate;
            UpdateText();
        });

        gameModeButton.onClick.AddListener(() => {
            var values = (LobbyManager.GameMode[])Enum.GetValues(typeof(LobbyManager.GameMode));
            var index = Array.IndexOf(values, gameMode);
            gameMode = values[(index + 1) % values.Length];
            UpdateText();
        });

        Hide();
    }

    private void UpdateText() {
        lobbyNameText.text = lobbyName;
        publicPrivateText.text = isPrivate ? "Private" : "Public";
        gameModeText.text = gameMode.ToString();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
        lobbyName = "Lobby" ;
        isPrivate = false;
        gameMode = LobbyManager.GameMode.GameMode1;

        UpdateText();
    }

}