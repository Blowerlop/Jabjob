using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Project
{
    public class EventAnimHelpers : MonoBehaviour
    {
        public AudioSource bodySourceSound;
        public SoundList[] soundList;
        public float fuckingRigWeight = 1f; 
        private Dictionary<string, AudioClip> _soundListDico = new Dictionary<string, AudioClip>();
        [SerializeField] PlayerShoot _playerShoot;
        [SerializeField] PlayerMovementController _playerMovement;
        [SerializeField]  Rig bodyRig; 

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

        private void Update()
        {
            if(bodyRig != null) bodyRig.weight = fuckingRigWeight;
        }

        public void SetRigWeight(float weight) 
        {
            fuckingRigWeight = weight;
        }
        #region Dash
        public void StartOfDash()
        {
            _playerMovement.DashEffectStart();
        }

        public void EndOfDash()
        {
            _playerMovement.EndOfDashVFX();
        }
        public void EndOfTrail()
        {
            _playerMovement.EndOfDashTrail();
        }
        #endregion

        #region Reload
        public void AutoReload()
        {
            _playerShoot.AutoReload();
        }
        public void EndOfReload()
        {
            _playerShoot.EndOfReload();
        }
        #endregion

        
    }
}
