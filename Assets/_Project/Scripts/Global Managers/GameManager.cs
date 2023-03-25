using System;
using System.Collections.Generic;
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
        [field: SerializeField] public float _respawnDuration { get; private set; } = 2.0f;
        [field: SerializeField] public float _gameDuration { get; private set; }
        [field: SerializeField] public NetworkTimer _networkTimer { get; private set; }
        
        
        #endregion


        #region Updates
 
        private void Awake()
        {
            instance = this;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SpawnNetworkTimerServerRpc();
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnClientServerRpc;
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
        
        public void AddPlayerLocal(ulong playerNetworkId, Player player) => _players.Add(playerNetworkId, player);

        public Player GetPlayer(ulong playerNetworkId) => _players[playerNetworkId];
        
        #endregion
        
        
        
        [ServerRpc]
        private void SpawnNetworkTimerServerRpc()
        {
            _networkTimer = Instantiate(_networkTimer);
            _networkTimer.GetComponent<NetworkObject>().Spawn();
            
            _networkTimer.StartTimerWithCallback(_gameDuration, () => GameEvent.onGameFinished.Invoke(this, true), true);
        }
        #endregion
    }
}

