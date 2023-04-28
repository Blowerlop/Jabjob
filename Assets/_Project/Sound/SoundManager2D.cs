using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager2D : MonoBehaviour
{
    public static SoundManager2D instance { get; private set; }

    public AudioSource uiSound, backgroundMusic;

    public SoundList[] soundList;
    private Dictionary<string, AudioClip> _soundListDico = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < soundList.Length; i++)
        {
            if (soundList[i].name != null && soundList[i].sound != null) _soundListDico.Add(soundList[i].name, soundList[i].sound);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic("Start Scene Background Music");
    }
    public void PlayBackgroundMusic(string name)
    {
        if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour la musique de fond : " + name);
        else
        {
            backgroundMusic.Stop();
            backgroundMusic.clip = _soundListDico[name];
            backgroundMusic.Play();
        }
    }

    public void PlayUISound(string name)
    {
        if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son UI : " + name);
        else uiSound.PlayOneShot(_soundListDico[name]);
    }

}