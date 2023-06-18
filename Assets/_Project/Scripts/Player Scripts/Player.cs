using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _Project.Scripts.Managers;
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
        public int maxHealth { get => _defaultHealth; private set => _defaultHealth = value; }
        [SerializeField] [ReadOnlyField] private int _currentHealth = 100;
        public int health { get => _currentHealth; } 
        private NetworkObject _networkObject;

        [SerializeField] private NetworkVariable<StringNetwork> _networkName = new NetworkVariable<StringNetwork>(new StringNetwork() { value = "" });
        [SerializeField] private NetworkVariable<StringNetwork> _networkModel = new NetworkVariable<StringNetwork>();
        [SerializeField] private NetworkVariable<Color> _networkColor = new NetworkVariable<Color>();
        [SerializeField] private NetworkVariable<int> _networkKills = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkAssists = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkDeaths = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<ulong> _networkKillerId = new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Owner); 
        [SerializeField] private NetworkVariable<int> _networkScore = new NetworkVariable<int>();
        [SerializeField] private NetworkVariable<int> _networkDamageDealt = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);
        [SerializeField] private NetworkVariable<bool> _networkIsHost = new NetworkVariable<bool>();
        [SerializeField] private Player _killer;

        public List<Vector3> spawnPostions = new List<Vector3>();

        public string playerName { get => _networkName.Value.value; private set => _networkName.Value = new StringNetwork() { value = value }; }
        public Color playerColor { get => _networkColor.Value; private set => _networkColor.Value = value; }
        public string modelName { get => _networkModel.Value.value; private set => _networkModel.Value = new StringNetwork() { value = value }; }
        public StringNetwork modelNameNetwork { get => _networkModel.Value; private set => _networkModel.Value = value; } 
        public int kills { get => _networkKills.Value; private set => _networkKills.Value = value; }
        public int assists { get => _networkAssists.Value; private set => _networkAssists.Value = value; }
        public int deaths { get => _networkDeaths.Value; private set => _networkDeaths.Value = value; }
        public ulong _killerId { get => _networkKillerId.Value; set => _networkKillerId.Value = value; }
        public int score { get => _networkScore.Value; private set => _networkScore.Value = value; }
        public int damageDealt { get => _networkDamageDealt.Value; set => _networkDamageDealt.Value = value; }
        public bool isHost { get => _networkIsHost.Value; private set => _networkIsHost.Value = value; }

        public bool isMale; 
        bool killerHasKnife { get => GameManager.instance.GetPlayer(_killerId).GetComponent<PlayerShoot>().hasKnife; }
        bool isDead = false;
        public  HashSet<ulong> damagersId = new HashSet<ulong>();

        private PlayerMovementController _playerMovementController; 
        private PlayerShoot _playerShoot;
        [HideInInspector] public SkinnedMeshRenderer playerMesh;
        [HideInInspector] public SkinnedMeshRenderer handsMesh; 
        private FeedbackManagerUI _feedbackManager;
        private Paintable[] _paintable;
        private float _gameDuration = 300;
        [SerializeField]private AnimationCurve _alphaCurve;
        [SerializeField] private float _invincibleDurationInSeconds = 1.5f;
        [SerializeField] [ReadOnlyField] private bool _isInvincible = false;

        public delegate void PlayerDamagedEvent(Transform damager, Color color);
        public event PlayerDamagedEvent onDamageTaken; 
        #endregion


        #region Updates

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            _playerShoot = GetComponent<PlayerShoot>();
            _playerMovementController = GetComponent<PlayerMovementController>();
            playerMesh = GetComponent<WeaponManager>().humanMesh;
            handsMesh = GetComponent<WeaponManager>().handsMesh;
            _feedbackManager = FindObjectOfType<FeedbackManagerUI>();
        }

        public override void OnNetworkSpawn()
        {
            GameManager.instance.AddPlayerLocal(OwnerClientId, this);
            GameEvent.onPlayerJoinGameEvent.Invoke(this, true, OwnerClientId);
            
            
            _networkName.OnValueChanged += OnNameValueChange;
            _networkColor.OnValueChanged += OnColorValueChange;
            _networkModel.OnValueChanged += OnModelValueChange; 
            _networkKills.OnValueChanged += OnKillValueChange;
            _networkAssists.OnValueChanged += OnAssistValueChange;
            _networkDeaths.OnValueChanged += OnDeathValueChange;
            _networkScore.OnValueChanged += OnScoreValueChange;
            _networkDamageDealt.OnValueChanged += OnDamageDealtValueChange;

            GameEvent.onGameTimerUpdated.Subscribe(UpdateAlpha,this);

#if UNITY_EDITOR
            _networkName.OnValueChanged += UpdatePlayersGameObjectNameLocal;
#endif

            _playerShoot.UpdatePlayerShootColor(playerColor);
            if (IsOwner == false)
            {
                enabled = false;
                return;
            }

            IsPlayerHostServerRpc();

            UpdatePlayerCharacterServerRpc(LobbyManager.Instance.GetPlayerModel());
            UpdatePlayerColorServerRpc(LobbyManager.Instance.GetPlayerColor());
            UpdatePlayerNameServerRpc(LobbyManager.Instance.GetPlayerName());
            PlayerModelsManager.instance.UpdateAllPlayers(); //Vraiment pas ouf ici, à déplacer lorsqu'on aura synchroniser le load des joueurs et appeler juste avant le StartGame

            _gameDuration = GameManager.instance.gameMode.gameDurationInSeconds;

            _playerMovementController.Teleport(new Vector3(OwnerClientId * 100, -500.0f, 0.0f));
            CursorManager.instance.ApplyNewCursor(new CusorState(CursorLockMode.Locked, "Player"));
            
        }

        private void Start()
        {
            _currentHealth = _defaultHealth;
        }

        private void OnEnable()
        {
            InputManager.instance.ResetPlayerInputBuffer();
            SetHealth(_defaultHealth);
            isDead = false;
            GameEvent.onPlayerSpawnEvent.Subscribe(SpawnPlayerRandomly, this);
        }

        private void OnDisable()
        {
            GameEvent.onPlayerSpawnEvent.Unsubscribe(SpawnPlayerRandomly);
        }

        public override void OnNetworkDespawn()
        {
            _networkName.OnValueChanged -= OnNameValueChange;
            _networkColor.OnValueChanged -= OnColorValueChange;
            _networkModel.OnValueChanged -= OnModelValueChange;
            _networkKills.OnValueChanged -= OnKillValueChange;
            _networkAssists.OnValueChanged -= OnAssistValueChange;
            _networkDeaths.OnValueChanged -= OnDeathValueChange;
            _networkScore.OnValueChanged -= OnScoreValueChange;
            _networkDamageDealt.OnValueChanged -= OnDamageDealtValueChange;
            GameEvent.onGameTimerUpdated.Unsubscribe(UpdateAlpha);

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

        public ulong GetOwnerId() => OwnerClientId;
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
        [ServerRpc]
        private void UpdatePlayerCharacterServerRpc(string modelName)
        {
            this.modelName = modelName;
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
        private void OnColorValueChange(Color previousValue, Color nextValue) { GameEvent.onPlayerUpdateColorEvent.Invoke(this, true, OwnerClientId, nextValue); _playerShoot.UpdatePlayerShootColor(nextValue); _playerMovementController.UpdateDashColor(nextValue); }
        private void OnModelValueChange(StringNetwork previousValue, StringNetwork nextValue) { GameEvent.onPlayerUpdateModelEvent.Invoke(this, true, OwnerClientId, nextValue); PlayerModelsManager.instance.ChangeCharacterModelIg(playerMesh, handsMesh, nextValue.value); isMale = PlayerModelsManager.instance.isModelMale(nextValue.value); }
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
            AddDamagersIdClientRpc(damagerId);
        }

        [ClientRpc]
        public void DamageClientRpc(int damage, ulong damagerId, ClientRpcParams clientRpcParams)
        {
            DamageLocalClient(damage, damagerId);
        }
        [ClientRpc]
        public void AddDamagersIdClientRpc(ulong damagerId)
        {
            AddDamagersIdLocal(damagerId);
        }
        public void AddDamagersIdLocal(ulong damagerId)
        {
            damagersId.Add(damagerId);
        }
        public void DamageLocalClient(int damage, ulong damagerId)
        {
            if (isDead) return;
            Debug.Log("You've taken damage");
             _killerId = damagerId;
            SetHealth(_currentHealth - damage);
            Player damager = GameManager.instance.GetPlayer(damagerId);
            if(onDamageTaken != null ) onDamageTaken.Invoke(damager.transform, damager.playerColor);
        }

        public void Heal(int heal)
        {
            SetHealth(_currentHealth + heal);
        }

        private void SetHealth(int newHealth)
        {
            _currentHealth = newHealth;
            GameEvent.onPlayerHealthChangedEvent.Invoke(this, false, OwnerClientId, _currentHealth);

            if (_currentHealth <= 0)
            {
                isDead = true;
                PlayerDeath();
            }
        }

        #endregion

        #region Spawn and Death

        private void PlayerDeath()
        {
            PlayerDeathBehaviourLocal();
            ulong[] _damagersIdArray = new ulong[damagersId.Count];
            damagersId.CopyTo(_damagersIdArray);
            PlayerDeathFeedBackLocal(_killerId, OwnerClientId, _damagersIdArray, killerHasKnife);
            PlayerDeathBehaviourServerRpc(OwnerClientId);
            _playerShoot.HideBarloadingAmmoBox();
            Debug.Log("You're dead");
        }

        [ServerRpc]
        private void PlayerDeathBehaviourServerRpc(ulong clientId)
        {
            deaths++;
            Player killer = GameManager.instance.GetPlayer(_killerId);
            killer.kills++;
            foreach (ulong assistId in damagersId)
            {
                if (assistId == _killerId) continue;
                Player assistPlayer = GameManager.instance.GetPlayer(assistId);
                assistPlayer.assists++;
                assistPlayer.UpdateScore();
            }
            PlayerDeathBehaviourClientRpc();
            Timer.StartTimerWithCallbackRealTime(GameManager.instance.gameMode.respawnDurationInSeconds, (() => PlayerRespawnClientRpc(clientId)));

            ulong[] _damagersIdArray = new ulong[damagersId.Count];
            damagersId.CopyTo(_damagersIdArray);
            PlayerDeathFeedbackClientRpc(_killerId, clientId, _damagersIdArray, killerHasKnife); 
            killer.UpdateScore();
            UpdateScore();
        }

        [ClientRpc]
        private void PlayerDeathBehaviourClientRpc()
        {
            if (IsOwner == false) PlayerDeathBehaviourLocal();
        }
        [ClientRpc]
        private void PlayerDeathFeedbackClientRpc(ulong killerId, ulong killedId, ulong[] assistPlayers, bool killedWithKnife)
        {
            if (IsOwner == false) PlayerDeathFeedBackLocal(killerId, killedId, assistPlayers, killedWithKnife);
        }
        private void PlayerDeathFeedBackLocal(ulong killerId, ulong killedId, ulong[] assistPlayers, bool killedWithKnife)
        {
            _feedbackManager.SendFeedback(killerId, killedId, assistPlayers, killedWithKnife);
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
            SpawnPlayerRandomly(clientId);
            player.gameObject.SetActive(true);
            player.damagersId.Clear();
            GameEvent.onPlayerRespawnedEvent.Invoke(this, true, clientId);

            StartCoroutine(SetInvincibleCoroutine(_invincibleDurationInSeconds));
        }

        private void SetInvincible(bool state)
        {
            if (state)
            {
                GetComponentsInChildren<Collider>().ForEach(col =>
                {
                    col.enabled = false;
                });

                _isInvincible = true;
            }
            else
            {
                GetComponentsInChildren<Collider>().ForEach(col =>
                {
                    col.enabled = true;
                });

                _isInvincible = false;
            }
        }
        
        private IEnumerator SetInvincibleCoroutine(float duration)
        {
            SetInvincible(true);
            yield return new WaitForSeconds(duration);
            SetInvincible(false);
        }

        private void SpawnPlayerRandomly(ulong clientId)
        {
            if (GameManager.instance.possibleSpawnPositions.Count <= 0) { GameManager.instance.possibleSpawnPositions = spawnPostions; }
            Vector3 choosenPosition = GameManager.instance.possibleSpawnPositions[UnityEngine.Random.Range(0, spawnPostions.Count)];
            GameManager.instance.possibleSpawnPositions.Remove(choosenPosition);
            _playerMovementController.Teleport(choosenPosition);
        }

        private void OnKillValueChange(int previousValue, int nextValue) => GameEvent.onPlayerGetAKillEvent.Invoke(this, true, OwnerClientId, nextValue); 
        private void OnAssistValueChange(int previousValue, int nextValue) => GameEvent.onPlayerGetAssistEvent.Invoke(this, true, OwnerClientId, nextValue);  
        private void OnDeathValueChange(int previousValue, int nextValue) => GameEvent.onPlayerDiedEvent.Invoke(this, true, OwnerClientId, _killerId, nextValue);  
        private void OnScoreValueChange(int previousValue, int nextValue) => GameEvent.onPlayerScoreEvent.Invoke(this, true, OwnerClientId, nextValue);
        private void OnDamageDealtValueChange(int previousValue, int nextValue) => GameEvent.onPlayerDamageDealtEvent.Invoke(this, true, OwnerClientId, nextValue);
        #endregion
        public int UpdateScore() // scoring de base pourri, peut être à changer
        {
            score = 3 * kills - 1 * deaths + 1 * assists; 
            return score; 
        }

        #region Visual

        void UpdateAlpha (float timer)
        {
            if(IsOwner)
            {
                _playerMovementController.DisableSmoke();
                return;
            }
            if(!GameManager.instance.gameHasStarted)
            {
                _paintable ??= gameObject.GetComponentsInChildren<Paintable>(true);
                _paintable.ForEach(x => x.SetAlpha(GetAlphaValueToApply(0)));
                return;
            }
            else  
            {
                if (timer > (_gameDuration * 2f / 3f)){
                    _paintable ??= gameObject.GetComponentsInChildren<Paintable>(true);
                    _paintable.ForEach(x => x.SetAlpha(GetAlphaValueToApply(_gameDuration - timer)));
                }
                else
                {
                    _playerMovementController.DisableSmoke();
                    GameEvent.onGameTimerUpdated.Unsubscribe(UpdateAlpha);
                }
            } 
        }
        float GetAlphaValueToApply(float value) => _alphaCurve.Evaluate(value / (_gameDuration / 3));

        #endregion
        
        #region Admin

        public void Kick()
        {
            KickServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void KickServerRpc()
        {
            ulong ownerId = OwnerClientId;
            KickClientRpc();
        }
        
        [ClientRpc]
        public void KickClientRpc()
        {
            KickLocalClient();
        }

        public void KickLocalClient()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, @out) =>
            {
                VivoxManager.Instance.VivoxLogOut();
                NetworkManager.Singleton.Shutdown();
                CursorManager.instance.Revert();
                SoundManager2D.instance.PlayBackgroundMusic("Start Scene Background Music");
            };

            SceneManager.LoadSceneNetwork("MenuScene");
        }

        public void SetKills(int killNumber) => kills = killNumber;

        #endregion

        #endregion
    }
}

