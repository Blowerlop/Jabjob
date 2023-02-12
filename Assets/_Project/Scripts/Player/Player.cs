using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class Player : NetworkBehaviour, IHealthManagement
    {
        #region Variables
        
        [SerializeField] private int _health = 100;
        private NetworkObject _networkObject;

        #endregion


        #region Updates

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
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
            SetHealth(_health - damage);
        }
        
        public void Heal(int heal)
        {
            SetHealth(_health + heal);
        }
        
        private void SetHealth(int newHealth)
        {
            _health = newHealth;
            GameEvent.onPlayerHealthChanged.Invoke(this, false, _health);
            
            if (_health <= 0)
            {
                PlayerDeath();
            }
        }

        private void PlayerDeath()
        {
            GameEvent.onPlayerDied.Invoke(this, true, OwnerClientId);
            Debug.Log("You're dead");
            PlayerDespawnServerRpc(); 
        }

        [ServerRpc]
        private void PlayerDespawnServerRpc()
        {
            _networkObject.Despawn();
        }
        
        
        
        #endregion

        #endregion
    }
}