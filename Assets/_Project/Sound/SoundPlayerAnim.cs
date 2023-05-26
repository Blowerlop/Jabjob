using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class SoundPlayerAnim : MonoBehaviour
    {

        public AudioSource bodySourceSound;
        public SoundList[] soundList;
        private Dictionary<string, AudioClip> _soundListDico = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            for (int i = 0; i < soundList.Length; i++)
            {
                if (soundList[i].name != null && soundList[i].sound != null) _soundListDico.Add(soundList[i].name, soundList[i].sound);
            }
        }
        public void PlaySound(string name)
        {
            if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son : " + name);
            else bodySourceSound.PlayOneShot(_soundListDico[name]);
        }
    }
}
