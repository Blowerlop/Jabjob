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
        [SerializeField] private RectTransform _textAreaRectTF;
        private readonly float baseTextAreaHeight = 147;


        [Header("Visual element ref")]
        [SerializeField] private Transform _textPoint;
        [SerializeField] private TextMeshProUGUI _textPrefab;
        [SerializeField] private Button _sendBTN;
        [SerializeField] private TMP_InputField _textField;
        [SerializeField] private Button _extendShrinkBTN;
        private List<TextMeshProUGUI> _messages = new List<TextMeshProUGUI>();
        private string _previousTextReceived;
        private bool _shrinkState = true;
        private void OnEnable()
        {
            if (!_vivoxManager)
            {
                _vivoxManager = FindObjectOfType<VivoxManager>();
                _vivoxManager.OnTextMessageLogReceived += OnMessageReceive;
                _sendBTN.onClick.AddListener(SendMessage);
                ShrinkTextArea(true);

                _extendShrinkBTN.onClick.AddListener(() =>
                {
                    _shrinkState = !_shrinkState;
                    ShrinkTextArea(_shrinkState);
                    Debug.Log(_shrinkState);
                });

            }


        }

        private void OnDisable()
        {
            if(_vivoxManager)
            {
                _vivoxManager.OnTextMessageLogReceived -= OnMessageReceive;
                _vivoxManager = null;
                _sendBTN.onClick.RemoveAllListeners();
                _extendShrinkBTN.onClick.RemoveAllListeners();
                for (int i = 0; i < _messages.Count; i++)
                {
                    Destroy(_messages[i]);
                }
                _messages.Clear();
                ShrinkTextArea(false);

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

        private void ShrinkTextArea(bool shrink)
        {
            if (shrink)
            {
                _textAreaRectTF.sizeDelta = new Vector2(_textAreaRectTF.rect.width, baseTextAreaHeight);
            }
            else
            {
                _textAreaRectTF.sizeDelta = new Vector2(_textAreaRectTF.rect.width, baseTextAreaHeight* 2.2f);
            }
        }
    }
}
