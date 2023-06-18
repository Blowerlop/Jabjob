using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Project.Utilities;
using TMPro;
using UnityEngine;

namespace Project
{
    public class Clamp_TMP_InputField : MonoBehaviour
    {
        private TMP_InputField _inputField;

        [SerializeField] private Vector2 _clampRange = new Vector2(0.0f, 1.0f);
        public Event<string> onValueClampedEvent = new Event<string>(nameof(onValueClampedEvent));

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        private void Start()
        {
            if (((_inputField.contentType == TMP_InputField.ContentType.Alphanumeric) || (_inputField.contentType == TMP_InputField.ContentType.DecimalNumber)) == false )
            {
                Debug.LogError("Trying to clamp a non Alphanumeric or non DecimalNumber inputFiled... Destroying the script !");
                Destroy(this);
            }
        }

        // private void OnEnable()
        // {
        //     _inputField.onEndEdit.AddListenerExtended(ClampValueAndSetText);
        // }
        //
        // private void OnDisable()
        // {
        //     _inputField.onEndEdit.RemoveListener(ClampValueAndSetText);
        // }


        public void ClampValueAndSetText(string text)
        {
            if (float.TryParse(text.ExtractNumber(), out float value))
            {
                text = Mathf.Clamp(value, _clampRange.x, _clampRange.y).ToString(CultureInfo.InvariantCulture);
                Debug.Log($"Clamped text : {text}");
            }
            else
            {
                Debug.LogError($"Error : TryParse false --> Is there a number ? Original Text : {text} / Output : {text.ExtractNumber()}");
            }

            Debug.Log(text.ExtractNumber());
            _inputField.text = text;
            onValueClampedEvent.Invoke(this, false, text);
        }
    }
}
