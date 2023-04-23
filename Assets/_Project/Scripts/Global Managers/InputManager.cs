using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Project;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    #endregion
    
    #region Variables
    public Vector2 move;
    public Vector2 look;
    public bool isJumping, isShooting;
    public UnityEvent reload, openCommand, escapeKey;
    public bool isDashing;
    public bool isConsoleOpened;
    public bool isWeaponSelectionOpen;

    [FormerlySerializedAs("PlayerInput")] public PlayerInput playerInput;
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

    public void OnEscape()
    {
        escapeKey.Invoke();
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
        }catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void OnOpenGunSelection()
    {
        isWeaponSelectionOpen = true;
    }
    #endregion 

    public void SwitchPlayerInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
        Debug.Log("Input Manager - New current Action Map is " + playerInput.currentActionMap.name);
    }
}
