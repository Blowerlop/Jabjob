using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Project
{
    public class Event
    {
        private Action _action;
        private List<Action> _actionsTrackList = new List<Action>();

        private string _eventName;
        
        public void Invoke(object sender) 
        {
            Debug.Log($"<color=#00FF00>{sender} invoked {_eventName}</color>");
            _action?.Invoke();
        }

        public Event(string eventName)
        {
            _eventName = eventName;
        }

        public void Subscribe(Action action)
        {
            if (IsListenerAlreadySubscribe(action))
            {
                Debug.LogError($"Method - {action.Method.Name} - is already registered in the event");
            }
            else
            {
                _action += action;
                _actionsTrackList.Add(action);
                Debug.Log($"Method - {action.Method.Name} - has subscribed");
            }
        }

        public void Unsubscribe(Action action)
        {
            if (IsListenerAlreadySubscribe(action) == false)
            {
                Debug.LogError($"Method - {action.Method.Name} - is not registered in the event");
            }
            else
            {
                _action -= action;
                _actionsTrackList.Remove(action);
                Debug.Log($"Method - {action.Method.Name} - has unsubscribed");
            }
            
        }

        private bool IsListenerAlreadySubscribe(Action action)
        {
            return _actionsTrackList.Contains(action);
        }
    }
}