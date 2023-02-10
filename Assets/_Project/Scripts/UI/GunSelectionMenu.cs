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
        private GameObject gunSelectionCamera;
        private CinemachineVirtualCamera gunSelectionVirtualCamera;

        private void Start()
        {
            gunSelectionCamera = GameObject.FindGameObjectWithTag("GunSelectionCamera");

            gunSelectionVirtualCamera = gunSelectionCamera.GetComponent<CinemachineVirtualCamera>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (InputManager.instance.isWeaponSelectionOpen)
            {
                gunSelectionVirtualCamera.Priority = 10;
                gameObject.GetComponent<Canvas>().enabled = true;
            }
        }

        public void closeSelection()
        {
            gunSelectionVirtualCamera.Priority = 0;
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }
}
