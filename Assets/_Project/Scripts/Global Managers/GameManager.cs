using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class GameManager : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        public static GameManager instance;
        private Dictionary<ulong, GameObject> players = new Dictionary<ulong, GameObject>();

        [Header("Game Settings")]
        // [SerializeField] private SOGameSettings _gameSettings --> Preview changement futur
        [SerializeField] private float _respawnDuration = 2.0f;
        [field: SerializeField] public float _gameDuration { get; private set; } 
        [SerializeField] private NetworkObject _playerPrefab;
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
            }
        }
        
        

        public void OnEnable()
        {
            GameEvent.onPlayerDied.Subscribe(StartRespawnTimerCoroutineServerRpc, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerDied.Unsubscribe(StartRespawnTimerCoroutineServerRpc);
        } 
        

        #endregion


        #region Methods

        public void AddPlayer(ulong playerNetworkId, GameObject playerGameObject) =>
            players.Add(playerNetworkId, playerGameObject);

        public GameObject GetPlayerGameObject(ulong playerNetworkId) => players[playerNetworkId];
        
        [ServerRpc]
        public void SpawnNetworkTimerServerRpc()
        {
            _networkTimer = Instantiate(_networkTimer);
            _networkTimer.GetComponent<NetworkObject>().Spawn();
            
            _networkTimer.StartTimerWithCallback(_gameDuration, () => GameEvent.onGameFinished.Invoke(this, true), true);
        }

        // [ServerRpc(RequireOwnership = false)] 
        // private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        // {
        //     StartCoroutine(RespawnPlayerTimerCoroutine(clientId));
        // }  
        
        [ServerRpc(RequireOwnership = false)] 
        private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        {
            Timer.StartTimerWithCallback(_respawnDuration, (() => RespawnPlayerTimerCoroutineClientRpc(clientId)));
        }  
        
        
        // private IEnumerator RespawnPlayerTimerCoroutine(ulong clientId)
        // {
        //     yield return new WaitForSeconds(_respawnDuration);
        //     NetworkObject playerInstance = Instantiate(_playerPrefab);
        //     playerInstance.SpawnWithOwnership(clientId, true);
        // }
        
        [ClientRpc]
        private void RespawnPlayerTimerCoroutineClientRpc(ulong clientId)
        {
            GameObject player = GetPlayerGameObject(clientId);
            player.transform.position = Vector3.zero;
            player.SetActive(true);
        }
        

        #endregion
    }
}

