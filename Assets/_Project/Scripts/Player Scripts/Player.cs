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

        [SerializeField] [ReadOnlyField] private int _kills;
        [SerializeField] [ReadOnlyField] private int _assists;
        [SerializeField] [ReadOnlyField] private int _deaths;
        [SerializeField] [ReadOnlyField] private Player _killer;
        

        #endregion


        #region Updates

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        public override void OnNetworkSpawn()
        {
            GameManager.instance.AddPlayerLocal(OwnerClientId, this);
            // Debug.Log("My id : " + OwnerClientId);
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

        #region Name

        #endregion
        
        #region  Health Relative
        
        public void Damage(int damage, ulong damagerId)
        {
            DamageServerRpc(damage, damagerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DamageServerRpc(int damage, ulong damagerId)
        {
            ulong ownerId = OwnerClientId;
            DamageClientRpc(damage, damagerId, NetworkUtilities.GetNewClientRpcSenderParams(new List<ulong> {ownerId}));
        }

        [ClientRpc]
        public void DamageClientRpc(int damage, ulong damagerId, ClientRpcParams clientRpcParams)
        {
            DamageLocalClient(damage, damagerId);
        }

        public void DamageLocalClient(int damage, ulong damagerId)
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
            _deaths++;
            PlayerDeathBehaviourLocal();
            PlayerDeathBehaviourServerRpc();
            StartRespawnTimerCoroutineServerRpc(OwnerClientId);
            GameEvent.onPlayerDied.Invoke(this, true, OwnerClientId);
            Debug.Log("You're dead");
        }

        [ServerRpc]
        private void PlayerDeathBehaviourServerRpc()
        {
            PlayerDeathBehaviourClientRpc();
        }

        [ClientRpc]
        private void PlayerDeathBehaviourClientRpc()
        {
            if (IsOwner == false) PlayerDeathBehaviourLocal();
        }

        private void PlayerDeathBehaviourLocal()
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
            Player player = GameManager.instance.GetPlayer(clientId);
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            
        }
        
        
        #endregion

        #endregion
    }
}