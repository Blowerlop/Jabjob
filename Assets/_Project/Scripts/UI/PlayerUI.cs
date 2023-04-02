using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


namespace Project
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] GameObject scoreBoard;
        [SerializeField] GameObject playerStatsList;
        [SerializeField] GameObject playerStatsPrefab;
        [SerializeField] GameObject escapeMenu;
        [SerializeField] int nbPlayer = 1;

        private bool isMenuOn = false;
        private PlayerShoot _playerShoot;
        private PlayerCameraController _playerCameraController;
        private void Awake()
        {
            for (int i = 0; i < nbPlayer; i++)
            {
                Instantiate(playerStatsPrefab, playerStatsList.transform);
            }
            _playerShoot = transform.root.GetComponent<PlayerShoot>();
            _playerCameraController = transform.root.GetComponent<PlayerCameraController>();
        }

        private void OnEnable()
        {
            InputManager.instance.escapeKey.AddListener(ToggleEscapeMenu);
        }

        private void OnDisable()
        {
            InputManager.instance.escapeKey.RemoveListener(ToggleEscapeMenu);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreBoard.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                scoreBoard.SetActive(false);
            }
        }


        public void ToggleEscapeMenu()
        {
            if (isMenuOn)
            {
                escapeMenu.SetActive(false);
                _playerShoot.enabled = true;
                _playerCameraController.enabled = true;
                isMenuOn = false;
            }
            else
            {
                escapeMenu.SetActive(true);
                _playerShoot.enabled = false;
                _playerCameraController.enabled = false;
                isMenuOn = true;
            }
            
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }

}