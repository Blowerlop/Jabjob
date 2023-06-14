using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class OpenCloseManager : Singleton<OpenCloseManager>
    {
        [SerializeField] private List<OpenCloseUI> _openCloseUIStack = new List<OpenCloseUI>();
        [SerializeField] private CursorLockMode _previousLockMode;

        public void Start()
        {
            _previousLockMode = Cursor.lockState;
            InputManager.instance.onEscapePressed.Subscribe(Raise, this);
        }

        private void OnDestroy()
        {
            if (InputManager.IsInstanceAlive)
            {
                InputManager.instance.onEscapePressed.Unsubscribe(Raise);
            }
        }
        

        public void Register(OpenCloseUI openCloseUI) => _openCloseUIStack.Add(openCloseUI);

        public void Unregister(OpenCloseUI openCloseUI) => _openCloseUIStack.Remove(openCloseUI);

        private void Raise()
        {
            if (_openCloseUIStack == null || _openCloseUIStack.Count == 0) return;
            
            var ins = _openCloseUIStack[^1];

            if (ins.stateToDo == OpenCloseUI.EOpenClose.Open)
            {
                _previousLockMode = Cursor.lockState;
                Cursor.lockState = CursorLockMode.None;
                InputManager.instance.SwitchPlayerInputMap("UI");
                ins.OpenUI();
            }
            else
            {
                ins.CloseUI();
                _openCloseUIStack.Remove(ins);
                if (_openCloseUIStack.Count == 0)
                {
                    Cursor.lockState = _previousLockMode;
                }
                else
                {
                    for (int i = 0; i < _openCloseUIStack.Count; i++)
                    {
                        if (_openCloseUIStack[i].stateToDo == OpenCloseUI.EOpenClose.Close)
                        {
                            Cursor.lockState = CursorLockMode.None;
                            return;
                        }
                    }
                    
                    Cursor.lockState = _previousLockMode;
                    InputManager.instance.SwitchPlayerInputMap("Player");

                }
            }
            
        }
    }
}
