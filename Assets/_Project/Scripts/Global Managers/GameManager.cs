using System;
using System.Collections;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class GameManager : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        public static GameManager instance;


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
            else
            {
                
            }
        }
        
        [ServerRpc]
        public void SpawnNetworkTimerServerRpc()
        {
            _networkTimer = Instantiate(_networkTimer);
            _networkTimer.GetComponent<NetworkObject>().Spawn();
            
            _networkTimer.StartTimerWithCallback(_gameDuration, () => GameEvent.onGameFinished.Invoke(this, true), true);
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

        // [ServerRpc(RequireOwnership = false)] 
        // private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        // {
        //     StartCoroutine(RespawnPlayerTimerCoroutine(clientId));
        // }  
        
        [ServerRpc(RequireOwnership = false)] 
        private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        {
            Timer.StartTimerWithCallback(_respawnDuration, RespawnPlayerTimerCoroutine, clientId);
        }  
        
        
        // private IEnumerator RespawnPlayerTimerCoroutine(ulong clientId)
        // {
        //     yield return new WaitForSeconds(_respawnDuration);
        //     NetworkObject playerInstance = Instantiate(_playerPrefab);
        //     playerInstance.SpawnWithOwnership(clientId, true);
        // }
        
        private void RespawnPlayerTimerCoroutine(ulong clientId)
        {
            NetworkObject playerInstance = Instantiate(_playerPrefab);
            playerInstance.SpawnWithOwnership(clientId, true);
        }

        #endregion
    }
}

