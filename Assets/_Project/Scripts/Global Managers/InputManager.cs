using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Project;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    #endregion
    
    #region Variables
    public Vector2 move;
    public Vector2 look;
    public bool isJumping, isShooting;
    public UnityEvent reload, openCommand;
    public bool isDashing;
    public bool isConsoleOpened;
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
    #endregion
}
