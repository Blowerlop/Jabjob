using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Project
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioMixer _audioMixer;
        

        public const string MASTER = "Master";
        public float GetMasterVolume => PlayerPrefs.GetFloat(MASTER, ConvertValue01ToVolume(1.0f)); 
        
        public const string MUSIC = "Music";
        public float GetMusicVolume => PlayerPrefs.GetFloat(MUSIC, ConvertValue01ToVolume(1.0f)); 

        public const string GAMESOUNDS = "GameSounds";
        public float GetGameSoundsVolume => PlayerPrefs.GetFloat(GAMESOUNDS, ConvertValue01ToVolume(1.0f));

        protected override void Awake()
        {
            keepAlive = false;
            
            base.Awake();
        }


        private void Start()
        {
            LoadVolumes();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exposedVolumeName"></param>
        /// <param name="value01">Volume is calculated in a range of 0 and 1</param>
        public void SetVolume(string exposedVolumeName, float value01)
        {
            SetVolumeWithoutSaving(exposedVolumeName, value01);
            SaveVolume(exposedVolumeName, ConvertValue01ToVolume(value01));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exposedVolumeName"></param>
        /// <param name="value01">Volume is calculated in a range of 0 and 1</param>
        public void SetVolumeWithoutSaving(string exposedVolumeName, float value01)
        {
            _audioMixer.SetFloat(exposedVolumeName, ConvertValue01ToVolume(value01));
        }
        
        
        public float ConvertValue01ToVolume(float value01)
        {
            return Mathf.Log10(Mathf.Clamp(value01, 0.0001f, 1f)) * (20.0f);
        }
        
        public float ConvertVolumeToValue01(float volume)
        {
            return Mathf.Round(Mathf.Pow(10, volume / 20) * 100) / 100;
            
            // return Mathf.Pow(10, volume / 20);
        }


        private void SaveVolume(string exposedVolumeName, float volume)
        {
            PlayerPrefs.SetFloat(exposedVolumeName, volume);
            PlayerPrefs.Save();
            
            Debug.Log($"Volume saved : {exposedVolumeName} ({volume}dB)");
        }

        private void LoadVolumes()
        {
            Debug.Log("Loading Volumes...");
            SetVolume(MASTER, ConvertVolumeToValue01(GetMasterVolume));
            SetVolume(MUSIC, ConvertVolumeToValue01(GetMusicVolume));
            SetVolume(GAMESOUNDS, ConvertVolumeToValue01(GetGameSoundsVolume));
            Debug.Log("Volumes loaded : \n " +
                      $"- {MASTER} {GetMasterVolume} dB \n " +
                      $"- {MUSIC} {GetMusicVolume} dB \n " +
                      $"- {GAMESOUNDS} {GetGameSoundsVolume} dB");
        }
    }
}
