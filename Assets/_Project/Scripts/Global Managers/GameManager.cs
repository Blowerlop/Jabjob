using System;
using System.Collections.Generic;
using System.Text;
using Project.Utilities;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SceneManager = _Project.Scripts.Managers.SceneManager;
using Timer = Project.Utilities.Timer;

namespace Project
{
    public class GameManager : NetworkBehaviour
    {
        #region Variables

        public static GameManager instance;
        [SerializeField] private Transform _playerPrefab;
        [ReadOnlyField] private readonly Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();

        //[Header("Game Settings")]
        // [SerializeField] private SOGameSettings _gameSettings --> Preview changement futur
        [field: SerializeField] public GameMode gameMode { get; private set; }
        [field: SerializeField] public NetworkTimer _networkTimer { get; private set; }

        public NetworkList<Vector3> possibleSpawnPositions;
        public List<Vector3> playerSpawnPositions = new List<Vector3>();
 
        [SerializeField] private TMP_Text _warmUp;
        public bool gameHasStarted = false;
        [SerializeField] bool firstBlood;
        [SerializeField] int announcerGameStep = 0;
        #endregion
        

        #region Updates

        private void Awake()
        {
            instance = this;

            possibleSpawnPositions = new NetworkList<Vector3>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

        }
        

        [ServerRpc(RequireOwnership = false)]
        public void RemoveSpawnPointServerRpc(Vector3 spawnPoint) => possibleSpawnPositions.Remove(spawnPoint);
        [ServerRpc(RequireOwnership = false)]
        public void AddSpawnPointServerRpc(Vector3 spawnPoint) => possibleSpawnPositions.Add(spawnPoint);
        

        public override void OnDestroy()
        {
            if (GetComponent<NetworkObject>().IsSpawned)
            {
                GetComponent<NetworkObject>().Despawn(true);
            }
            
            base.OnDestroy();
            
            if (IsServer)
            {
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.SceneManager.OnLoadComplete -= SpawnClientServerRpc;
                    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                }
            }
            
            gameMode.OnDestroy();
            GameEvent.onGameTimerUpdated.Unsubscribe(AnnouncerLinkedToTimer);
        }

        public override void OnNetworkSpawn()
        {
            GameEvent.onGameFinishedEvent.Subscribe(EndGameBehaviour, this);

            if (IsServer)
            {
                playerSpawnPositions.ForEach(x =>
                {
                    possibleSpawnPositions.Add(x);
                });
                
                SpawnNetworkTimerServerRpc();
                
                GameEvent.onPlayerJoinGameEvent.Subscribe(ALlPlayerJoinEventHandler, this);
                GameEvent.onAllPlayersJoinEvent.Subscribe(EndWarmUpBehaviour, this);

                
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnClientServerRpc;
                
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                
                Timer.StartTimerWithCallbackRealTime(2.0f, StartWarmup);

                // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += EndWarmUpBehaviour;
                
            }
        }


        public override void OnNetworkDespawn()
        {
            GameEvent.onGameFinishedEvent.Unsubscribe(EndGameBehaviour);

            if (IsServer)
            {
                GameEvent.onPlayerJoinGameEvent.Unsubscribe(ALlPlayerJoinEventHandler);
                GameEvent.onAllPlayersJoinEvent.Unsubscribe(EndWarmUpBehaviour);            

                
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= SpawnClientServerRpc;
                
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        #endregion 


        #region Methods

        #region ManageClients
        
        [ServerRpc]
        private void SpawnClientServerRpc(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            Transform playerTransform = Instantiate((_playerPrefab));
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        private void OnClientDisconnect(ulong clientId) => GameEvent.onPlayerLeaveGameEvent.Invoke(this, true, clientId);

        public void AddPlayerLocal(ulong playerNetworkId, Player player) => _players.Add(playerNetworkId, player);

        public Player GetPlayer(ulong playerNetworkId) => _players[playerNetworkId];
        
        public Player[] GetPlayers()
        {
            Player[] players = new Player[_players.Count];

            int index = 0;
            foreach (KeyValuePair<ulong, Player> kvp in _players)
            {
                players[index] = kvp.Value;
                index++;
            }
            
            return players;
        }

        public ulong GetPlayerId(string playerName)
        {
            foreach (var kvp in _players)
            {
                if (kvp.Value.playerName == playerName)
                {
                    return kvp.Key;
                }
            }

            Debug.LogError($"No player with the name {playerName} has been found");
            return ulong.MaxValue;
        }
        
        #endregion
        
        
        
        [ServerRpc]
        private void SpawnNetworkTimerServerRpc()
        {
            _networkTimer = Instantiate(_networkTimer);
            _networkTimer.GetComponent<NetworkObject>().Spawn();
        }


        [ServerRpc]
        public void EndGameServerRpc() => EndGameClientRpc();
        
        [ClientRpc]
        private void EndGameClientRpc()
        {
            GameEvent.onGameFinishedEvent.Invoke(this, true);
        }

        
        private void EndGameBehaviour()
        {
            Debug.Log("OnLoadComplete end game");
            
            Timer.StartTimerWithCallbackRealTime(10.0f, () =>
            {
                if (IsServer)
                {
                    SceneManager.LoadSceneNetwork("MenuScene");
                }
                
                NetworkManager.Singleton.SceneManager.OnLoadComplete += (id, sceneName, mode) =>
                {
                    if (IsServer)
                    {           
                        // This will let you know when a load is completed
                        // Server Side: receives this notification for both itself and all clients
                        if (id == NetworkManager.Singleton.LocalClientId)
                        {
                            SoundManager2D.instance.PlayBackgroundMusic("Start Scene Background Music");
                            CursorManager.instance.Revert();
                        }
                        else
                        {
                            // Handle client LoadComplete **server-side** notifications here
                        }
                    }
                    else // Clients generate this notification locally
                    {
                        SoundManager2D.instance.PlayBackgroundMusic("Start Scene Background Music");
                        CursorManager.instance.Revert();
                    }
                    
                    Debug.Log("EndGameBehaviour OnLoadComplete");

                };
                    
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, @out) =>
                {
                    // SoundManager2D.instance.PlayBackgroundMusic("Start Scene Background Music");
                    // CursorManager.instance.Revert();
                    // RevertPlayerCursorStateServerRpc();
                    //
                    // Debug.Log("OnLoadComplete end game");

                    if (IsServer)
                    {
                        NetworkManager.Singleton.Shutdown();
                    }
                };
            });
        }

        private void StartWarmup()
        {
            Debug.Log("WarmUp starting up !");
            Debug.Log("Players in lobby : " + LobbyManager.Instance.joinedLobby.Players.Count);
            UpdateWarmUpTextClientRpc(null);
            _networkTimer.StartTimerWithCallback(120.0f, () => Debug.Log("Time's up ! Not all the players had been able to join the server !"));
        }
        
        private void EndWarmUpBehaviour()
        {
            if (IsServer)
            {
                Debug.Log("WarmUp is finishing in 30 seconds !");
                UpdateWarmUpTextClientRpc("All players has connected, be ready !");
                _networkTimer.StartTimerWithCallback(30.0f, () =>
                {
                    StartGameClientRpc();
                }, true);

                
            }
        }

        PlayerMovementController therealone;
        PlayerCameraController therealoneother;
        PlayerShoot therealoneshooter; 
        [ClientRpc]
        public void StartGameClientRpc()
        {
            _players.ForEach((key, value) =>
            {
                if (value.GetComponent<PlayerMovementController>().enabled == true)
                {
                    therealone = value.GetComponent<PlayerMovementController>();
                    therealone.enabled = false;
                    therealoneother = value.GetComponent<PlayerCameraController>();
                    therealoneother.enabled = false;
                    therealoneshooter = value.GetComponent<PlayerShoot>();
                    therealoneshooter.enabled = false;
                    therealoneshooter.ReloadTotalAmmoRespawn();

                    GameEvent.onPlayerSpawnEvent.Invoke(this, false, value.GetOwnerId());
                }
            });


            if (IsServer == false) return;
            

            _networkTimer.StopTimer();
            SubscribeAnnouncerServerRpc();
            _networkTimer.StartTimerWithCallback(3.0f + 0.2f, () =>
                {
                    UpdateWarmUpTextClientRpc("Fight !");
                    
                    _networkTimer.StartTimerWithCallback(gameMode.gameDurationInSeconds, EndGameClientRpc,
                        true);
                    gameMode.Start();
                    azdazdClientRpc();

                }, true, true, timer =>
                {
                    UpdateWarmUpTextClientRpc($"Game starting in {timer}... !");
                });
            
            
        }

        [ClientRpc]
        private void UpdateWarmUpTextClientRpc(string text)
        {
            if (_warmUp == null) return;
            _warmUp.gameObject.SetActive(true);

            if (text.IsNullOrEmpty()) return;
            _warmUp.text = text;
        }
            

        [ClientRpc]
        private void azdazdClientRpc()
        {
            DesactiveWarpUpTextClientRpc();

            therealone.enabled = true;
            therealoneother.enabled = true;
            therealoneshooter.enabled = true;
            gameHasStarted = true;
            Debug.Log("gameHasStarted : " + gameHasStarted);
        }
        
        #endregion

        [ClientRpc]
        private void DesactiveWarpUpTextClientRpc()
        {
            Timer.StartTimerWithCallbackRealTime(3.0f, () => _warmUp.gameObject.SetActive(false));
        }

        private void ALlPlayerJoinEventHandler(ulong playerId)
        {
            if (LobbyManager.Instance.joinedLobby == null)
            {
                Debug.LogError($"{this}: JoinedLobby is null");
                return;
            }
            
            if (_players.Keys.Count == LobbyManager.Instance.joinedLobby.Players.Count)
            {
                GameEvent.onAllPlayersJoinEvent.Invoke(this);
                SimpeAnnouncerSoundServerRpc("Welcome");
            }
        }


        public void AskToBeDisconnected(ulong clientId) => DisconnectClientServerRpc(clientId);

        [ServerRpc(RequireOwnership = false)]
        private void DisconnectClientServerRpc(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
        }

        [ServerRpc]
        private void SubscribeAnnouncerServerRpc()
        {
            SubscribeAnnouncerClientRpc();
        }
        [ClientRpc]
        private void SubscribeAnnouncerClientRpc()
        {
            SoundManager2D.instance.PlayAnnouncerSound("Timer3s");
            GameEvent.onGameTimerUpdated.Subscribe(AnnouncerLinkedToTimer, this);
            GameEvent.onPlayerGetAKillEvent.Subscribe(AnnouncerFirstBlood, this);
        }

        private void AnnouncerFirstBlood(ulong cliendId, int killValue)
        {
            if (!firstBlood)
            {
                SoundManager2D.instance.PlayAnnouncerSound("FirstBlood");
                firstBlood = true;
                GameEvent.onPlayerGetAKillEvent.Unsubscribe(AnnouncerFirstBlood);
            }
        }
        private void AnnouncerLinkedToTimer(float timer)
        {
            if (!gameHasStarted) return;
            if (announcerGameStep == 0 && timer < 3f + (gameMode.gameDurationInSeconds * 2f / 3f))
            {
                SoundManager2D.instance.PlayAnnouncerSound("InvisibilityDisabled");
                announcerGameStep++;
            }
            if(announcerGameStep == 1 && timer < 31f)
            {
                SoundManager2D.instance.PlayAnnouncerSound("30sLeft");
                announcerGameStep++;
            }
        }
        [ServerRpc]
        private void SimpeAnnouncerSoundServerRpc(string name)
        {
            SimpeAnnouncerSoundClientRpc(name);
        }
        [ClientRpc]
        private void SimpeAnnouncerSoundClientRpc(string name)
        {
            SoundManager2D.instance.PlayAnnouncerSound(name);
        }
    }
}

