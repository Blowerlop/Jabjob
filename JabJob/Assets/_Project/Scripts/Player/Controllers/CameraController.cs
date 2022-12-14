using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    #region Variables
    [Header("Camera")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _sensitivity = 0.2f;
    [SerializeField] private float _threshold = 0.001f;
    float _cinemachineTargetPitch;

    #endregion


    #region Updates
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

            //var newVerticalRotation = rotateVerticalVelocity + _cameraRootTransform.localRotation.eulerAngles.x;
            //Debug.Log(newVerticalRotation + " ----------------");
            //Quaternion rotation = Quaternion.Euler(newVerticalRotation, 0, 0);
            //rotation.x = Mathf.Clamp(newVerticalRotation, -90.0f, 90.0f);

            transform.Rotate(rotateHorizontalVelocity * Vector3.up);
            _cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            //_cameraRootTransform.localRotation = rotation;
            //_cameraRootTransform.Rotate(rotateVerticalVelocity * Vector3.right);
            //Debug.Log(_cameraTarget.localEulerAngles.x + " ///////////////");
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
