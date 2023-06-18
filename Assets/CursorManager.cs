 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [System.Serializable]
    public sealed class CursorState
    {
        public CursorLockMode previousCursorLockMode;
        public CursorLockMode currentCursorLockMode;
        public string previousActionMap;
        public string currentActionMap;
        
        public CursorState(CursorLockMode cursorLockMode, string actionMap)
        {
            previousCursorLockMode = Cursor.lockState;
            currentCursorLockMode = cursorLockMode;

            previousActionMap = InputManager.instance.GetPlayerInputMap();
            currentActionMap = actionMap;
        }
    }
    
    public class CursorManager : Singleton<CursorManager>
    {
        public Stack<CursorState> cursorStates = new Stack<CursorState>();
        [SerializeField] private List<CursorState> aa = new List<CursorState>();

        protected override void Awake()
        {
            keepAlive = false;
            base.Awake();
        }


        public void ApplyNewCursor(CursorState cursorState)
        {
            cursorStates.Push(cursorState);
            aa.Add(cursorState);
            
            Cursor.lockState = cursorState.currentCursorLockMode;
            InputManager.instance.SwitchPlayerInputMap(cursorState.currentActionMap);
        }

        public void Revert()
        {
            if (cursorStates.TryPop(out CursorState result))
            {
                Cursor.lockState = result.previousCursorLockMode;
                InputManager.instance.SwitchPlayerInputMap(result.previousActionMap);
                aa.Remove(result);
            }
            else
            {
                Debug.LogError("No cursor");
            }
            
        }
    }
}
