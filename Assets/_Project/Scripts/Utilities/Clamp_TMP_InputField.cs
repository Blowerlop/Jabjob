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


        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        private void Start()
        {
            if (_inputField.contentType != TMP_InputField.ContentType.Alphanumeric)
            {
                Debug.LogError("Trying to clamp a non Alphanumeric inputFiled... Destroying the script !");
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
            if (int.TryParse(text.ExtractNumber(), out int value))
            {
                text = Mathf.Clamp(value, _clampRange.x, _clampRange.y).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                Debug.LogError($"Error : TryParse false --> Is there a number ? Text : {text}");
            }

            _inputField.text = text;
        }
    }
}
