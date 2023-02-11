using System;
using System.Collections;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class GameManager : MonoBehaviour, IGameEventListener
    {
        #region Variables

        public static GameManager instance;
        
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
            GameEvent.onPlayerDied.Subscribe(StartRespawnTimerCoroutine, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerDied.Unsubscribe(StartRespawnTimerCoroutine);
        }
        
        #endregion


        #region Methods

        private void StartRespawnTimerCoroutine(ulong clientId)
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

