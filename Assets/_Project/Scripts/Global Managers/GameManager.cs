using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            
        #endregion


        #region Updates
 
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            gameMode.Start();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            gameMode.OnDestroy();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SpawnNetworkTimerServerRpc();
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnClientServerRpc;
                
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
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
            
            _networkTimer.StartTimerWithCallback(gameMode.gameDurationInSeconds, EndGameClientRpc);
        }

        [ClientRpc]
        private void EndGameClientRpc()
        {
            GameEvent.onGameFinishedEvent.Invoke(this, true);
        }
        #endregion
    }
}

