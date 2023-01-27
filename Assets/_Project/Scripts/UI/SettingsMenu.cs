using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] resolutions;
    private int resolutionIndex;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        ReloadPlayerPrefs();
        ReloadPlayerPrefsView();
    }

    public void SetResolution(int localResolutionIndex)
    {
        resolutionIndex= localResolutionIndex;
        Resolution resolution = resolutions[localResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, IntToBool(PlayerPrefs.GetInt("fullscreen", 1)));
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SavePlayerPref()
    {
        float volume;

        if (audioMixer.GetFloat("volume", out volume))
        {
            PlayerPrefs.SetFloat("volume", volume);
        }

        PlayerPrefs.SetInt("resolution", resolutionIndex);
        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("fullscreen", BoolToInt(Screen.fullScreen));

        PlayerPrefs.Save();

        Debug.Log("Player prefs have been save");
    }

    private void ReloadPlayerPrefs()
    {
        SetResolution(PlayerPrefs.GetInt("resolution", 0));
        SetQuality(PlayerPrefs.GetInt("quality", 2));
        SetFullscreen(IntToBool(PlayerPrefs.GetInt("fullscreen", 1)));
        SetVolume(PlayerPrefs.GetFloat("volume", 0));

        Debug.Log("Player prefs have been load");
    }

    private void ReloadPlayerPrefsView()
    {
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution", 0);
        graphicsDropdown.value = PlayerPrefs.GetInt("quality", 2);
        fullscreenToggle.isOn = IntToBool(PlayerPrefs.GetInt("fullscreen", 1));
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0);

        Debug.Log("Player prefs have been load visualy");
    }

    private int BoolToInt(bool value)
    {
        if (value)
            return 1;
        else
            return 0;
    }

    private bool IntToBool(int value)
    {
        if (value != 0)
            return true;
        else
            return false;
    }
}
