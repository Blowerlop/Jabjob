using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager2D : MonoBehaviour
{
    public static SoundManager2D instance { get; private set; }

    public AudioSource uiSound, backgroundMusic, announcer, inGameSound2D;

    public SoundList[] soundListBackground;
    public SoundList[] soundListUI;
    public SoundList[] announcerListUI;
    public SoundList[] inGameSound2DList;

    private Dictionary<string, AudioClip> _soundListDicoBackground = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _soundListDicoUI = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _soundListDicoAnnouncer = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _inGameSoundDico = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < soundListBackground.Length; i++)
        {
            if (soundListBackground[i].name != null && soundListBackground[i].sound != null) _soundListDicoBackground.Add(soundListBackground[i].name, soundListBackground[i].sound);
        }
        for (int i = 0; i < soundListUI.Length; i++)
        {
            if (soundListUI[i].name != null && soundListUI[i].sound != null) _soundListDicoUI.Add(soundListUI[i].name, soundListUI[i].sound);
        }
        for (int i = 0; i < announcerListUI.Length; i++)
        {
            if (announcerListUI[i].name != null && announcerListUI[i].sound != null) _soundListDicoAnnouncer.Add(announcerListUI[i].name, announcerListUI[i].sound);
        }
        for (int i = 0; i < inGameSound2DList.Length; i++)
        {
            if (inGameSound2DList[i].name != null && inGameSound2DList[i].sound != null) _inGameSoundDico.Add(inGameSound2DList[i].name, inGameSound2DList[i].sound);
        }
    }
    private void Start()
    {
        PlayBackgroundMusic("Start Scene Background Music");
    }
    public void PlayBackgroundMusic(string name)
    {
        if (!_soundListDicoBackground.ContainsKey(name)) Debug.LogError("Mauvais string pour la musique de fond : " + name);
        else
        {
            backgroundMusic.Stop();
            backgroundMusic.clip = _soundListDicoBackground[name];
            backgroundMusic.Play();
        }
    } 
    public void PlayUISound(string name)
    {
        if (!_soundListDicoUI.ContainsKey(name)) Debug.LogError("Mauvais string pour le son UI : " + name);
        else uiSound.PlayOneShot(_soundListDicoUI[name]);
    }
    public void PlayAnnouncerSound(string name)
    {
        if (!_soundListDicoAnnouncer.ContainsKey(name)) Debug.LogError("Mauvais string pour l'annoncer : " + name);
        else
        {
            announcer.Stop();
            announcer.clip = _soundListDicoAnnouncer[name];
            announcer.Play();
        }
    }
    public void PlayInGameSound(string name)
    {
        if (!_inGameSoundDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son in game : " + name);
        else inGameSound2D.PlayOneShot(_inGameSoundDico[name]);
    }

}