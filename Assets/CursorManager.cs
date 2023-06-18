 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [System.Serializable]
    public sealed class CusorState
    {
        public CursorLockMode previousCursorLockMode;
        public CursorLockMode currentCursorLockMode;
        public string previousActionMap;
        public string currentActionMap;
        
        public CusorState(CursorLockMode cursorLockMode, string actionMap)
        {
            previousCursorLockMode = Cursor.lockState;
            currentCursorLockMode = cursorLockMode;

            previousActionMap = InputManager.instance.GetPlayerInputMap();
            currentActionMap = actionMap;
        }
    }
    
    public class CursorManager : Singleton<CursorManager>
    {
        private Stack<CusorState> _list = new Stack<CusorState>();
        [SerializeField] private List<CusorState> aa = new List<CusorState>();

        protected override void Awake()
        {
            keepAlive = false;
            base.Awake();
        }


        public void ApplyNewCursor(CusorState cusorState)
        {
            _list.Push(cusorState);
            aa.Add(cusorState);
            
            Cursor.lockState = cusorState.currentCursorLockMode;
            InputManager.instance.SwitchPlayerInputMap(cusorState.currentActionMap);
        }

        public void Revert()
        {
            if (_list.TryPop(out CusorState result))
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
