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

        int knifeRotation = 1; 
        private void Awake()
        {
            for (int i = 0; i < soundList.Length; i++)
            {
                if (soundList[i].name != null && soundList[i].sound != null) _soundListDico.Add(soundList[i].name, soundList[i].sound);
            }
        }

        public void PlayOneShotSound(string name)
        {
            if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son : " + name);
            else bodySourceSound.PlayOneShot(_soundListDico[name]);
        }
        public void PlaySound(string name)
        {
            if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son : " + name);
            else
            {
                bodySourceSound.Stop();
                bodySourceSound.clip = _soundListDico[name]; 
                bodySourceSound.Play();
            }
        }

        private void Update()
        {
            if(bodyRig != null) bodyRig.weight = fuckingRigWeight;
        }

        // RIG
        public void SetRigWeight(float weight) 
        {
            fuckingRigWeight = weight;
        }

        // KNIFE
        public void TakeOutKnife()
        {
            _playerShoot.PutClipToWeapon();
        }
        //GUN
        public void StartFire()
        {
            _playerShoot.PutClipToWeapon();
        }
        public void PerformKnife()
        {
            _playerShoot.PerformKnifeCalculation();
            PlayOneShotSound("PaintBrush" + knifeRotation);
            knifeRotation = (knifeRotation + 1) % 3; 
        }
        // DASH
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

        //JUMp

        public void NoMoreInAir()
        {
            _playerMovement.NoMoreInAir();
        }
        // RELOAD
        public void StartOfReload()
        {
            PlaySound("Reload");
            _playerShoot.PutClipToLeftHand();
        }
        public void AutoReload()
        {
            _playerShoot.AutoReload();
        }
        public void ResetClipPosition()
        {
            _playerShoot.PutClipToWeapon(true);
        }
        public void EndOfReload()
        {
            _playerShoot.EndOfReload(); 
        }

        
    }
}
