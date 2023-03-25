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
    float _rotateVerticalVelocity;

    [Header("References")]
    // [SerializeField] private GameObject _playerCamera;
    private CinemachineVirtualCamera _cinemachineCamera;
    [SerializeField] private Transform _cameraTarget;

    [Header("Multiplayer")]
    [SerializeField] private bool isMultiplayer = true;

    private Animator _animator;
    #endregion


    #region Updates
    private void Awake()
    {
        _cinemachineCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isMultiplayer == false) return;

        
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner == false)
        {
            _cinemachineCamera.Priority = -1;
            enabled = false;
        }
        else
        {
            // if (Camera.main != null) Camera.main.gameObject.SetActive(false);
            // _playerCamera.SetActive(true);
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
            _rotateVerticalVelocity += InputManager.instance.look.y * _sensitivity; 
            _rotateVerticalVelocity = ClampAngle(this._rotateVerticalVelocity, -60.0f, 60.0f);

            transform.Rotate(rotateHorizontalVelocity * Vector3.up);
            _cameraTarget.transform.localRotation = Quaternion.Euler(this._rotateVerticalVelocity, 0.0f, 0.0f);


            _animator.SetFloat("VerticalAngle", -_rotateVerticalVelocity/60f);
            if (rotateHorizontalVelocity > 0) _animator.SetBool("isRotatingLeft", true);
            else if (rotateHorizontalVelocity < 0) _animator.SetBool("isRotatingRight", true);
        }
       else
        {
            _animator.SetBool("isRotatingLeft", false);
            _animator.SetBool("isRotatingRight", false);
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
