using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; 

using UnityEngine;

namespace Project
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] Image playerPortraitBackground;
        [SerializeField] Image playerPortrait;
        [SerializeField] Image deathBackground;
        [SerializeField] TextMeshProUGUI deathTimerText;

        public ulong playerId = 0;
        float deathTimer; 
        bool deathTimerIsActive = false; 
        private void Awake()
        {
            GameEvent.onPlayerUpdateColorEvent.Subscribe(UpdatePlayerColor, this);
            GameEvent.onPlayerUpdateModelEvent.Subscribe(UpdatePlayerModel, this);
            GameEvent.onPlayerDiedEvent.Subscribe(SetDeathTimer, this);
            GameEvent.onPlayerRespawnedEvent.Subscribe(PlayerRespawn, this);
            deathTimer = GameManager.instance.gameMode.respawnDurationInSeconds;
        }
        public void OnDestroy()
        {
            GameEvent.onPlayerUpdateColorEvent.Unsubscribe(UpdatePlayerColor);
            GameEvent.onPlayerUpdateModelEvent.Unsubscribe(UpdatePlayerModel);
            GameEvent.onPlayerDiedEvent.Unsubscribe(SetDeathTimer);
            GameEvent.onPlayerRespawnedEvent.Unsubscribe(PlayerRespawn);
        }

        private void Update()
        {
            if (!deathTimerIsActive) return;
            deathTimer -= Time.deltaTime;
            deathTimerText.text = Mathf.Ceil(deathTimer).ToString();
            if(deathTimer < 0)
            {
                deathTimer = 0; 
                deathTimerIsActive = false;
            }
        }
        public void UpdatePlayerColor(ulong playerId, Color color)
        {
            if (this.playerId != playerId) return;
            playerPortraitBackground.color = color; 
        }
        public void UpdatePlayerModel(ulong playerId, StringNetwork modelName)
        {
            if (this.playerId != playerId) return;
            int indexModel = PlayerModelsManager.instance.GetCurrentIndexInList(modelName.value);
            playerPortrait.sprite = PlayerModelsManager.instance.PlayerModelList[indexModel].portrait;
        }
        private void SetDeathTimer(ulong playerId, ulong killerId, int value)
        {
            if (this.playerId != playerId) return;
            deathTimer = GameManager.instance.gameMode.respawnDurationInSeconds;
            deathTimerIsActive = true;
            deathBackground.enabled = true;
            deathTimerText.enabled = true;
        }
        private void PlayerRespawn(ulong playerId)
        {
            if (this.playerId != playerId) return;
            deathTimerIsActive = false;
            deathBackground.enabled = false;
            deathTimerText.enabled = false;

        }
    }
}

