using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI gameMapText;
    [SerializeField] private Image gameMapImage;
    [SerializeField] private Button leaveLobbyButton;
    //[SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Project.GameMapsUI gameMapsUI;
    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });

        startGameButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.StartGame();
        });
        //Un bouton pour changer de gameMode si on en a besoin
        /*
        changeGameModeButton.onClick.AddListener(() => {
            LobbyManager.Instance.ChangeGameMode();
        });
        */
    }

    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        //LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnStartGame += LobbyManager_OnStartGame; 
        Hide();
    }

    private void LobbyManager_OnStartGame(object sender, System.EventArgs e)
    {
        ClearLobby();
        //SoundManager2D.instance.PlayBackgroundMusic("Heads Will Roll");
        transform.root.gameObject.SetActive(false);
    }
    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                LobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Pas d'auto kick
            );
            lobbyPlayerSingleUI.UpdatePlayer(player);
        }
        SetStartButtonVisible(LobbyManager.Instance.IsLobbyHost()); // Seul l'host peut lancer la game

        //changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = "MODE : " + lobby.Data[LobbyManager.KEY_GAME_MODE].Value;
        gameMapText.text = "MAP : " + gameMapsUI.GetMapName(lobby.Data[LobbyManager.KEY_GAMEMAP_NAME].Value) ;
        gameMapImage.sprite = gameMapsUI.GetMapSprite(lobby.Data[LobbyManager.KEY_GAMEMAP_NAME].Value);
        Show();
    }

    private void ClearLobby() {
        if (container == null) return;
        foreach (Transform child in container) {
            if (child == playerSingleTemplate || child == null) continue;
            Destroy(child.gameObject); 
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show() {
        gameObject.SetActive(true); 
    }

    private void SetStartButtonVisible(bool visible)
    {
        startGameButton.gameObject.SetActive(visible);
    }
}