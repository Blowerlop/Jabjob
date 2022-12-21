using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerCameraController : NetworkBehaviour
{
    #region Variables
    [Header("Camera")]
    [SerializeField] private float _sensitivity = 0.2f;
    [SerializeField] private float _threshold = 0.001f;
    float _cinemachineTargetPitch;

    [Header("References")]
    [SerializeField] private GameObject _playerCamera;
    private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraTarget;

    [Header("Multiplayer")]
    [SerializeField] private bool isMultiplayer = true;

    #endregion


    #region Updates
    private void Awake()
    {
        _cinemachineCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        if (isMultiplayer == false) return;

        if (IsOwner == false)
        {
            _cinemachineCamera.Priority = 0;
            enabled = false;
        }
        else
        {
            Camera.main.gameObject.SetActive(false);
            _playerCamera.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        PerfomCameraRotation();
    }
    #endregion


    #region Methods
    private void PerfomCameraRotation()
    {
        if (InputManager.instance.look.sqrMagnitude >= _threshold)
        {
            float rotateHorizontalVelocity = InputManager.instance.look.x * _sensitivity;
            float rotateVerticalVelocity = InputManager.instance.look.y * _sensitivity;

            _cinemachineTargetPitch += InputManager.instance.look.y * _sensitivity;
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -60.0f, 60.0f);

            transform.Rotate(rotateHorizontalVelocity * Vector3.up);
            _cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    #endregion
}
