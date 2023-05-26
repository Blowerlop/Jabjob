using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Project.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Project
{
    public class AutoPrefixSuffix_TMP_InputField : MonoBehaviour
    {
        private TMP_InputField _inputField;

        [SerializeField] private bool _usePrefix;
        [SerializeField] private string _prefix;

        [SerializeField] private bool _useSuffix;
        [SerializeField] private string _suffix;


        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        private void OnEnable()
        {
            _inputField.onValueChanged.AddListenerExtended(AddPrefixOrSuffix);
        }

        private void OnDisable()
        {
            _inputField.onValueChanged.RemoveListener(AddPrefixOrSuffix);
        }

        public void AddPrefixOrSuffix(string text)
        {
            _inputField.onValueChanged.RemoveListener(AddPrefixOrSuffix);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(text);


            if (_usePrefix && string.IsNullOrEmpty(_prefix) == false)
            {
                stringBuilder.Replace(_prefix, "");
                stringBuilder.Insert(0, _prefix);
            }


            if (_useSuffix && string.IsNullOrEmpty(_suffix) == false)
            {
                stringBuilder.Replace(_suffix, "");
                stringBuilder.Append(_suffix);
            }

            _inputField.text = stringBuilder.ToString();

            // textInfo.textComponent.text = $"{_suffix}{textInfo.textComponent.text}{_prefix}";

            StartCoroutine(UtilitiesClass.WaitForEndOfFrameAndDoActionCoroutine(() =>
                _inputField.onValueChanged.AddListener(AddPrefixOrSuffix)));
        }
    }
}
