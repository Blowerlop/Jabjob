using System;
using System.Collections;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class GameManager : NetworkBehaviour, IGameEventListener
    {
        #region Variables

        public static GameManager instance;

        [SerializeField] private NetworkObject _networkObject;
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private float _respawnDuration = 2.0f;
        
        #endregion


        #region Updates

        private void Awake()
        {
            instance = this;
            _networkObject = GetComponent<NetworkObject>();
        }
        
        public void OnEnable()
        {
            GameEvent.onPlayerDied.Subscribe(StartRespawnTimerCoroutineServerRpc, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerDied.Unsubscribe(StartRespawnTimerCoroutineServerRpc);
        }

        public override void OnNetworkSpawn()
        {
            _networkObject.ChangeOwnership(NetworkManager.ServerClientId);
        }

        #endregion


        #region Methods

        [ServerRpc(RequireOwnership = false)]
        private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        {
            StartCoroutine(RespawnPlayerTimerCoroutine(clientId));
        }  
        
        
        private IEnumerator RespawnPlayerTimerCoroutine(ulong clientId)
        {
            yield return new WaitForSeconds(_respawnDuration);
            NetworkObject playerInstance = Instantiate(_playerPrefab);
            playerInstance.SpawnWithOwnership(clientId, true);
        }

        #endregion
    }
}

