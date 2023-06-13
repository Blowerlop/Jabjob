using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project
{
    public sealed class OpenCloseUI : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [Tooltip("The button is by default the one that is in the same GameObject as the script")] [SerializeField] private bool _overrideButton = false;
        [SerializeField] private bool _dontUseButton = false;
        [SerializeField] private Button _button;
        
        public enum EOpenClose{Open, Close}
        [field: SerializeField] public EOpenClose stateToDo { get; private set; } = EOpenClose.Open;
        private Action _stateToDoMethod;

        [SerializeField] private UnityEvent onOpenUIEvent;
        [SerializeField] private UnityEvent onCloseUIEvent;

        [SerializeField] private bool _openOrCloseWithKey = false;
        
        private void Awake()
        {
            if (_overrideButton == false && _dontUseButton == false) _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            switch (stateToDo)
            {
                case EOpenClose.Open:
                    if (_dontUseButton == false) _button.onClick.AddListener(OpenUI);  
                    
                    if (_openOrCloseWithKey)
                        // InputManager.instance.onEscapePressed.Subscribe(OpenUI, this);
                    OpenCloseManager.instance.Register(this);
                    
                    _stateToDoMethod = OpenUI;
                    break;
                
                case EOpenClose.Close:
                    if (_dontUseButton == false) _button.onClick.AddListener(CloseUI);
                    
                    if (_openOrCloseWithKey)
                        // InputManager.instance.onEscapePressed.Subscribe(CloseUI, this);
                        OpenCloseManager.instance.Register(this);

                    _stateToDoMethod = CloseUI;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (stateToDo)
            {
                case EOpenClose.Open:
                    if (OpenCloseManager.IsInstanceAlive && _openOrCloseWithKey)
                    {
                        // InputManager.instance.onEscapePressed.Unsubscribe(OpenUI);
                        OpenCloseManager.instance.Unregister(this);

                    }
                    break;
                
                case EOpenClose.Close:
                    if (OpenCloseManager.IsInstanceAlive && _openOrCloseWithKey)
                    {
                        // InputManager.instance.onEscapePressed.Unsubscribe(CloseUI);
                        OpenCloseManager.instance.Unregister(this);

                    }
                    break;
            }
            
            if (_dontUseButton == false) _button.onClick.RemoveAllListeners();
        }


        public void OpenUI()
        {
            onOpenUIEvent?.Invoke();
            _target.SetActive(true);
        }
        
        public void CloseUI()
        {
            onCloseUIEvent?.Invoke();
            _target.SetActive(false);
        }

        private void OpenOrCloseUI()
        {
            if (_target.activeSelf) 
                CloseUI();
            else 
                OpenUI();
        }

        public void ForceActivation() => _stateToDoMethod?.Invoke();
    }
    

// #if UNITY_EDITOR
//     [CustomEditor(typeof(CloseUI))]
//     public class CloseUIEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             var myScript = target as CloseUI;
//
//             if (myScript != null)
//             {
//                 myScript.overrideButton = GUILayout.Toggle(myScript.button, "Override Button");
//
//                 if (myScript.overrideButton)
//                 {
//                     myScript.bulletSpeed = EditorGUILayout.FloatField("Bullet speed", myScript.bulletSpeed);
//                     myScript.spray = GUILayout.Toggle(myScript.spray, "Spray");
//                 }
//             }
//         }
//     }
// #endif
}



