using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
<<<<<<< HEAD
using static UnityEngine.InputSystem.InputAction;
=======
>>>>>>> main

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
<<<<<<< HEAD
    public Vector2 look;
    public bool isJumping;
    public bool isDashing;
=======
>>>>>>> main
    #endregion


    #region Methods
    public void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }
<<<<<<< HEAD

    public void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
        look.y *= -1;
    }

    public void OnJump(InputValue inputValue)
    {
        //isJumping = inputValue.Get<CallbackContext>().started;
        isJumping = true;
    }

    public void OnDash()
    {
        isDashing = true;
    }
=======
>>>>>>> main
    #endregion
}
