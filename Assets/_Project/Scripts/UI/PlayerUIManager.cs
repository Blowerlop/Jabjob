using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;


namespace Project
{
    public class PlayerUIManager : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI playerName;
        [SerializeField] Slider playerHealthSlider;
        [SerializeField] TextMeshProUGUI playerHealthText;
        [SerializeField] Image playerPortrait;
        [SerializeField] Image playerPortraitBackground;
        [SerializeField] Transform playersBarTransform; 
        [SerializeField] PlayerUI template; 
        [SerializeField] Player _player;
        [SerializeField] Image fakeImage; 
        int playerMaxHealth;

        private readonly Dictionary<ulong, PlayerUI> playersUI = new Dictionary<ulong, PlayerUI>();

        private void Awake()
        {
            playerMaxHealth = _player.maxHealth;
            GameEvent.onPlayerUpdateNameEvent.Subscribe(UpdatePlayerName, this);
            GameEvent.onPlayerHealthChangedEvent.Subscribe(UpdatePlayerHealth, this);
            GameEvent.onPlayerUpdateColorEvent.Subscribe(UpdatePlayerColor, this);
            GameEvent.onPlayerUpdateModelEvent.Subscribe(UpdatePlayerModel, this);
            GameEvent.onPlayerJoinGameEvent.Subscribe(AddPlayerToTheBar, this);


            Player[] players = GameManager.instance.GetPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];
                ulong playerId = player.OwnerClientId;
                AddPlayerToTheBar(playerId);
                playersUI[playerId].UpdatePlayerColor(playerId, player.playerColor);
                playersUI[playerId].UpdatePlayerModel(playerId, player.modelNameNetwork);
            }
        }
        public override void OnDestroy()
        {
            GameEvent.onPlayerUpdateNameEvent.Unsubscribe(UpdatePlayerName);
            GameEvent.onPlayerHealthChangedEvent.Unsubscribe(UpdatePlayerHealth);
            GameEvent.onPlayerUpdateColorEvent.Unsubscribe(UpdatePlayerColor);
            GameEvent.onPlayerUpdateModelEvent.Unsubscribe(UpdatePlayerModel);
            GameEvent.onPlayerJoinGameEvent.Unsubscribe(AddPlayerToTheBar);
        }

        private void UpdatePlayerName(ulong playerId, StringNetwork playerName)
        {
            if (playerId != OwnerClientId) return;
            this.playerName.text = playerName.value; 
        }

        private void UpdatePlayerHealth(ulong playerId, int health)
        {
            if (playerId != OwnerClientId) return;
            this.playerHealthText.text = health.ToString() + " / " + playerMaxHealth.ToString() ;
            playerHealthSlider.value = health / (float)playerMaxHealth; 
        }

        private void UpdatePlayerColor(ulong playerId, Color color)
        {
            if (playerId != OwnerClientId) return;
            playerPortraitBackground.color = color; 
        }

        private void UpdatePlayerModel(ulong playerId, StringNetwork modelName)
        {
            if (playerId != OwnerClientId) return;
            int indexModel = PlayerModelsManager.instance.GetCurrentIndexInList(modelName.value);
            playerPortrait.sprite = PlayerModelsManager.instance.PlayerModelList[indexModel].portrait;
        }
        
        private void AddPlayerToTheBar(ulong playerId)
        { 
            PlayerUI playerBarUI = Instantiate(template, playersBarTransform);
            playerBarUI.playerId = playerId; 
            playersUI.Add(playerId, playerBarUI);
            playerBarUI.gameObject.SetActive(true);
            if (playersUI.Count == 3) { fakeImage.gameObject.SetActive(true); fakeImage.transform.SetAsFirstSibling(); }
            else fakeImage.gameObject.SetActive(false);
        }
    }

}