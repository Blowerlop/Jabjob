using System;
using System.Collections;
using System.Threading.Tasks;
using Project;
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
        [SerializeField] private float _gameDuration; 
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private float _respawnDuration = 2.0f;
        
        #endregion


        #region Updates

        private void Awake()
        {
            instance = this;
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

}

