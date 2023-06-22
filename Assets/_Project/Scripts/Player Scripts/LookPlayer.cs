using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Project.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class LookPlayer : MonoBehaviour
    {
        private ulong _currentPlayerIndex;
        private CinemachineVirtualCamera _currentCinemachineVirtualCamera;
        private float _speed = 1.0f;
        private Vector3 _originalPosition;

        public void Awake()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            GameManager.instance.GetPlayer(NetworkManager.Singleton.LocalClientId).gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_currentCinemachineVirtualCamera != null)
            {
                _currentCinemachineVirtualCamera.Priority = -1;
                _currentCinemachineVirtualCamera.transform.position = _originalPosition;
            }
            GameManager.instance.GetPlayer(NetworkManager.Singleton.LocalClientId).gameObject.SetActive(true);
        } 

        private void FixedUpdate()
        {
            if (InputManager.instance.isShooting)
            {
                if (_currentCinemachineVirtualCamera != null)
                {
                    _currentCinemachineVirtualCamera.Priority = -1;
                    _currentCinemachineVirtualCamera.transform.position = _originalPosition;
                }
                
                _currentPlayerIndex++;

                if ((int)_currentPlayerIndex >= GameManager.instance.GetPlayers().Length)
                {
                    _currentPlayerIndex = 0;
                }

                GameManager.instance.GetPlayer(_currentPlayerIndex).GetComponentsInChildren<CinemachineVirtualCamera>().ForEach(
                    x =>
                    {
                        if (x.name == "KillCamCinemachineCamera")
                        {
                            _currentCinemachineVirtualCamera = x;
                            _currentCinemachineVirtualCamera.Priority = 1;
                            _originalPosition = _currentCinemachineVirtualCamera.transform.position;
                        }
                    });
                
                InputManager.instance.isShooting = false;
            }


            _speed += Input.mouseScrollDelta.y;
            

            Vector2 moveHorizontal = InputManager.instance.move * _speed;

            float upDown = 0.0f;
            // if (Input.GetKeyDown(KeyCode.Space));
            // {
            //     upDown += _speed;
            // }
            // if (Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     upDown -= _speed;
            // }

            
            
            if (_currentCinemachineVirtualCamera == null) return;

            var transform1 = _currentCinemachineVirtualCamera.transform;
            transform1.position +=
                (moveHorizontal.x * transform1.right +
                 moveHorizontal.y * transform1.forward +
                 upDown * transform1.up) * Time.fixedDeltaTime;
        }
    }
}
