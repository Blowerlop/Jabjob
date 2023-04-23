using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Project.Utilities;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Project
{
    public class Player : NetworkBehaviour, IHealthManagement
    {
        #region Variables

        [SerializeField] private int _health = 100; 
        [SerializeField] [ReadOnlyField] private int _currentHealth = 100;
        private NetworkObject _networkObject;

        [SerializeField] private NetworkVariable<StringNetwork> _nameNetworkVariable = new NetworkVariable<StringNetwork>(new StringNetwork() {value = ""});
        [SerializeField] private NetworkVariable<int> _killsNetworkVariable = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _assistsNetworkVariable = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _deathsNetworkVariable = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<bool> _isHost = new NetworkVariable<bool>();
        [SerializeField] private Player _killer;

        public string playerName { get => _nameNetworkVariable.Value.value; private set => _nameNetworkVariable.Value = new StringNetwork() {value = value}; }
        public int kills { get => _killsNetworkVariable.Value; private set => _killsNetworkVariable.Value = value; }
        public int assists { get => _assistsNetworkVariable.Value; private set => _assistsNetworkVariable.Value = value; }
        public int deaths { get => _deathsNetworkVariable.Value; private set => _deathsNetworkVariable.Value = value; }
        public bool isHost {get => _isHost.Value; private set => _isHost.Value = value;}
        
 
        

        private ulong _damagerId;

        #endregion


        #region Updates
 
        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        public override void OnNetworkSpawn()
        {
            GameEvent.onPlayerJoinGameEvent.Invoke(this, true, OwnerClientId);
            GameManager.instance.AddPlayerLocal(OwnerClientId, this);

            _nameNetworkVariable.OnValueChanged += OnNameValueChange;
            _killsNetworkVariable.OnValueChanged += OnKillValueChange;
            _assistsNetworkVariable.OnValueChanged += OnAssistValueChange;
            _deathsNetworkVariable.OnValueChanged += OnDeathValueChange;
            
            #if UNITY_EDITOR
            _nameNetworkVariable.OnValueChanged += UpdatePlayersGameObjectNameLocal;
            #endif
            
            if (IsOwner == false)
            {
                enabled = false;
                return;
            }

            IsPlayerHostServerRpc();


            UpdatePlayerNameServerRpc(LobbyManager.Instance.GetPlayerName()); 
        }
 
        private void Start()
        {
            _currentHealth = _health;
        }

        private void OnEnable()
        {
            SetHealth(_health);
        }

        public override void OnNetworkDespawn()
        {
            _nameNetworkVariable.OnValueChanged -= OnNameValueChange;
            _killsNetworkVariable.OnValueChanged -= OnKillValueChange;
            _assistsNetworkVariable.OnValueChanged -= OnAssistValueChange;
            _deathsNetworkVariable.OnValueChanged -= OnDeathValueChange;
            
            #if UNITY_EDITOR
            _nameNetworkVariable.OnValueChanged -= UpdatePlayersGameObjectNameLocal;
            #endif
        }

        #endregion 


        #region Methods

        #region Network

        [ServerRpc]
        private void IsPlayerHostServerRpc() => isHost = IsOwner && IsHost;
        private bool IsMyPlayer(ulong playerOwnerId) => OwnerClientId == playerOwnerId;
        
        #endregion
        
        #region Name
        
        [ServerRpc]
        private void UpdatePlayerNameServerRpc(string playerName)
        {
            this.playerName = playerName;
            Debug.Log("New player has joined : Player name : " + this.playerName);

            #if UNITY_EDITOR
            UpdatePlayerGameObjectNameClientRpc();
            #endif
        }
        
        #if UNITY_EDITOR
        [ClientRpc]
        private void UpdatePlayerGameObjectNameClientRpc()
        {
            UpdatePlayersGameObjectNameLocal(new StringNetwork(), new StringNetwork());
        }
        
        public void UpdatePlayersGameObjectNameLocal(StringNetwork previousValue, StringNetwork nextValue)
        {
            StringBuilder newPlayerGameObjectName = new StringBuilder();

            Player[] players = GameManager.instance.GetPlayers();
            
            for (int i = 0; i < players.Length; i++)
            {
                newPlayerGameObjectName.Clear();
                Player player = players[i];
                
                newPlayerGameObjectName.Append(player.playerName);
                
                if (player.isHost) newPlayerGameObjectName.Append(" (Host)");
                if (player.IsOwner) newPlayerGameObjectName.Append(" (You))");
                
                player.name = newPlayerGameObjectName.ToString();
            }
        }
        #endif

        
        
        private void OnNameValueChange(StringNetwork previousValue, StringNetwork nextValue) => GameEvent.onPlayerUpdateNameEvent.Invoke(this, true, OwnerClientId, nextValue);



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
            _damagerId = damagerId;
            SetHealth(_currentHealth - damage);
        }
        
        public void Heal(int heal)
        {
            SetHealth(_currentHealth + heal);
        }
        
        private void SetHealth(int newHealth)
        { 
            _currentHealth = newHealth;
            GameEvent.onPlayerHealthChangedEvent.Invoke(this, false, _currentHealth);
            
            if (_currentHealth <= 0)
            {
                PlayerDeath(); 
            }
        }
        
        #endregion
        
        #region Spawn and Death

        private void PlayerDeath()
        {
            PlayerDeathBehaviourLocal();
            PlayerDeathBehaviourServerRpc(OwnerClientId);
            Debug.Log("You're dead");
        }

        [ServerRpc]
        private void PlayerDeathBehaviourServerRpc(ulong clientId)
        {
            deaths++;
            GameManager.instance.GetPlayer(_damagerId).kills++;
            PlayerDeathBehaviourClientRpc();
            Timer.StartTimerWithCallback(GameManager.instance._respawnDuration, (() => PlayerRespawnClientRpc(clientId)));
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
        
        
        [ClientRpc]
        private void PlayerRespawnClientRpc(ulong clientId)
        {
            PlayerRespawnLocal(clientId);
        }

        private void PlayerRespawnLocal(ulong clientId)
        {
            Player player = GameManager.instance.GetPlayer(clientId);
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
        }
        
        private void OnKillValueChange(int previousValue, int nextValue) => GameEvent.onPlayerGetAKillEvent.Invoke(this, true, OwnerClientId, nextValue);
        private void OnAssistValueChange(int previousValue, int nextValue) => GameEvent.onPlayerGetAssistEvent.Invoke(this, true, OwnerClientId, nextValue);
        private void OnDeathValueChange(int previousValue, int nextValue) => GameEvent.onPlayerDiedEvent.Invoke(this, true, OwnerClientId, nextValue);

        #endregion

        #endregion
    }
}
