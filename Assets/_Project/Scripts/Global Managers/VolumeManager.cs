using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Project
{
    public class VolumeManager : Singleton<VolumeManager>
    {
        [SerializeField] private AudioMixer _audioMixer;
        

        public const string MASTER = "Master";
        public const string MUSIC = "Music";
        public const string GAMESOUNDS = "GameSounds";
        
        public override void Awake()
        {
            keepAlive = false;
            
            base.Awake();
        }


        private void Start()
        {
            SetVolume(MASTER, 0f);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume">Volume is calculated with a range between 0 and 1</param>
        public void SetVolume(string _exposedVolumeName, float volume)
        {
            _audioMixer.SetFloat(_exposedVolumeName, ValueToVolume(volume));
        }
        
        
        private float ValueToVolume(float value)
        {
            // return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * (20.0f - _THRESHOLDVOLUME) / 4f + 20.0f;
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * (20.0f);
            // return Mathf.Log10(value) * 20;
        }
    }
}
