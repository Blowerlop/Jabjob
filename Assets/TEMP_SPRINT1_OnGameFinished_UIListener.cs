using System;
using TMPro;
using UnityEngine;

namespace Project
{
    public class TEMP_SPRINT1_OnGameFinished_UIListener : MonoBehaviour, IGameEventListener
    {
        [SerializeField] private TMP_Text textTMP;

        

        public void OnEnable()
        {
            GameEvent.onGameFinishedEvent.Subscribe(ShowUI, this);
        }

        public void OnDisable()
        {
            GameEvent.onGameFinishedEvent.Unsubscribe(ShowUI);
        }

        public void ShowUI()
        {
            textTMP.enabled = true; 
        }
    }
}