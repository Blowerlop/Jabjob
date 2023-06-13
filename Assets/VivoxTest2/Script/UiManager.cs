using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class UiManager : MonoBehaviour
{
    [SerializeField] private VivoxManager _vivoxManager;
    [SerializeField] private GameObject _logginPanel;
    [SerializeField] private GameObject _messageBoxPanel;
    [SerializeField] private TextMeshProUGUI _textMessage;
    [SerializeField] private Transform _textPoint;
    [Header("Loggin panel")]
    [SerializeField] private Button _logginButon;
    [SerializeField] private TMP_InputField DisplayField;

    [Header("MessageBox")]
    [SerializeField] private Button _sendButon;
    [SerializeField] private TMP_InputField _messageField;



    private void Awake()
    {
        _vivoxManager.OnUserLoggedIn += () =>
        {
            _logginPanel.SetActive(false);
            _messageBoxPanel.SetActive(true);
        }; 

        _vivoxManager.OnTextMessageLogReceived += ReceiveMessage;

        _logginButon.onClick.AddListener(() => {
            _vivoxManager.Login(DisplayField.text);

        });

        _sendButon.onClick.AddListener(() => {
            _vivoxManager.SendMessageVivox(_messageField.text);
            _messageField.text = string.Empty;
        });

    }

    private void ReceiveMessage(string senderName, VivoxUnity.IChannelTextMessage textMessage, string Color)
    {
        TextMeshProUGUI message = Instantiate(_textMessage, _textPoint);
        message.text = $"{senderName} : {textMessage.Message}";
    }
}