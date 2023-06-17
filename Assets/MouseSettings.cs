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

        private void OnEnable()
        {
            _mouseSensitivityInputField.onEndEdit.AddListenerExtended(SetSensitivity);

            StartCoroutine(UtilitiesClass.WaitForEndOfFrameAndDoActionCoroutine(() =>
            {
                SetSensitivity(MouseManager.instance.mouseSensitivity.ToString(CultureInfo.InvariantCulture));

            }));
        }

        private void OnDisable()
        {
            _mouseSensitivityInputField.onEndEdit.RemoveListenerExtended(SetSensitivity);
        }

        public void AddMoreSensitivity()
        {
            float sensitivityToAdd = MouseManager.instance.mouseSensitivity + _addLessOrMoveSensitivityBearing;
            SetSensitivity(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
        } 
        
        public void AddLessSensitivity()
        {
            float sensitivityToAdd = MouseManager.instance.mouseSensitivity - _addLessOrMoveSensitivityBearing;
            SetSensitivity(sensitivityToAdd.ToString(CultureInfo.InvariantCulture));
        } 

        public void SetSensitivity(string sensitivity)
        {
            MouseManager.instance.SetMouseSensitivity(float.Parse(sensitivity));
            _mouseSensitivityInputField.SetTextWithoutNotify($"{float.Parse(sensitivity):F2}");
        } 


    }
}
