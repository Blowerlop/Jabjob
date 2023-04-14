using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using VivoxUnity;
public class TextPanel : MonoBehaviour
{
    [SerializeField] private VivoxManager vivoxManager;

    [SerializeField] private Transform _textPoint;
    [SerializeField] private TextMeshProUGUI _textPrefab;

    private void Awake()
    {
        vivoxManager.OnTextMessageLogReceived += OnTextMessageLogReceived;
    }

    private void OnTextMessageLogReceived(string displayName, IChannelTextMessage arg1)
    {
        var obj = Instantiate(_textPrefab, _textPoint);
        obj.text = displayName + ": " + arg1.Message;
    }
}
