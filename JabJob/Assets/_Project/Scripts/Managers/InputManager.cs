using System.Collections;
using System.Collections.Generic;
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
    public bool isJumping, shoot;
    public UnityEvent reload;
<<<<<<< HEAD
=======
    public bool isDashing;
>>>>>>> main
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
        shoot = true;
<<<<<<< HEAD
        Debug.Log(shoot);
=======
>>>>>>> main
    }
    public void OnUnFire()
    {
        shoot = false;
<<<<<<< HEAD
        Debug.Log(shoot);
=======
>>>>>>> main
    }

    public void OnReload()
    {
        reload.Invoke();
    }
<<<<<<< HEAD
=======

    public void OnDash()
    {
        isDashing = true;
    }
>>>>>>> main
    #endregion
}
