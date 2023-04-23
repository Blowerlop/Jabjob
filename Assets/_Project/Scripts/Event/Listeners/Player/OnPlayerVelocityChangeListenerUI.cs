using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Project
{
    public class OnPlayerVelocityChangeListenerUI : MonoBehaviour, IGameEventListener
    {
        [SerializeField] private TMP_Text _textUI;
        [SerializeField] [ReadOnlyField] private string _defaultText = "Velocity:";
        private StringBuilder _stringBuilder = new StringBuilder();
        
        private void Awake()
        {
            _textUI = GetComponent<TMP_Text>();
        }

        public void OnEnable()
        {
            GameEvent.onPlayerVelocityChangedEvent.Subscribe(UpdatePlayerVelocityUI, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerVelocityChangedEvent.Unsubscribe(UpdatePlayerVelocityUI);
        }


        private void UpdatePlayerVelocityUI(Vector3 velocity)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_defaultText);
            _stringBuilder.Append(velocity);
            _textUI.text = _stringBuilder.ToString();
        }
    }
}