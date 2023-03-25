using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Project;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Event = Project.Event;


public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager instance { get; private set; }
    
    private void Awake()
    {
        instance = this;

        _playerInput = GetComponent<PlayerInput>();
    }

    #endregion
    
    #region Variables
    public Vector2 move;
    public Vector2 look;
    public bool isJumping, isShooting;
    public UnityEvent reload, openCommand;
    public bool isDashing;
    public bool isConsoleOpened;
    public bool isWeaponSelectionOpen;
    public readonly Event onEscapePressed = new Event(nameof(onEscapePressed));
    public readonly Event onEnterPressed = new Event(nameof(onEnterPressed));
    
    
    
    private PlayerInput _playerInput;
    public enum EActionMap
    {
        Player,
        UI
    }; 

    
    #endregion


    #region Methods
    public void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
        look.y *= -1;
    }

    public void OnJump()
    {
        isJumping = true;
    }

    public void OnFire()
    {
        isShooting = true;
    }
    public void OnUnFire()
    {
        isShooting = false;
    }

    public void OnReload()
    {
        reload.Invoke();
    }

    public void OnDash()
    {
        isDashing = true;
    }

    public void OnConsole()
    {
        isConsoleOpened = !isConsoleOpened;
        openCommand.Invoke();
    }

    public void OnOpenGunSelection()
    {
        isWeaponSelectionOpen = true;
    }

    public void OnEscape()
    {
        onEscapePressed.Invoke(this, true);
    }

    public void OnEnter()
    {
        onEnterPressed.Invoke(this, true);
    }
    #endregion


    public void SwitchActionMapTo(EActionMap actionMap)
    {
        _playerInput.SwitchCurrentActionMap(actionMap.ToString());
    }
}
