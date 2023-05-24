using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class ResolutionContainer : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private string _resolutionString;
        private Resolution _resolution;
        
        public Resolution resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                _resolutionString = value.ToString();
            }
        }
        #else
        public Resolution resolution;
        #endif
    }
}
