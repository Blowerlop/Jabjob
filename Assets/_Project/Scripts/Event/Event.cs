using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    public class Event
    {
        private event Action _action;

        
        public void Invoke() => _action?.Invoke();
        
        public void Subscribe(Action action)
        {
            if (IsActionAlreadySubscribe(action))
            {
                Debug.LogError($"{action.Method.Name} is already registered in the event");
            }
            else
            {
                _action += action;
            }
        }

        public void Unsubscribe(Action action)
        {
            if (IsActionAlreadySubscribe(action) == false)
            {
                Debug.LogError($"{action.Method.Name} is not registered in the event");
            }
            else
            {
                _action -= action;

            }
            
        }

        private bool IsActionAlreadySubscribe(Action action)
        {
            Delegate[] invocationList = _action.GetInvocationList();

            return invocationList.Contains(action);

        }
    }
}