using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class MouseManager : Singleton<MouseManager>
    {
        public const string MOUSESENSITIVITY = "MouseSensitivity";
        public float mouseSensitivity => PlayerPrefs.GetFloat(MOUSESENSITIVITY, 1.0f); 
        
        protected override void Awake()
        {
            keepAlive = false;
            
            base.Awake();
        }


        private void Start()
        {
            LoadMouseSensitivity();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exposedVolumeName"></param>
        /// <param name="value01">Volume is calculated in a range of 0 and 1</param>
        public void SetMouseSensitivity(float sensitivity)
        {
            SaveMouseSensitivity(sensitivity);
        }
        
        private void SaveMouseSensitivity(float sensitivity)
        {
            PlayerPrefs.SetFloat(MOUSESENSITIVITY, sensitivity);
            PlayerPrefs.Save();
            
            Debug.Log($"Mouse sensitivity saved : {sensitivity}");
        }

        private void LoadMouseSensitivity()
        {
            Debug.Log("Setting the mouse sensitivity...");
            SetMouseSensitivity(mouseSensitivity);
            Debug.Log($"Mouse sensitivity loaded : {mouseSensitivity}");
        }
    }
}
