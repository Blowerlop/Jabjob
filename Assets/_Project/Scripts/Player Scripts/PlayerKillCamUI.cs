using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class PlayerKillCamUI : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _spectatedPlayerNameText;

        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

        [SerializeField] private UnityEvent _onKillCamEnableEvent = new UnityEvent();
        [SerializeField] private UnityEvent _onKillCamDisableEvent = new UnityEvent();
        private void Awake()
        {
            // SetSpectatedPlayerNameText(OwnerClientId, GameManager.instance.GetPlayer(OwnerClientId).playerName);
            // GameEvent.onPlayerUpdateNameEvent.Subscribe(SetSpectatedPlayerNameText, this);
            GameEvent.onPlayerDiedEvent.Subscribe(EnableKillCam, this);
            GameEvent.onPlayerRespawnedEvent.Subscribe(DisableKillCam, this);
            
            // gameObject.SetActive(false); 
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner) Destroy(this);
        }

        public override void OnDestroy()
        {
            // GameEvent.onPlayerUpdateNameEvent.Unsubscribe(SetSpectatedPlayerNameText);
            GameEvent.onPlayerDiedEvent.Unsubscribe(EnableKillCam);
            GameEvent.onPlayerRespawnedEvent.Unsubscribe(DisableKillCam);
        }
         
        

        private void EnableKillCam(ulong playerKilledId, ulong playerKillerId, int playerDeaths)
        {
            if (playerKillerId != OwnerClientId || GameManager.instance.GetPlayer(playerKilledId).enabled == false) return;
            
            _spectatedPlayerNameText.text = $"Spectating : {GameManager.instance.GetPlayer(playerKillerId).name}";
            
            transform.GetChild(0).gameObject.SetActive(true);
            _cinemachineVirtualCamera.Priority = 3;
            _onKillCamEnableEvent.Invoke();
        }

        private void DisableKillCam(ulong playerKilledId)
        {
            if (playerKilledId == OwnerClientId) return;
            transform.GetChild(0).gameObject.SetActive(false);
            _cinemachineVirtualCamera.Priority = -1;
            _onKillCamDisableEvent.Invoke();  
        }
    }
}
