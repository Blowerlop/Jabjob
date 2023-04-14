using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SendPanel : MonoBehaviour
{
    [SerializeField] private VivoxManager vivoxManager;
    [SerializeField] private Button _sendButton;
    [SerializeField] private TMP_InputField _messageField;

    private void Awake()
    {
        _sendButton.onClick.AddListener(() =>
        {
            vivoxManager.SendMessageVivox(_messageField.text);
            _messageField.text = "";
        });
    }
}
