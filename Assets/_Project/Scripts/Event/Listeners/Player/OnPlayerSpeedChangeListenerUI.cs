using System.Text;
using TMPro;
using UnityEngine;

namespace Project
{
    public class OnPlayerSpeedChangeListenerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textUI;
        [SerializeField] [ReadOnlyField] private string _defaultText = "Speed:";
        private StringBuilder _stringBuilder = new StringBuilder();
        
        private void Awake()
        {
            _textUI = GetComponent<TMP_Text>();
        }

        public void OnEnable()
        {
            GameEvent.onPlayerSpeedChangedEvent.Subscribe(UpdatePlayerSpeedUI, this);
        }

        public void OnDisable()
        {
            GameEvent.onPlayerSpeedChangedEvent.Unsubscribe(UpdatePlayerSpeedUI);
        }


        private void UpdatePlayerSpeedUI(float speed)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_defaultText);
            _stringBuilder.Append(speed);
            _textUI.text = _stringBuilder.ToString();
        }
    }
}