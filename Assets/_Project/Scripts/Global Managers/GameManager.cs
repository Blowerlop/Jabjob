using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class GameManager : NetworkBehaviour
    {
        #region Variables

        public static GameManager instance;
        private Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();

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
            }
        }

        #endregion


        #region Methods

        public void AddPlayer(ulong playerNetworkId, Player playerGameObject) =>
            players.Add(playerNetworkId, playerGameObject);

        public Player GetPlayer(ulong playerNetworkId) => players[playerNetworkId];
        
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

