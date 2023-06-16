using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Project
{
    public class ChatBoxUI : MonoBehaviour
    {
        private VivoxManager _vivoxManager;
        [SerializeField] private RectTransform _textAreaRectTF;
        private readonly float baseTextAreaHeight = 147;

        [SerializeField] private bool _chatGameplay;

        [Header("Visual element ref")]
        [SerializeField] private Transform _textPoint;
        [SerializeField] private TextMeshProUGUI _textPrefab;
        [SerializeField] private Button _sendBTN;
        [SerializeField] private TMP_InputField _textField;
        [SerializeField] private Button _extendShrinkBTN;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _textAreaImage;
        private List<TextMeshProUGUI> _messages = new List<TextMeshProUGUI>();
        private string _previousTextReceived;
        private bool _shrinkState = true;
        private PlayerInputAction _inputAction;
        private InputManager _inputManager;

        private void Start()
        {
            if (!_vivoxManager)
            {
                _vivoxManager = FindObjectOfType<VivoxManager>();
                _vivoxManager.OnTextMessageLogReceived = null;
                _vivoxManager.OnTextMessageLogReceived += OnMessageReceive;

                _sendBTN.onClick.AddListener(() => SendMessage());
                ShrinkTextArea(true);


                _extendShrinkBTN.onClick.AddListener(() =>
                {
                    _shrinkState = !_shrinkState;
                    ShrinkTextArea(_shrinkState);
                });

                _inputAction = new PlayerInputAction();
                if (!_inputAction.UI.enabled)
                    _inputAction.UI.Enable();
                _inputAction.UI.Enter.performed += Enter_performed;

                _inputManager = FindObjectOfType<InputManager>();
            }

        }

        private void Enter_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_inputAction != null)
            {

                if (!_textField.isFocused)
                {
                    _textField.Select();
                    if (_chatGameplay)
                    {
                        ShowChatBox(true);
                    }
                    return;
                }

                if (_textField.text != string.Empty)
                {
                    SendMessage();
                    EventSystem.current.SetSelectedGameObject(null);
                    if (_chatGameplay)
                    {
                        ShowChatBox(false);
                    }
                    return;
                }


                if (_textField.text == string.Empty && _canvasGroup.alpha == 1 && _chatGameplay)
                {
                    ShowChatBox(false);
                    EventSystem.current.SetSelectedGameObject(null);

                    return;
                }
            }
        }

        private void ShowChatBox(bool state)
        {
            if(state)
            {
                _canvasGroup.alpha = 1;
                Color color = _textAreaImage.color;
                color.a = .5f;
                _textAreaImage.color = color;

                _inputManager.SwitchPlayerInputMap("UI");
            }
            else
            {
                _canvasGroup.alpha = 0;

                Color color = _textAreaImage.color;
                color.a = 0;
                _textAreaImage.color = color;
                _inputManager.SwitchPlayerInputMap("Player");
            }
        }
        

        private void OnDestroy()
        {
            if (_vivoxManager)
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
                _inputAction = null;
            }
        }

        private void SendMessage(bool sendWithEnter = false)
        {
            _vivoxManager.SendMessageVivox(_textField.text);
            _textField.text = "";

            if (sendWithEnter)
                _textField.Select();
        }

        private void OnMessageReceive(string playerName, VivoxUnity.IChannelTextMessage messageText, string color)
        {
            var colo = $"<color=#{color}>";
            var endColo = "</color>";
            var value = $"{colo}{playerName}{endColo} : {messageText.Message}";

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
                _textAreaRectTF.sizeDelta = new Vector2(_textAreaRectTF.rect.width, baseTextAreaHeight * 2.2f);
            }
        }
    }
}