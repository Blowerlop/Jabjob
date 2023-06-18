using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Project.Utilities;
using TMPro;
using UnityEngine;

namespace Project
{
    public class MouseSettings : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _mouseSensitivityInputField;
        [SerializeField] private float _addLessOrMoveSensitivityBearing = 0.25f;

        [SerializeField] private Clamp_TMP_InputField _clampTMPInputField;
        

        private void OnEnable()
        {
            _clampTMPInputField.onValueClampedEvent.Subscribe(ConvertToToFloatPrecision, this);
            _mouseSensitivityInputField.onValueChanged.AddListenerExtended(SetSensitivity);
            // _mouseSensitivityInputField.onEndEdit.AddListenerExtended(ConvertToToFloatPrecision);

            StartCoroutine(UtilitiesClass.WaitForEndOfFrameAndDoActionCoroutine(() =>
            {
                SetSensitivity(MouseManager.instance.mouseSensitivity.ToString(CultureInfo.InvariantCulture));
                ConvertToToFloatPrecision(
                    MouseManager.instance.mouseSensitivity.ToString(CultureInfo.InvariantCulture));
            }));
        }

        private void OnDisable()
        {
            _clampTMPInputField.onValueClampedEvent.Unsubscribe(ConvertToToFloatPrecision);
            _mouseSensitivityInputField.onValueChanged.RemoveListenerExtended(SetSensitivity);
            // _mouseSensitivityInputField.onEndEdit.RemoveListenerExtended(ConvertToToFloatPrecision);
        }

        public void AddMoreSensitivity()
        {
            float sensitivityToAdd = MouseManager.instance.mouseSensitivity + _addLessOrMoveSensitivityBearing;
            SetSensitivity(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
            _mouseSensitivityInputField.onEndEdit.Invoke(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
        } 
        
        public void AddLessSensitivity()
        {
            float sensitivityToAdd = MouseManager.instance.mouseSensitivity - _addLessOrMoveSensitivityBearing;
            SetSensitivity(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
            _mouseSensitivityInputField.onEndEdit.Invoke(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
        } 

        public void SetSensitivity(string sensitivity)
        {
            MouseManager.instance.SetMouseSensitivity(float.Parse(sensitivity));
        } 

        private void ConvertToToFloatPrecision(string sensitivity) => _mouseSensitivityInputField.text = $"{float.Parse(sensitivity):F2}";
    }
}
