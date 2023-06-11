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

        [SerializeField] private TMP_Text _warmUp;
            
        #endregion


        #region Updates
 
        private void Awake()
        {
            instance = this;
        }
        

        public override void OnDestroy()
        {
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
        }

        private void OnEnable()
        {
            GameEvent.onGameFinishedEvent.Subscribe(EndGameBehaviour, this);
        }
        
        private void OnDisable()
        {
            GameEvent.onGameFinishedEvent.Unsubscribe(EndGameBehaviour);
        }
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SpawnNetworkTimerServerRpc();
                
                GameEvent.onPlayerJoinGameEvent.Subscribe(ALlPlayerJoinEventHandler, this);
                GameEvent.onAllPlayersJoinEvent.Subscribe(EndWarmUpBehaviour, this);
                
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnClientServerRpc;
                
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                
                StartWarmup();

                // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += EndWarmUpBehaviour;
                
            }
        }


        public override void OnNetworkDespawn()
        {
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
            GameEvent.onPlayerSpawnEvent.Invoke(this, false, clientId);
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

        [ClientRpc]
        private void EndGameClientRpc()
        {
            GameEvent.onGameFinishedEvent.Invoke(this, true);
        }

        
        private void EndGameBehaviour()
        {
            if (IsServer)
            {
                Timer.StartTimerWithCallbackRealTime(10.0f, () =>
                {
                    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, @out) =>
                    {
                        NetworkManager.Singleton.Shutdown();
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    };
                        
                    SceneManager.LoadSceneNetwork("MenuScene");
                });
            }

            
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

                    therealone.transform.position = Vector3.zero;
                }
            });


            if (IsServer == false) return;
            

            _networkTimer.StopTimer();
            
            _networkTimer.StartTimerWithCallback(3.0f + 1.0f, () =>
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
        }
        
        #endregion

        [ClientRpc]
        private void DesactiveWarpUpTextClientRpc()
        {
            Timer.StartTimerWithCallbackRealTime(3.0f, () => _warmUp.gameObject.SetActive(false));
        }

        private void ALlPlayerJoinEventHandler(ulong playerId)
        {
            
            if (_players.Keys.Count == LobbyManager.Instance.joinedLobby.Players.Count)
            {
                GameEvent.onAllPlayersJoinEvent.Invoke(this);
            }
        }
    }
}

