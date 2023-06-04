using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Project
{
    public class ChatBoxUI : MonoBehaviour
    {
        private VivoxManager _vivoxManager;

        [Header("Visual element ref")]
        [SerializeField] private Transform _textPoint;
        [SerializeField] private TextMeshProUGUI _textPrefab;
        [SerializeField] private Button _sendBTN;
        [SerializeField] private TMP_InputField _textField;

        private List<TextMeshProUGUI> _messages = new List<TextMeshProUGUI>();
        private string _previousTextReceived;

        private void OnEnable()
        {
            if (!_vivoxManager)
            {
                _vivoxManager = FindObjectOfType<VivoxManager>();
                _vivoxManager.OnTextMessageLogReceived += OnMessageReceive;
                _sendBTN.onClick.AddListener(SendMessage);
            }


        }

        private void OnDisable()
        {
            if(_vivoxManager)
            {
                _vivoxManager.OnTextMessageLogReceived -= OnMessageReceive;
                _vivoxManager = null;
                _sendBTN.onClick.RemoveAllListeners();

                for (int i = 0; i < _messages.Count; i++)
                {
                    Destroy(_messages[i]);
                }
                _messages.Clear();

            }
        }

        private void SendMessage()
        {
            _vivoxManager.SendMessageVivox(_textField.text);
            _textField.text = string.Empty;
        }

        private void OnMessageReceive(string playerName, VivoxUnity.IChannelTextMessage messageText)
        {
            var value = $"{playerName} : {messageText.Message}";
            if (_previousTextReceived == value)
                return;

            TextMeshProUGUI TMP = Instantiate(_textPrefab, _textPoint);
            TMP.text = value;
            _previousTextReceived = value;
            _messages.Add(TMP);

        }

        
    }
}
