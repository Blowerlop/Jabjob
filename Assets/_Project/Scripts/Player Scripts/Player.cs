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

        [SerializeField] private int _defaultHealth = 100;
        [SerializeField] [ReadOnlyField] private int _currentHealth = 100;
        private NetworkObject _networkObject;

        [SerializeField] private NetworkVariable<StringNetwork> _networkName = new NetworkVariable<StringNetwork>(new StringNetwork() { value = "" });
        [SerializeField] private NetworkVariable<Color> _networkColor = new NetworkVariable<Color>();
        [SerializeField] private NetworkVariable<int> _networkKills = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkAssists = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkDeaths = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkScore = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<bool> _networkIsHost = new NetworkVariable<bool>();
        [SerializeField] private Player _killer;

        public string playerName { get => _networkName.Value.value; private set => _networkName.Value = new StringNetwork() { value = value }; }
        public Color playerColor { get => _networkColor.Value; private set => _networkColor.Value = value; }
        public int kills { get => _networkKills.Value; private set => _networkKills.Value = value; }
        public int assists { get => _networkAssists.Value; private set => _networkAssists.Value = value; }
        public int deaths { get => _networkDeaths.Value; private set => _networkDeaths.Value = value; }
        public int score { get => _networkScore.Value; private set => _networkScore.Value = value; }
        public bool isHost { get => _networkIsHost.Value; private set => _networkIsHost.Value = value; }

        private HashSet<ulong> _damagersId = new HashSet<ulong>();
        private ulong _killerId;

        private PlayerShoot _playerShoot;
        private FeedbackManagerUI _feedbackManager; 
        #endregion


        #region Updates

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            _playerShoot = GetComponent<PlayerShoot>();
            _feedbackManager = FindObjectOfType<FeedbackManagerUI>();
        }

        public override void OnNetworkSpawn()
        {
            GameEvent.onPlayerJoinGameEvent.Invoke(this, true, OwnerClientId);
            GameManager.instance.AddPlayerLocal(OwnerClientId, this);

            _networkName.OnValueChanged += OnNameValueChange;
            _networkColor.OnValueChanged += OnColorValueChange;
            _networkKills.OnValueChanged += OnKillValueChange;
            _networkAssists.OnValueChanged += OnAssistValueChange;
            _networkDeaths.OnValueChanged += OnDeathValueChange;
            _networkScore.OnValueChanged += OnScoreValueChange;

#if UNITY_EDITOR
            _networkName.OnValueChanged += UpdatePlayersGameObjectNameLocal;
#endif

            _playerShoot.paintColor = playerColor;
            if (IsOwner == false)
            {
                enabled = false;
                return;
            }

            IsPlayerHostServerRpc();

            UpdatePlayerColorServerRpc(LobbyManager.Instance.GetPlayerColor());
            UpdatePlayerNameServerRpc(LobbyManager.Instance.GetPlayerName());
        }

        private void Start()
        {
            _currentHealth = _defaultHealth;
        }

        private void OnEnable()
        {
            SetHealth(_defaultHealth);
        }

        public override void OnNetworkDespawn()
        {
            _networkName.OnValueChanged -= OnNameValueChange;
            _networkColor.OnValueChanged -= OnColorValueChange;
            _networkKills.OnValueChanged -= OnKillValueChange;
            _networkAssists.OnValueChanged -= OnAssistValueChange;
            _networkDeaths.OnValueChanged -= OnDeathValueChange;
            _networkScore.OnValueChanged -= OnScoreValueChange;

#if UNITY_EDITOR
            _networkName.OnValueChanged -= UpdatePlayersGameObjectNameLocal;
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
        [ServerRpc]
        private void UpdatePlayerColorServerRpc(Color color)
        {
            this.playerColor = color;
            this.transform.GetComponent<PlayerShoot>().paintColor = color;
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

        private void OnColorValueChange(Color previousValue, Color nextValue) { GameEvent.onPlayerUpdateColorEvent.Invoke(this, true, OwnerClientId, nextValue); _playerShoot.paintColor = nextValue; }

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
            DamageClientRpc(damage, damagerId, NetworkUtilities.GetNewClientRpcSenderParams(new List<ulong> { ownerId }));
        }

        [ClientRpc]
        public void DamageClientRpc(int damage, ulong damagerId, ClientRpcParams clientRpcParams)
        {
            DamageLocalClient(damage, damagerId);
        }

        public void DamageLocalClient(int damage, ulong damagerId)
        {
            Debug.Log("You've taken damage");
            _damagersId.Add(damagerId);
            _killerId = damagerId;
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
            ulong[] _damagersIdArray = new ulong[_damagersId.Count];
            _damagersId.CopyTo(_damagersIdArray);
            PlayerDeathFeedBackLocal(_killerId, OwnerClientId, _damagersIdArray);
            PlayerDeathBehaviourServerRpc(OwnerClientId);
            Debug.Log("You're dead");
        }

        [ServerRpc]
        private void PlayerDeathBehaviourServerRpc(ulong clientId)
        {
            deaths++;
            Player killer = GameManager.instance.GetPlayer(_killerId);
            killer.kills++;
            foreach (ulong assistId in _damagersId)
            {
                if (assistId == _killerId) continue;
                Player assistPlayer = GameManager.instance.GetPlayer(assistId);
                assistPlayer.assists++;
                assistPlayer.UpdateScore();
            }
            PlayerDeathBehaviourClientRpc();
            Timer.StartTimerWithCallback(GameManager.instance._respawnDuration, (() => PlayerRespawnClientRpc(clientId)));

            ulong[] _damagersIdArray = new ulong[_damagersId.Count];
            _damagersId.CopyTo(_damagersIdArray);
            PlayerDeathFeedbackClientRpc(_killerId, clientId, _damagersIdArray); 
            _damagersId.Clear();

            killer.UpdateScore();
            UpdateScore();
        }

        [ClientRpc]
        private void PlayerDeathBehaviourClientRpc()
        {
            if (IsOwner == false) PlayerDeathBehaviourLocal();
        }
        [ClientRpc]
        private void PlayerDeathFeedbackClientRpc(ulong killerId, ulong killedId, ulong[] assistPlayers)
        {
            if (IsOwner == false) PlayerDeathFeedBackLocal(killerId, killedId, assistPlayers);
        }
        private void PlayerDeathFeedBackLocal(ulong killerId, ulong killedId, ulong[] assistPlayers)
        {
            _feedbackManager.SendFeedback(killerId, killedId, assistPlayers);
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
        private void OnScoreValueChange(int previousValue, int nextValue) => GameEvent.onPlayerScoreEvent.Invoke(this, true, OwnerClientId, nextValue);
        #endregion
        public int UpdateScore() // scoring de base pourri, peut �tre � changer
        {
            score = 3 * kills - 1 * deaths + 1 * assists; 
            return score; 
        }
        #endregion
    }
}

