using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    #endregion


    #region Methods
    public void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }
    #endregion
}
