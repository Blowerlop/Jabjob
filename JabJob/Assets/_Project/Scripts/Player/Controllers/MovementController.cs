using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    #region Variables
    [Header("Movements")]
    [SerializeField] private float _moveSpeed = 6.0f;
    [SerializeField] private float _jumpHeight = 2.0f;
    [SerializeField] private float _gravityForce = -9.81f;
    [SerializeField] private bool _gravityEnabled = true;
    [SerializeField] private float _maximumVerticalVelocity = -40.0f;
    [SerializeField] private float _groundedVerticalVelocity = -2.0f;
    private bool _isGrounded;
    private float _verticalVelocity;

    [Header("References")]
    private Rigidbody _rigidbody;
    private CharacterController _characterController;
    #endregion


    #region Updates
    private void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGrounded();
        PerformJumpAndGravity();
        PerfomMovement();
    }
    #endregion


    #region Methods
    private void CheckGrounded()
    {
        _isGrounded = _characterController.isGrounded;
    }

    private void PerformJumpAndGravity()
    {
        if (_gravityEnabled == false) return;

        if (_isGrounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = _groundedVerticalVelocity;
            }

            if (InputManager.instance.isJumping)
            {
                _verticalVelocity = Mathf.Sqrt(-2.0f * _gravityForce * _jumpHeight);
            }
        }
        else
        {
            if (_verticalVelocity > _maximumVerticalVelocity)
            {
                _verticalVelocity += _gravityForce * Time.deltaTime;
            }
        }
    }

    private void PerfomMovement()
    {
        float targetVelocity;
        if (InputManager.instance.move == Vector2.zero)
        {
            targetVelocity = 0.0f;
        }
        else
        {
            targetVelocity = _moveSpeed;
        }

        float currentVelocity = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;
        float velocityToApply;

        if (currentVelocity < targetVelocity - 0.1f || currentVelocity > targetVelocity + 0.1f)
        {
            velocityToApply = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10.0f);
        }
        else
        {
            velocityToApply = targetVelocity;
        }





        Vector2 moveHorizontalVelocity = InputManager.instance.move * velocityToApply;
        Vector3 moveVerticalVelocity = Vector3.up * _verticalVelocity;


        _characterController.Move(
            (
            ((moveHorizontalVelocity.x * transform.right + moveHorizontalVelocity.y * transform.forward) + moveVerticalVelocity)
            * Time.deltaTime
            ));

        

    }
    #endregion
}
