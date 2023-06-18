using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;

namespace Project
{
    public class OpenCloseManager : Singleton<OpenCloseManager>
    {
        [SerializeField] [ReadOnlyField] public List<OpenCloseUI> _openCloseUIStack = new List<OpenCloseUI>();
        [SerializeField] private CursorLockMode _previousLockMode;

        private bool _inUse = false;
        
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
        

        public void Register(OpenCloseUI openCloseUI)
        {
            if (openCloseUI._openOrCloseWithKey)
            {
                _openCloseUIStack.Add(openCloseUI);
            }
            if (openCloseUI._dontUseButton == false)
            {
                // openCloseUI._button.onClick.AddListenerExtended(Raise);
                openCloseUI._button.onClick.AddListener(() => RaiseSpecial(openCloseUI));
            }
        }

        public void Unregister(OpenCloseUI openCloseUI)
        {
            if (openCloseUI.name == "Quit Button") openCloseUI.CloseUI(false);
            else openCloseUI.CloseUI();
            
            if (openCloseUI._openOrCloseWithKey)
            {
                _openCloseUIStack.Remove(openCloseUI);
            }

            if (openCloseUI._dontUseButton == false)
            {
                // openCloseUI._button.onClick.RemoveListenerExtended(Raise);             
                openCloseUI._button.onClick.RemoveAllListeners();             
            }
        }

        
        private void Raise()
        {
            if (_openCloseUIStack == null || _openCloseUIStack.Count == 0) return;
            
            var ins = _openCloseUIStack[^1];

            if (ins.stateToDo == OpenCloseUI.EOpenClose.Open)
            {
                CursorManager.instance.ApplyNewCursor(new CursorState(CursorLockMode.Confined, "UI"));
                InputManager.instance.SwitchPlayerInputMap("UI");
                ins.OpenUI();
                _inUse = true;
            }
            else
            {
                ins.CloseUI();
                _openCloseUIStack.Remove(ins);
                
                // if (_openCloseUIStack.Count == 0)
                // {
                //     _inUse = false;
                //     CursorManager.Revert();
                // }
                // else
                // {
                //     for (int i = 0; i < _openCloseUIStack.Count; i++)
                //     {
                //         if (_openCloseUIStack[i].stateToDo == OpenCloseUI.EOpenClose.Close)
                //         {
                //             //Cursor.lockState = CursorLockMode.None;
                //             CursorManager.ApplyNewCursor(new CusorState(CursorLockMode.Confined));
                //             return;
                //         }
                //     }
                //     
                //     //Cursor.lockState = _previousLockMode;
                //     CursorManager.Revert();
                //     InputManager.instance.SwitchPlayerInputMap("Player");

                // }
                
                if (_openCloseUIStack.Count == 0 || _openCloseUIStack[^1].stateToDo == OpenCloseUI.EOpenClose.Open)
                {
                    _inUse = false;
                    CursorManager.instance.Revert();
                }
            }
        }
        
        private void RaiseSpecial(OpenCloseUI a)
        {
            if (_openCloseUIStack == null || _openCloseUIStack.Count == 0) return;
            
            _openCloseUIStack.Add(a);
            var ins = _openCloseUIStack[^1];

            if (ins.stateToDo == OpenCloseUI.EOpenClose.Open)
            {
                CursorManager.instance.ApplyNewCursor(new CursorState(CursorLockMode.Confined, "UI"));
                InputManager.instance.SwitchPlayerInputMap("UI");
                ins.OpenUI();
                _inUse = true;
            }
            else
            {
                ins.CloseUI();
                _openCloseUIStack.Remove(ins);
                
                // if (_openCloseUIStack.Count == 0)
                // {
                //     _inUse = false;
                //     CursorManager.Revert();
                // }
                // else
                // {
                //     for (int i = 0; i < _openCloseUIStack.Count; i++)
                //     {
                //         if (_openCloseUIStack[i].stateToDo == OpenCloseUI.EOpenClose.Close)
                //         {
                //             //Cursor.lockState = CursorLockMode.None;
                //             CursorManager.ApplyNewCursor(new CusorState(CursorLockMode.Confined));
                //             return;
                //         }
                //     }
                //     
                //     //Cursor.lockState = _previousLockMode;
                //     CursorManager.Revert();
                //     InputManager.instance.SwitchPlayerInputMap("Player");

                // }
                
                if (_openCloseUIStack.Count == 0 || _openCloseUIStack[^1].stateToDo == OpenCloseUI.EOpenClose.Open)
                {
                    _inUse = false;
                    CursorManager.instance.Revert();
                }
            }
        }
    }
}
