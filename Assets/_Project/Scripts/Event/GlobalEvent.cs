using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project
{
    public class GlobalEvent
    {
        private List<IGlobalListener> _listeners = new List<IGlobalListener>();

        
        public void Invoke() 
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].action.Invoke();
            }
        }
        
        public void Subscribe(IGlobalListener listener)
        {
            if (IsListenerAlreadySubscribe(listener))
            {
                Debug.LogError($"{listener} is already registered in the event");
            }
            else
            {
                _listeners.Add(listener);
            }
        }

        public void Unsubscribe(IGlobalListener listener)
        {
            if (IsListenerAlreadySubscribe(listener) == false)
            {
                Debug.LogError($"{listener} is not registered in the event");
            }
            else
            {
                _listeners.Remove(listener);
            }
            
        }

        private bool IsListenerAlreadySubscribe(IGlobalListener listener)
        {
            return _listeners.Contains(listener);
        }
    }

    
}