using System;
using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Project
{
    [RequireComponent(typeof(ButtonUIVisualConfigurator))]
    public class ButtonUIBehaviourConfigurator : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [Header("References")] 
        private ButtonUIVisualConfigurator _buttonUIVisualConfigurator;

        [SerializeField] private EButtonState _defaultState = EButtonState.Normal;
        [SerializeField] [ReadOnlyField] private EButtonState _currentState;

        [SerializeField] private ButtonUIBehaviourConfigurator[] _buttonUIBehaviourConfigurators;
        [SerializeField] private UnityEvent _onButtonClickedEvent = new UnityEvent();
        
        private void Awake()
        {
            _buttonUIVisualConfigurator = GetComponent<ButtonUIVisualConfigurator>();
        }


        private void Start()
        {
            _currentState = _defaultState;
            _buttonUIVisualConfigurator.SetVisual(_currentState);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            HighlightedStateBehaviour(false);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            NormalStateBehaviour(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickedBehaviour(true);
        }


        public void NormalStateBehaviour(bool forceState)
        {
            if (_currentState == EButtonState.Normal) return;
            if (_currentState == EButtonState.Clicked && forceState == false) return;

            _currentState = EButtonState.Normal;
            _buttonUIVisualConfigurator.SetVisual(_currentState);
        }
        
        public void HighlightedStateBehaviour(bool forceState)
        {
            if (_currentState == EButtonState.Highlighted) return;
            if (_currentState == EButtonState.Clicked && forceState == false) return;

            _currentState = EButtonState.Highlighted;
            _buttonUIVisualConfigurator.SetVisual(_currentState);
        }

        public void ClickedBehaviour(bool notify)
        {
            if (_currentState == EButtonState.Clicked) return;
            
            _currentState = EButtonState.Clicked;
            _buttonUIVisualConfigurator.SetVisual(_currentState);
            _buttonUIBehaviourConfigurators.ForEach(x => x.NormalStateBehaviour(true));
            
            if (notify) _onButtonClickedEvent.Invoke();
        }
    }
}
