using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class Player : NetworkBehaviour, IHealthManagement
    {
        #region Variables
        
        [SerializeField] private int _health = 100; 
        [SerializeField] [ReadOnlyField] private int _currentHealth = 100;
        private NetworkObject _networkObject;

        #endregion


        #region Updates

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        public override void OnNetworkSpawn()
        {
            GameManager.instance.AddPlayer(OwnerClientId, gameObject);
        }

        private void Start()
        {
            _currentHealth = _health;
        }

        private void OnEnable()
        {
            SetHealth(_health);
        }

        #endregion


        #region Methods

        #region  Health Relative
        
        public void Damage(int damage)
        {
            DamageServerRpc(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DamageServerRpc(int damage)
        {
            ulong ownerId = OwnerClientId;
            DamageClientRpc(damage, NetworkUtilities.GetNewClientRpcSenderParams(new List<ulong> {ownerId}));
        }

        [ClientRpc]
        public void DamageClientRpc(int damage, ClientRpcParams clientRpcParams)
        {
            DamageLocalClient(damage);
        }

        public void DamageLocalClient(int damage)
        {
            Debug.Log("You've taken damage");
            SetHealth(_currentHealth - damage);
        }
        
        public void Heal(int heal)
        {
            SetHealth(_currentHealth + heal);
        }
        
        private void SetHealth(int newHealth)
        { 
            _currentHealth = newHealth;
            GameEvent.onPlayerHealthChanged.Invoke(this, false, _currentHealth);
            
            if (_currentHealth <= 0)
            {
                PlayerDeath(); 
            }
        }
        
        #endregion
        
        #region Spawn and Death

        private void PlayerDeath()
        {
            PlayerDeathLocal();
            StartRespawnTimerCoroutineServerRpc(OwnerClientId);
            GameEvent.onPlayerDied.Invoke(this, true, OwnerClientId); 
            PlayerDeathServerRpc();
            Debug.Log("You're dead");
        }

        [ServerRpc]
        private void PlayerDeathServerRpc()
        {
            PlayerDeathClientRpc();
        }

        [ClientRpc]
        private void PlayerDeathClientRpc()
        {
            if (IsOwner == false) gameObject.SetActive(false);
        }

        private void PlayerDeathLocal()
        {
            gameObject.SetActive(false);
        }
        
        [ServerRpc]
        private void StartRespawnTimerCoroutineServerRpc(ulong clientId)
        {
            Timer.StartTimerWithCallback(GameManager.instance._respawnDuration, (() => RespawnPlayerTimerCoroutineClientRpc(clientId)));
        }  
        
        [ClientRpc]
        private void RespawnPlayerTimerCoroutineClientRpc(ulong clientId)
        {
            GameObject player = GameManager.instance.GetPlayerGameObject(clientId);
            player.transform.position = Vector3.zero;
            player.SetActive(true);
        }
        
        
        #endregion

        #endregion
    }
}