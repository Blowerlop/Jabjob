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
    private float _verticalVelocity;

    [Header("References")]
    private Rigidbody _rigidbody;
    private CharacterController CharacterController;
    #endregion


    #region Updates
    private void Awake()
    {
        //_rigidbody = GetComponent<Rigidbody>();
        CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGrounded();
        PerformJumpAndGravity();
        PerfomMovement();
    }
    #endregion


    #region Methods
    private bool CheckGrounded()
    {
        return true;
    }

    private void PerformJumpAndGravity()
    {

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

        float currentVelocity = new Vector3(CharacterController.velocity.x, 0.0f, CharacterController.velocity.z).magnitude;
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


        CharacterController.Move(
            (
            ((moveHorizontalVelocity.x * transform.right + moveHorizontalVelocity.y * transform.forward) + moveVerticalVelocity)
            * Time.deltaTime
            ));

        

    }
    #endregion
}
