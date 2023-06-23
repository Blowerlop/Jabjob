using System;
using System.Collections;
using System.Collections.Generic;
using Project;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour {


    public static LobbyCreateUI Instance { get; private set; }

    [SerializeField] private Button closeButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button publicPrivateButton;
    [SerializeField] private Button gameModeButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI publicPrivateText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private GameMapsUI _gameMaps;
    [SerializeField] private GameObject _popUpMessage;


    private string lobbyName;
    private bool isPrivate;
    private LobbyManager.GameMode gameMode;
    public string gameMapSceneName = "";

    private void Awake() {
        Instance = this;
        createButton.onClick.AddListener(() => {
            if (gameMapSceneName == "")
            {
                Debug.Log("Choose a map before creating the lobby !");
                GameObject popUp = Instantiate(_popUpMessage);
                popUp.GetComponent<MessagePopUpUI>().Message.text = "Please choose a map before creating the lobby.";
                return;
            }
            LobbyManager.Instance.CreateLobby(
            isPrivate,
            gameMode,
            gameMapSceneName
            );
            Hide();
        });


        closeButton.onClick.AddListener(() =>
        {
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

        _gameMaps.AddListeners(map => gameMapSceneName = map._gameMap.sceneName);

        Hide();
    }


    private void OnDestroy()
    {
        _gameMaps.RemoveListeners(map => gameMapSceneName = map._gameMap.sceneName);
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
        gameMode = LobbyManager.GameMode.FreeForAll ;

        UpdateText();
    }

}