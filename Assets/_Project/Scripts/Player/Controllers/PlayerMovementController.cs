using System.Collections;
using Project;
using Unity.Netcode;
using UnityEngine;


public class PlayerMovementController : NetworkBehaviour
{
    #region Variables
    [Header("Movements")]
    [SerializeField] private float _moveSpeed = 6.0f;
    private float _speedOffset = 0.1f;
    private float _horizontalVelocity;

    [Header("Gravity")]
    [SerializeField] private bool _gravityEnabled = true;
    [SerializeField] private float _gravityForce = -9.81f;
    [SerializeField] private float _groundedVerticalVelocity = -2.0f;
    [SerializeField] private float _maximumVerticalVelocity = -40.0f;
    private float _verticalVelocity;

    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 2.0f;
    private bool _isGrounded;
    private int _jumpCount = 0;
    private bool canJump = true;
    [SerializeField] private float _jumpThreshold = 0.2f;

    [Header("References")]
    private CharacterController _characterController;

    [Header("Multiplayer")]
    [SerializeField] private bool isMultiplayer = true;
    #endregion


    #region Updates
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (isMultiplayer == false) return;
        if (IsOwner == false) enabled = false;
    }

    private void Update()
    {
        
        CheckGrounded();
        PerformJumpAndGravity();
        PerfomMovement();

        if (InputManager.instance.isDashing)
        {
            AddForce(transform.forward * 2.0f);

            InputManager.instance.isDashing = false;
        }
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

            ResetJump();
        }
        else
        {
            if (_verticalVelocity > _maximumVerticalVelocity)
            {
                _verticalVelocity += _gravityForce * Time.deltaTime;
            }
        }

        if (InputManager.instance.isJumping && canJump)
        {
             StartCoroutine(Jump());
        }
        InputManager.instance.isJumping = false;
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

        if (currentVelocity < targetVelocity - _speedOffset || currentVelocity > targetVelocity + _speedOffset)
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


        /*

        float targetVelocity;
        if (InputManager.instance.move == Vector2.zero)
        {
            targetVelocity = 0.0f;
        }
        else
        {
            targetVelocity = _moveSpeed;
        }

        float currentVelocity = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z).magnitude;
        float velocityToApply;

        if (currentVelocity < targetVelocity - 0.1f || currentVelocity > targetVelocity + 0.1f)
        {
            velocityToApply = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 10.0f);
        }
        else
        {
            velocityToApply = targetVelocity;
        }

        Vector2 moveHorizontalVelocity = InputManager.instance.move * velocityToApply * _rigidbody.mass;
        Vector3 moveVerticalVelocity = Vector3.up * _verticalVelocity;
        CharacterController characterController;

        Debug.Log(moveHorizontalVelocity);

        //_rigidbody.velocity = (moveHorizontalVelocity.x * transform.right + moveHorizontalVelocity.y * transform.forward) + moveVerticalVelocity;
        _rigidbody.AddRelativeForce((moveHorizontalVelocity.x * transform.right + moveHorizontalVelocity.y * transform.forward), ForceMode.Force);

        */
    }

    private void AddForce(Vector3 force)
    {
        _characterController.Move(force);
    }

    private IEnumerator Jump()
    {
        canJump = false;
        if (_jumpCount < 2) 
        { 
            _verticalVelocity = Mathf.Sqrt(-2.0f * _gravityForce * _jumpHeight);
            _jumpCount++;
        }

        yield return new WaitForSeconds(_jumpThreshold);
        canJump = true;
    }

    private void ResetJump()
    {
        _jumpCount = 0;
        StopCoroutine(Jump());
        canJump = true;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false) return;
        Vector3 playerPosition = transform.position;

        // Draw grounded
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(new Vector3(playerPosition.x - _characterController.radius, playerPosition.y, playerPosition.z), 0.5f);

        // Draw Jump state
        Gizmos.color = _jumpCount == 0 ? Color.green : _jumpCount == 1 ? Color.yellow : Color.red;
        Gizmos.DrawSphere(new Vector3(playerPosition.x + _characterController.radius, playerPosition.y, playerPosition.z), 0.5f);
    }
    #endregion
}
