using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 move;

    public void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }
}
