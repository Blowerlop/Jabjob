using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    #region Variables
    #endregion


    #region Updates
    private void Update()
    {
        CheckGrounded();
        PerformJumpAndGravity();
        PerfomMovement();
    }
    #endregion


    #region Methods
    private bool CheckGrounded()
    {
        return true;
    }

    private void PerformJumpAndGravity()
    {

    }

    private void PerfomMovement()
    {

    }
    #endregion
}
