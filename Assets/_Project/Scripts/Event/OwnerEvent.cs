using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class OwnerEvent
    {
        private List<IOwnerlListener> _listeners = new List<IOwnerlListener>();


        public void Invoke()
        {
            for (int i = 0; i < _listeners.Count; i++)
            {
                IOwnerlListener listener = _listeners[i];
                if (listener.IsPlayerOwner())
                {
                    _listeners[i].action.Invoke();
                }
            }
        }

        public void Subscribe(IOwnerlListener listener)
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

        public void Unsubscribe(IOwnerlListener listener)
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

        private bool IsListenerAlreadySubscribe(IOwnerlListener listener)
        {
            return _listeners.Contains(listener);
        }
    }
}