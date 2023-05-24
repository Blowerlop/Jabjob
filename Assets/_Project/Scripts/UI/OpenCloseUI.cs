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
        [SerializeField] private EOpenClose _stateToDo = EOpenClose.Open;
        private Action _stateToDoMethod;

        [SerializeField] private UnityEvent onOpenUIEvent;
        [SerializeField] private UnityEvent onCloseUIEvent;

        private void Awake()
        {
            if (_overrideButton == false && _dontUseButton == false) _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            switch (_stateToDo)
            {
                case EOpenClose.Open:
                    if (_dontUseButton == false) _button.onClick.AddListener(OpenUI);  
                    _stateToDoMethod = OpenUI;
                    break;
                
                case EOpenClose.Close:
                    if (_dontUseButton == false) _button.onClick.AddListener(CloseUI);
                    
                    InputManager.instance.onEscapePressed.Subscribe(CloseUI, this);

                    _stateToDoMethod = CloseUI;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (_stateToDo)
            {
                case EOpenClose.Open:
                    break;
                
                case EOpenClose.Close:
                    if (InputManager.IsInstanceAlive)
                    {
                        InputManager.instance.onEscapePressed.Unsubscribe(CloseUI);
                    }
                    break;
            }
            
            if (_dontUseButton == false) _button.onClick.RemoveAllListeners();
        }


        private void OpenUI()
        {
            onOpenUIEvent?.Invoke();
            _target.SetActive(true);
        }
        
        private void CloseUI()
        {
            onCloseUIEvent?.Invoke();
            _target.SetActive(false);
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



