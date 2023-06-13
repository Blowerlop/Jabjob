using System;
using System.Collections;
using System.Globalization;
using Project.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("Master Volume")]
        [SerializeField] private TMP_InputField _masterVolumeInputField;
        [SerializeField] private Slider _masterVolumeSlider;
        
        [Header("Music Volume")]
        [SerializeField] private TMP_InputField _musicVolumeInputField;
        [SerializeField] private Slider _musicVolumeSlider;
        
        [Header("Music Volume")]
        [SerializeField] private TMP_InputField _gameSoundsInputField;
        [SerializeField] private Slider _gameSoundsVolumeSlider;


        private void Start()
        {
            SetMasterVolumeSliderValue((AudioManager.instance.ConvertVolumeToValue01(AudioManager.instance.GetMasterVolume) * _masterVolumeSlider.maxValue).ToString(CultureInfo.InvariantCulture));
            SetMusicVolumeSliderValue((AudioManager.instance.ConvertVolumeToValue01(AudioManager.instance.GetMusicVolume) * _musicVolumeSlider.maxValue).ToString(CultureInfo.InvariantCulture));
            SetGameSoundsVolumeSliderValue((AudioManager.instance.ConvertVolumeToValue01(AudioManager.instance.GetGameSoundsVolume) * _gameSoundsVolumeSlider.maxValue).ToString(CultureInfo.InvariantCulture));
            
            // SetMasterVolumeSliderValue((AudioManager.instance.ConvertVolumeToValue01(AudioManager.instance.GetMasterVolume) * _masterVolumeSlider.maxValue).ToString(CultureInfo.InvariantCulture));

            // _masterVolumeSlider.SetValueWithoutNotify();
            // _musicVolumeSlider.SetValueWithoutNotify(AudioManager.instance.GetMusicVolume);
            // _gameSoundsVolumeSlider.SetValueWithoutNotify(AudioManager.instance.GetGameSoundsVolume);
        }


        private void OnEnable()
        {
            Start();
            
            _masterVolumeSlider.onValueChanged.AddListenerExtended(SetMasterVolume);
            _masterVolumeInputField.onEndEdit.AddListenerExtended(SetMasterVolumeSliderValue);
            
            _musicVolumeSlider.onValueChanged.AddListenerExtended(SeMusicVolume);
            _musicVolumeInputField.onEndEdit.AddListenerExtended(SetMusicVolumeSliderValue);
            
            _gameSoundsVolumeSlider.onValueChanged.AddListenerExtended(SetGameSoundsVolume);
            _gameSoundsInputField.onEndEdit.AddListenerExtended(SetGameSoundsVolumeSliderValue);
        }

        private void OnDisable()
        {
            _masterVolumeSlider.onValueChanged.RemoveListenerExtended(SetMasterVolume);
            _masterVolumeInputField.onEndEdit.RemoveListenerExtended(SetMasterVolumeSliderValue);

            _musicVolumeSlider.onValueChanged.RemoveListenerExtended(SeMusicVolume);
            _musicVolumeInputField.onEndEdit.RemoveListenerExtended(SetMusicVolumeSliderValue);

            _gameSoundsVolumeSlider.onValueChanged.RemoveListenerExtended(SetGameSoundsVolume);
            _gameSoundsInputField.onEndEdit.RemoveListenerExtended(SetGameSoundsVolumeSliderValue);
        }


        private void SetMasterVolumeSliderValue(string text)
        {
            _masterVolumeSlider.value = int.Parse(text.ExtractNumber());
        }
        
        private void SetMusicVolumeSliderValue(string text)
        {
            _musicVolumeSlider.value = int.Parse(text.ExtractNumber());
        }
        
        private void SetGameSoundsVolumeSliderValue(string text)
        {
            _gameSoundsVolumeSlider.value = int.Parse(text.ExtractNumber());
        }
        
        
        

        private void SetMasterVolume(float value)
        {
            AudioManager.instance.SetVolume(AudioManager.MASTER, value / _masterVolumeSlider.maxValue);
            _masterVolumeInputField.text = value.ToString(CultureInfo.InvariantCulture);
        }
        
        private void SeMusicVolume(float value)
        {
            AudioManager.instance.SetVolume(AudioManager.MUSIC, value / _musicVolumeSlider.maxValue);
            _musicVolumeInputField.text = value.ToString(CultureInfo.InvariantCulture);
        }
        
        private void SetGameSoundsVolume(float value)
        {
            AudioManager.instance.SetVolume(AudioManager.GAMESOUNDS, value / _gameSoundsVolumeSlider.maxValue);
            _gameSoundsInputField.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
