using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using _Project.Scripts.Managers;
using Project;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Event = Project.Event;


public class InputManager : Singleton<InputManager>
{
    #region Singleton

    public override void Awake()
    {
        keepAlive = false;
        base.Awake();
        
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
    public Event onEscapePressed = new Event(nameof(onEscapePressed));
    public bool TabPressed;
    public PlayerInput _playerInput;
    
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
    public void OnTabulation()
    {
        TabPressed = true;
    }
    public void OnConsole()
    {
        isConsoleOpened = !isConsoleOpened;
        openCommand.Invoke();
    }

    public void OnScrollWheel()
    {
        try
        {
            Utils.Console.Instance.follow = false;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void OnOpenGunSelection()
    {
        isWeaponSelectionOpen = true;
    }

    public void OnEscape()
    {
        onEscapePressed.Invoke(this, false);
    }
     
    public void SwitchPlayerInputMap(string actionMap)
    {
        _playerInput.SwitchCurrentActionMap(actionMap);
    }
    

    #endregion
}