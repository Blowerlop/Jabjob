using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Managers;
using Project.Utilities;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;



public class LobbyManager : MonoBehaviour {


    #region Public Attributes
    public static LobbyManager Instance { get; private set; }
    

    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_PLAYER_COLOR = "Color";
    public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_RELAY_GAME = "RelayGameCode";
    public const string KEY_GAMEMAP_NAME = "GameMapName";
    public const string KEY_VIVOX_CHAN_NAME = "VivoxChanName";
    public int maxPlayerLobby = 4;



    public event EventHandler OnLeftLobby;
    public event EventHandler OnStartGame;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }


    public enum GameMode {
        GameMode1,
        GameMode2,
        GameMode3 
    }


    public GameObject PopUpPrefab; 

    #endregion

    #region Private Attribute
    private float heartbeatTimer;
    private float lobbyPollTimer;
    private float refreshLobbyListTimer = 5f;
    public Lobby joinedLobby { get; private set; }
    private string playerName;
    private string playerModel = "Hotdog"; 
    private Color playerColor = Color.white;
    #endregion


    #region Event Vivox

    public Action<string> VivoxOnAuthenticate;
    public Action<string> VivoxOnCreateLobby;
    public Action<string> VivoxOnJoinLobby;
    public Action VivoxOnLeaveLobby;
    #endregion

    private void Awake() {
        Instance = this; 
    }

    private void Start()
    {
        OnJoinedLobby += (sender, args) =>         Debug.Log("Player id in lobby : " + AuthenticationService.Instance.PlayerId);
    }
    
    private void Update() {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
        
        if (Input.GetKey(KeyCode.I)) Debug.Log("Player id" + AuthenticationService.Instance.PlayerId);
    }

    #region LobbyMethod
    public async Task<bool> Authenticate(string playerName) {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log(playerName + " signed in. ID :  " + AuthenticationService.Instance.PlayerId);
            RefreshLobbyList();
        };
        
        try { await AuthenticationService.Instance.SignInAnonymouslyAsync();
            VivoxOnAuthenticate?.Invoke(playerName);
            await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(10)), checkVivox());
            return VivoxManager.Instance.isConnected; }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public async Task<bool> AuthenticateOnlyVivox(string playerName)
    {
        this.playerName = playerName;
        try
        {
            VivoxOnAuthenticate?.Invoke(playerName);
            await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(10)), checkVivox());
            return VivoxManager.Instance.isConnected;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public async Task<bool> checkVivox()
    {
        while (!VivoxManager.Instance.isConnected) await Task.Delay(25);
        return true;
    }

    private void HandleRefreshLobbyList() {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn) {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f) {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    private async void HandleLobbyHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling() {
        if (joinedLobby != null) {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f) {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby()) {
                    Debug.Log("Kicked from Lobby ");

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                    VivoxOnLeaveLobby?.Invoke();
                    joinedLobby = null;
                }

                if(joinedLobby.Data[KEY_RELAY_GAME].Value != "0")
                {
                    if (!IsLobbyHost())
                    {
                        RelayWithLobby.Instance.JoinRelay(joinedLobby.Data[KEY_RELAY_GAME].Value);
                        OnStartGame?.Invoke(this, EventArgs.Empty);
                    }

                    // J'ai du enlevé le fait de rendre null le lobby
                    // Si un joueur prenez plus de T seconds alors le server
                    // qui avait besoins des infos lobby au moment de la connection avec une null ref ce qui poser des
                    // soucis pour la suite du warmup
                    
                    // Timer.StartTimerWithCallbackRealTime(10.0f, () => joinedLobby = null);

                }
            }
        }
    }

    public Lobby GetJoinedLobby() {
        return joinedLobby;
    }

    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private bool IsPlayerInLobby() {
        if (joinedLobby != null && joinedLobby.Players != null) {
            foreach (Player player in joinedLobby.Players) {
                if (player.Id == AuthenticationService.Instance.PlayerId) {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public Player GetPlayer() {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
             { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Hotdog") },
            { KEY_PLAYER_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Color.white.ToString()) },
        });
    }

    public Player GetPlayer(string playerId) {
        return new Player(playerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
             { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Hotdog") },
            { KEY_PLAYER_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, Color.white.ToString()) },
        });
    }

    public void ChangeGameMode() {
        if (IsLobbyHost()) {
            GameMode gameMode =
                Enum.Parse<GameMode>(joinedLobby.Data[KEY_GAME_MODE].Value);

            var values =(GameMode[]) Enum.GetValues(typeof(GameMode));
            var index = Array.IndexOf(values, gameMode);
            gameMode =  values[(index + 1) % values.Length];
            UpdateLobbyGameMode(gameMode);
        }
    }

    public async void CreateLobby(bool isPrivate, GameMode gameMode, string gameMapSceneName)
    {
        try
        {
            Player player = GetPlayer();
            string vivoxChanName = Guid.NewGuid().ToString();
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_RELAY_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") },
                { KEY_GAMEMAP_NAME, new DataObject(DataObject.VisibilityOptions.Public, gameMapSceneName) },
                {KEY_VIVOX_CHAN_NAME, new DataObject(DataObject.VisibilityOptions.Public, vivoxChanName) }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(string.Concat(playerName, "'s Lobby"), maxPlayerLobby, options);
            Debug.Log("Created " + lobby.Name);
            joinedLobby = lobby;
            
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            VivoxOnCreateLobby?.Invoke(vivoxChanName);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            GameObject PopupGO = Instantiate(PopUpPrefab);
            PopupGO.GetComponent<Project.MessagePopUpUI>().Message.text = "Lobby creation failed. Please retry.";
        }

    }
    public async void CreateLobby(string lobbyName, bool isPrivate, GameMode gameMode) {
        Player player = GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = isPrivate,
            Data = new Dictionary<string, DataObject> {
                { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) },
                { KEY_RELAY_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
            }
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayerLobby, options);
        Debug.Log("Created Lobby " + lobby.Name);
        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void RefreshLobbyList() {
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            Project.MessagePopUpUI PopupGO = Instantiate(PopUpPrefab).GetComponent<Project.MessagePopUpUI>();
            PopupGO.Message.text = "Error when refreshing the list. Please check your connectiion and try again without spamming it. You can refresh the list every 20s.";
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        Player player = GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions {
            Player = player
        });

        joinedLobby = lobby;

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
    }

    public async void JoinLobby(Lobby lobby) {
        Player player = GetPlayer();

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
            {
                Player = player
            });

            VivoxOnJoinLobby?.Invoke(joinedLobby.Data[KEY_VIVOX_CHAN_NAME].Value);
        }
        
        catch (Exception e)
        {
            Debug.LogError(e);
            GameObject PopupGO = Instantiate(PopUpPrefab);
            PopupGO.GetComponent<Project.MessagePopUpUI>().Message.text = "Lobby join failed. Please retry.";
        }

        OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        

    }

    public async void UpdatePlayerName(string playerName) {
        this.playerName = playerName;

        if (joinedLobby != null) {
            try {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_NAME, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerCharacter(string playerCharacter)
    {
        this.playerModel = playerCharacter;
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_CHARACTER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerCharacter)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void UpdatePlayerColor(string playerColorHex)
    {
        if (joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_COLOR, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerColorHex)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                Color newColor; ColorUtility.TryParseHtmlString("#" + playerColorHex, out newColor);
                playerColor = newColor;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void QuickJoinLobby() {
        try {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
                VivoxOnLeaveLobby?.Invoke();
            } catch (LobbyServiceException e) {
                Debug.Log(e);
                GameObject PopupGO = Instantiate(PopUpPrefab);
                PopupGO.GetComponent<Project.MessagePopUpUI>().Message.text = "Leaving lobby failed. Please retry.";
            }
        }
    }

    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                Debug.Log("Start Game");
                string relayCode = await RelayWithLobby.Instance.CreateRelay();
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_RELAY_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });
                joinedLobby = lobby;
                Debug.Log(joinedLobby);
                OnStartGame?.Invoke(this, EventArgs.Empty);

                
                // SceneManager.LoadSceneAsyncNetworkServerRpc(SceneManager.EScene.Multi_Lobby); 
                // SceneManager.LoadSceneNetwork(SceneManager.EScene.Multi_Lobby);ù
                SceneManager.LoadSceneNetwork(joinedLobby.Data[KEY_GAMEMAP_NAME].Value);
                
                  
                Debug.Log("Instant log : " + Time.timeSinceLevelLoad);
            } 
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                GameObject PopupGO = Instantiate(PopUpPrefab);
                PopupGO.GetComponent<Project.MessagePopUpUI>().Message.text = "Starting the game failed. Please retry.";
            }
        }
    }
    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
                GameObject PopupGO = Instantiate(PopUpPrefab);
                PopupGO.GetComponent<Project.MessagePopUpUI>().Message.text = "Kicking player failed. Please retry.";
            }
        }
    }

    public async void UpdateLobbyGameMode(GameMode gameMode) { 
        try {
            Debug.Log("UpdateLobbyGameMode " + gameMode);
            
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode.ToString()) }
                }
            });

            joinedLobby = lobby;
            

            OnLobbyGameModeChanged?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public string GetPlayerName() => playerName;
    public Color GetPlayerColor() => playerColor;
    public string GetPlayerModel() => playerModel;
    #endregion
}