using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project
{
    public class GunSelectionMenu : MonoBehaviour
    {
        private GameObject player;

        private GameObject gunSelectionCamera;
        private CinemachineVirtualCamera gunSelectionVirtualCamera;

        [SerializeField] ScriptableObjectChanger scriptableObjectChanger;

        private void Start()
        {
            gunSelectionCamera = GameObject.FindGameObjectWithTag("GunSelectionCamera");
            //gunSelectionVirtualCamera = gunSelectionCamera.GetComponent<CinemachineVirtualCamera>();

            player = transform.root.gameObject;
        }

        // Update is called once per frame
        private void Update()
        {
            //if (InputManager.instance.isWeaponSelectionOpen)
            //{
            //    InputManager.instance.isWeaponSelectionOpen = false;
            //    scriptableObjectChanger.ActivateWeaponPreview();
            //    gunSelectionVirtualCamera.Priority = 10;
            //    gameObject.GetComponent<Canvas>().enabled = true;
            //}
        }

        public void CloseSelection()
        {
            gunSelectionVirtualCamera.Priority = 0;
            gameObject.GetComponent<Canvas>().enabled = false;
        }

        public void ChooseNewWeapon()
        {
            SOWeapon weaponChoose = scriptableObjectChanger.GetWeaponChoose();
            Weapon weaponScript = weaponChoose.model.GetComponent<Weapon>();
            WeaponManager weaponManagerScript = player.GetComponent<WeaponManager>();

            weaponManagerScript.EquipWeaponLocal(weaponScript);
        }
    }
}
