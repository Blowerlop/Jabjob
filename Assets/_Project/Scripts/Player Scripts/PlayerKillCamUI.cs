using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class PlayerKillCamUI : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _spectatedPlayerNameText;

        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
         
        private void Awake()
        {
            // SetSpectatedPlayerNameText(OwnerClientId, GameManager.instance.GetPlayer(OwnerClientId).playerName);
            // GameEvent.onPlayerUpdateNameEvent.Subscribe(SetSpectatedPlayerNameText, this);
            GameEvent.onPlayerDiedEvent.Subscribe(EnableKillCam, this);
            GameEvent.onplayerRespawnedEvent.Subscribe(DisableKillCam, this);
            
            gameObject.SetActive(false); 
        } 

        public override void OnDestroy()
        {
            // GameEvent.onPlayerUpdateNameEvent.Unsubscribe(SetSpectatedPlayerNameText);
            GameEvent.onPlayerDiedEvent.Unsubscribe(EnableKillCam);
            GameEvent.onplayerRespawnedEvent.Unsubscribe(DisableKillCam);
        }
         

        private void SetSpectatedPlayerNameText(ulong clientId, StringNetwork playerName) => SetSpectatedPlayerNameText(clientId, playerName.ToString());
        private void SetSpectatedPlayerNameText(ulong clientId, string playerName)
        {
            if (clientId == OwnerClientId)
            {
                _spectatedPlayerNameText.text = playerName.ToString();
            }
        }

        private void EnableKillCam(ulong playerKilledId, ulong playerKillerId, int playerDeaths)
        {
            _spectatedPlayerNameText.text = GameManager.instance.GetPlayer(playerKillerId).name;
            
            gameObject.SetActive(true);
            // _cinemachineVirtualCamera.Priority = 2;
            GameManager.instance.GetPlayer(playerKillerId).GetComponentInChildren<CinemachineVirtualCamera>().Priority =
                3;
        }

        private void DisableKillCam(ulong playerKilledId)
        {
            gameObject.SetActive(false);
            // _cinemachineVirtualCamera.Priority = -1;

        }
    }
}
