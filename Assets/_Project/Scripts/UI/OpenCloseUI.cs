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
        [SerializeField] public bool _dontUseButton = false;
        [SerializeField] public Button _button;
        
        public enum EOpenClose{Open, Close}
        [field: SerializeField] public EOpenClose stateToDo { get; private set; } = EOpenClose.Open;
        private Action _stateToDoMethod;

        [SerializeField] private UnityEvent onOpenUIEvent;
        [SerializeField] private UnityEvent onCloseUIEvent;

        [SerializeField] public bool _openOrCloseWithKey = false;

        private void Awake()
        {
            if (_overrideButton == false && _dontUseButton == false) _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            OpenCloseManager.instance.Register(this);
        }

        private void OnDisable()
        {
            if (OpenCloseManager.IsInstanceAlive)
            {
                OpenCloseManager.instance.Unregister(this);

            }
        }


        public void UnRegister()
        {
            OpenCloseManager.instance._openCloseUIStack.Remove(this);
        }

        public void OpenUI(bool notify = true)
        {
            if (notify)
            {
                onOpenUIEvent?.Invoke();
            }
            _target.SetActive(true);
        }

        public void CloseUI(bool notify = true)
        {
            if (notify)
            {
                onCloseUIEvent?.Invoke();
            }
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



