using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Project.Utilities;
using TMPro;
using UnityEngine;

namespace Project
{
    public class NetworkTimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
    
        public void OnEnable()
        {
            GameEvent.onGameTimerUpdated.Subscribe(UpdateTimerText, this);
        }

        public void OnDisable()
        {
            GameEvent.onGameTimerUpdated.Unsubscribe(UpdateTimerText);
        }

        private void UpdateTimerText(float timerValue)
        {
            float minutes = UtilitiesClass.ConvertSecondsToMinutes(timerValue);
            float seconds = Mathf.FloorToInt(timerValue % 60);

            _text.text = $"{minutes:00} : {seconds:00}";

            
        }
    }
}
