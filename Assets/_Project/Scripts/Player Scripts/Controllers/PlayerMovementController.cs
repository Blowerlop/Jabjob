using System;
using System.Collections;
using System.Collections.Generic;
using Project;
using Unity.Netcode;
using UnityEngine;
using _Project.Scripts.Managers;
using UnityEngine.VFX;
using Timer = Project.Utilities.Timer;

public class PlayerMovementController : NetworkBehaviour
{
    #region Variables
    public bool ShowDebug = true;
    [Header("Movements")]
    [SerializeField] private float _moveSpeed = 6.0f;
    private float _speedOffset = 0.1f;
    private Vector3 characterControllerLastVelocity;

    [Header("Dash")]
    [SerializeField] private int _dashNumber = 3;
    [SerializeField] private float _dashCooldown = 3;
    private int currentDashNumber = 3;
    private Timer _timer = new Timer();
    [SerializeField] private float _dashForce = 2.0f;

    [Header("Gravity")]
    [SerializeField] private bool _gravityEnabled = true;
    [SerializeField] private float _gravityForce = -9.81f;
    [SerializeField] private float _groundedVerticalVelocity = -2.0f;
    [SerializeField] private float _maximumVerticalVelocity = -40.0f;
    private float _verticalVelocity;

    [Header("Jump")]
    [SerializeField] private int _maxJumpNumber = 2;
    [SerializeField] private float _jumpHeight = 2.0f;
    [SerializeField] [ReadOnlyField] private int _jumpCount = 0;
    [SerializeField] [ReadOnlyField] private bool _canJump = true;
    [SerializeField] private float _jumpThreshold = 0.2f;

    [Header("Player Grounded")] 
    [SerializeField] [ReadOnlyField] private bool _isGrounded;
    private bool _isSoonGrounded;
    [SerializeField] private float _groundedOffset = -0.14f;
    [SerializeField] private float _groundedRadius = 0.28f;
    [SerializeField] [ReadOnlyField] private float _soonGroundedRadius = 1f;
    [SerializeField] private LayerMask _groundLayerMask;
    
    [Header("References")]
    private CharacterController _characterController; 

    [Header("Multiplayer")]
    [SerializeField] private bool _isMultiplayer = true;

    [Header("Sound")]
    public AudioSource bodySourceSound;
    public SoundList[] soundList;
    private Dictionary<string, AudioClip> _soundListDico = new Dictionary<string, AudioClip>();

    [Header("Visuals")]
    [SerializeField] private Animator _animatorMain;
    [SerializeField] private Animator _fakeWeaponAnim;
    [SerializeField] private Transform _DashVFX;
    [SerializeField] private ParticleSystem _dashParticles;
    [SerializeField] private ParticleSystem _smokeParticles;
    private Player _player ;
    [SerializeField] private Transform _cameraRoot;

    #endregion


    #region Updates
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _player = GetComponent<Player>(); 
        for (int i = 0; i < soundList.Length; i++)
        { 
            if (soundList[i].name != null && soundList[i].sound != null) _soundListDico.Add(soundList[i].name, soundList[i].sound);
        }
    }

    private void Start()
    {
        if (_isMultiplayer == false) return;
        UpdateDashColor(_player.playerColor);
        if (IsOwner == false) { enabled = false; return; }
        InitializeDashVFX();
        var smokePart = _smokeParticles.main;
        smokePart.playOnAwake = false;
        _smokeParticles.Stop();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        PerformJumpAndGravity();
        PerformMovement();

        PerformDash();
    }
    #endregion


    #region Methods
    private void CheckGrounded()
    {
        Vector3 position = transform.position;
        Vector3 spherePosition = new Vector3(position.x, position.y - _groundedOffset, position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayerMask, QueryTriggerInteraction.Ignore);
        _animatorMain.SetBool("isGrounded", _isGrounded);
        _isSoonGrounded = Physics.CheckSphere(spherePosition, _soonGroundedRadius, _groundLayerMask, QueryTriggerInteraction.Ignore);
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

            if (_jumpCount != 0)
            {
                ResetJump();
                _animatorMain.SetBool("JumpingDown", false);
                PlaySound("StepLanding");
            }
        }
        else
        {
            if (_isSoonGrounded && _verticalVelocity < 0f) _animatorMain.SetBool("JumpingDown", true);
            if (_verticalVelocity > _maximumVerticalVelocity)
            {
                _verticalVelocity += _gravityForce * Time.fixedDeltaTime;
            }
        }

        if (InputManager.instance.isJumping && _canJump)
        {
            StartCoroutine(Jump());
        }
        InputManager.instance.isJumping = false;
    }

    // public void Tp(Vector3 position) => _characterController.
    
    private void PerformMovement()
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

        Vector3 characterControllerVelocity = _characterController.velocity;
        //GameEvent.onPlayerVelocityChange.Invoke(this, false, characterControllerVelocity);
        // if (characterControllerLastVelocity != characterControllerVelocity) // MARCHE PAS CAR FLOAT ISSUE --> JE CHERCHERAI UN TRUC PLUS TARD
        // {
        //     GameEvent.onPlayerVelocityChange.Invoke(this, characterControllerVelocity);
        // }
        //characterControllerLastVelocity = characterControllerVelocity;

        float currentVelocity = new Vector3(characterControllerVelocity.x, 0.0f, characterControllerVelocity.z).magnitude;
        float velocityToApply;

        if (currentVelocity < targetVelocity - _speedOffset || currentVelocity > targetVelocity + _speedOffset)
        {
            velocityToApply = Mathf.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 10.0f);
        }
        else
        {
            velocityToApply = targetVelocity;
        }
        //GameEvent.onPlayerSpeedChange.Invoke(this, false, Mathf.Round(velocityToApply * 100.0f) / 100.0f); // ON VERRA PLUS TARD SI CA IMPACTE LES PERFORMANCES, SI CA LE FAIT JE CHERCHERaIS UNE BONNE CONDITIOn



        Vector2 moveHorizontalVelocity = InputManager.instance.move * velocityToApply;
        Vector3 moveVerticalVelocity = Vector3.up * _verticalVelocity;


        _characterController.Move(
            (
            ((moveHorizontalVelocity.x * transform.right + moveHorizontalVelocity.y * transform.forward) + moveVerticalVelocity)
            * Time.fixedDeltaTime
            ));


        //ANIMATION 

        if (_isGrounded)
        {
            NoMainRunningAnimBool();
            if (moveHorizontalVelocity.y > 0 && moveHorizontalVelocity.x > 0) _animatorMain.SetBool("isRunningFRight", true);
            else if (moveHorizontalVelocity.y > 0 && moveHorizontalVelocity.x < 0) _animatorMain.SetBool("isRunningFLeft", true);
            else if (moveHorizontalVelocity.y > 0 && moveHorizontalVelocity.x == 0) _animatorMain.SetBool("isRunningF", true);
            else if (moveHorizontalVelocity.y < 0 && moveHorizontalVelocity.x < 0) _animatorMain.SetBool("isRunningBLeft", true);
            else if (moveHorizontalVelocity.y < 0 && moveHorizontalVelocity.x > 0) _animatorMain.SetBool("isRunningBright", true);
            else if (moveHorizontalVelocity.y < 0 && moveHorizontalVelocity.x == 0) _animatorMain.SetBool("isRunningB", true);
            else if (moveHorizontalVelocity.y == 0 && moveHorizontalVelocity.x < 0) _animatorMain.SetBool("isRunningLeft", true);
            else if (moveHorizontalVelocity.y == 0 && moveHorizontalVelocity.x > 0) _animatorMain.SetBool("isRunningRight", true);

            _fakeWeaponAnim.SetBool("isRunning", moveHorizontalVelocity.y != 0 || moveHorizontalVelocity.x != 0);
        }

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

    private void PerformDash()
    {
        if (InputManager.instance.isDashing)
        {
            if (currentDashNumber > 0)
            {
                // Dash physics
                Vector3 currentVelocity = _characterController.velocity;
                if (currentVelocity == Vector3.zero)
                {
                    InputManager.instance.isDashing = false;
                    return;
                }
                
                if (Vector3.Dot(_characterController.velocity, transform.forward) < 0)
                {
                    AddForce(-transform.forward * _dashForce);
                }
                else
                {
                    float viewAngle = Mathf.Clamp(_cameraRoot.localRotation.eulerAngles.x, 0.0f, 60.0f);
                    Vector3 direction = _cameraRoot.forward * (_dashForce + ((2.5f * viewAngle) / 60));
                    AddForce(direction);
                }
                //
                _animatorMain.SetTrigger("Dash");
                PlaySound("Dash");
                currentDashNumber -= 1;
                GameEvent.onPlayerDashEvent.Invoke(this, false, _dashCooldown);
                _timer.StartTimerWithCallbackScaledTime(_dashCooldown+0.05f, ReloadDash);
            }
            InputManager.instance.isDashing = false;
        }

        if (currentDashNumber < _dashNumber)
        {
            _timer.StartTimerWithCallbackScaledTime(_dashCooldown+0.05f, ReloadDash);
        }
    }

    private void AddForce(Vector3 force)
    {
        _characterController.Move(force);
    }

    private IEnumerator Jump()
    {
        _canJump = false;
        if (_jumpCount < _maxJumpNumber)
        {
            _verticalVelocity = Mathf.Sqrt(-2.0f * _gravityForce * _jumpHeight);
            PlaySound("Jump");
            _animatorMain.SetTrigger("Jumped");
            _jumpCount++;
        }

        yield return new WaitForSeconds(_jumpThreshold);
        _canJump = true;
    }
    private void ResetJump()
    {
        _jumpCount = 0;
        StopCoroutine(Jump());
        _canJump = true;
    }

    public void NoMainRunningAnimBool()
    {
        foreach (AnimatorControllerParameter parameter in _animatorMain.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && (parameter.name.Length > 9 && parameter.name.Remove(9) == "isRunning"))
                _animatorMain.SetBool(parameter.name, false);
        }
    }


    private void ReloadDash()
    {
        currentDashNumber += 1;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false) return;
        if (!ShowDebug) return;
        Vector3 playerPosition = transform.position;

        // Draw grounded
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(new Vector3(playerPosition.x - _characterController.radius, playerPosition.y - _groundedOffset, playerPosition.z), _groundedRadius);

        // Draw Jump state
        Gizmos.color = _jumpCount == 0 ? Color.green : _jumpCount == 1 ? Color.yellow : Color.red;
        Gizmos.DrawSphere(new Vector3(playerPosition.x + _characterController.radius, playerPosition.y, playerPosition.z), 0.5f);
    }

    public void Teleport(Vector3 position)
    {
        _characterController.enabled = false;
        _characterController.transform.position = position;
        _characterController.enabled = true;
    }
    #endregion

    #region Visuals and Sound
    public void PlaySound(string name)
    {
        if (!_soundListDico.ContainsKey(name)) Debug.LogError("Mauvais string pour le son : " + name);
        else bodySourceSound.PlayOneShot(_soundListDico[name]);
    }

    private void InitializeDashVFX()
    {
        _DashVFX.SetParent(Camera.main.transform);
        _DashVFX.localPosition = new Vector3(0, 0, 2.5f);
        _DashVFX.localRotation = Quaternion.identity;
        VisualEffect dashEffect = _DashVFX.GetComponent<VisualEffect>();
        //Gradient GradientColor = ColorHelpersUtilities.GetGradient(_player.playerColor);    //Je trouve qu'en blanc c'est mieux pour tout le monde
        //dashEffect.SetGradient("ColorGradient", GradientColor);

    }
    public void UpdateDashColor(Color color)
    {
        Gradient GradientColor = ColorHelpersUtilities.GetGradient(color);
        ParticleSystem.ColorOverLifetimeModule col = _dashParticles.colorOverLifetime;
        col.color = GradientColor;
    }
    public void DashEffectStart()
    {
        PlaySound("Dash");
        if(IsOwner) _DashVFX.gameObject.SetActive(true); 
        _dashParticles.Play();
    }
    public void EndOfDashTrail()
    {
        _dashParticles.Stop();
    }
    public void EndOfDashVFX()
    {
        if (IsOwner) _DashVFX.gameObject.SetActive(false);
    }
    public void DisableSmoke()
    {
        var smokePart = _smokeParticles.main;
        smokePart.playOnAwake = false;
        _smokeParticles.Stop();
    }
    #endregion
}
